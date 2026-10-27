using System.Diagnostics;
using FarFlow.Core;
using FarFlow_Windows.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
namespace FarFlow_Windows;

public sealed partial class MainWindow : Window
{
    private readonly WorkerClient worker=new();
    private readonly TriggerGesture gesture=new();
    private readonly CancellationTokenSource lifetime=new();
    private readonly DispatcherTimer timer=new(){Interval=TimeSpan.FromMilliseconds(40)};
    private GlobalTrigger? trigger;
    private ActivityBar? bar;
    private MicrophoneCapture? microphone;
    private Task? captureStart;
    private CancellationTokenSource? operation;
    private bool recording,busy,closing,closed;
    private int generation;
    private float amplitude;
    private long recordingAt;
    private bool heroLoaded,markLoaded;
    public MainWindow()
    {
        InitializeComponent();
        var work=Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(AppWindow.Id,Microsoft.UI.Windowing.DisplayAreaFallback.Primary).WorkArea;
        int width=Math.Min(1180,work.Width-48),height=Math.Min(850,work.Height-48);
        AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(work.X+(work.Width-width)/2,work.Y+(work.Height-height)/2,width,height));
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory,"Assets","AppIcon.ico"));
        AppWindow.Closing+=async(_,args)=>
        {
            if(closed)return;args.Cancel=true;if(closing)return;
            closing=true;generation++;lifetime.Cancel();operation?.Cancel();timer.Stop();trigger?.Dispose();microphone?.Dispose();bar?.Dispose();
            await worker.ShutdownAsync();closed=true;Close();
        };
        timer.Tick+=(_,_)=>
        {
            ApplyGesture(gesture.Tick(Environment.TickCount64));
            if(recording && Environment.TickCount64-recordingAt>=AudioRules.MaximumSeconds*1000)_=FinishAsync();
            bar?.Update(recording,amplitude);
        };
        timer.Start();
        try
        {
            bar=new ActivityBar();trigger=new GlobalTrigger();
            trigger.Edge+=down=>DispatcherQueue.TryEnqueue(()=>ApplyGesture(down?gesture.Press(Environment.TickCount64):gesture.Release(Environment.TickCount64)));
            trigger.Cancel+=()=>DispatcherQueue.TryEnqueue(CancelSession);
        }
        catch(Exception e){ShowError("Global shortcut unavailable. Record here remains available. "+e.Message);}
        if(SpeechPack.Files.All(f=>File.Exists(Path.Combine(SpeechPack.DefaultDirectory,f.Name))))_=LoadModelAsync();
    }
    private void ApplyGesture(GestureAction action)
    {if(closing)return;if(action==GestureAction.Start)_=BeginAsync();if(action==GestureAction.Stop)_=FinishAsync();}
    private async Task BeginAsync()
    {
        if(recording||busy||closing)return;
        if(!worker.Ready){gesture.Reset();StatusText.Text="Install and load your local model first.";return;}
        int session=++generation;recording=true;recordingAt=Environment.TickCount64;amplitude=0;
        Notice.IsOpen=false;StatusText.Text="Listening · English + Nederlands";SetControls();
        microphone=new MicrophoneCapture();microphone.LevelChanged+=value=>amplitude=value;
        microphone.LimitReached+=()=>DispatcherQueue.TryEnqueue(()=>_=FinishAsync());
        captureStart=Task.Run(microphone.Start);
        try{await captureStart;}
        catch(Exception e)
        {
            if(session!=generation||closing)return;
            recording=false;gesture.Reset();microphone?.Dispose();microphone=null;
            ShowError("Microphone unavailable. Check Windows privacy and your input device. "+e.Message);SetControls();
        }
    }
    private async Task FinishAsync()
    {
        if(!recording||closing)return;
        var target=TextDelivery.Snapshot();int session=generation;
        recording=false;busy=true;gesture.Reset();SetControls();StatusText.Text="Finishing your words locally…";
        var clock=Stopwatch.StartNew();operation=CancellationTokenSource.CreateLinkedTokenSource(lifetime.Token);operation.CancelAfter(TimeSpan.FromMinutes(2));
        try
        {
            if(captureStart is not null)await captureStart;
            var samples=microphone is null?[]:await microphone.StopAsync();microphone?.Dispose();microphone=null;
            if(!AudioRules.HasSpeechEnergy(samples)){StatusText.Text="No clear speech captured. Try speaking closer to your microphone.";return;}
            var result=await worker.TranscribeAsync(samples,operation.Token);
            if(session!=generation||closing)return;
            Transcript.Text=result.Text;
            StatusText.Text=string.IsNullOrWhiteSpace(result.Text)?"No words recognized. Try again or check your microphone.":TextDelivery.InsertIntoNotepad(target,result.Text);
            TimingText.Text=$"{samples.Length/(double)AudioRules.SampleRate:0.0}s audio · {clock.Elapsed.TotalSeconds:0.00}s after stop · local CPU · audio not saved";
        }
        catch(OperationCanceledException){if(session==generation)StatusText.Text="Canceled. Load the model to begin again.";}
        catch(Exception e){if(session==generation&&!closing)ShowError("The session stopped safely. "+e.Message);}
        finally{if(session==generation){microphone?.Dispose();microphone=null;busy=false;operation?.Dispose();operation=null;SetControls();}}
    }
    private void CancelSession()
    {
        if((!recording&&!busy)||closing)return;
        generation++;gesture.Reset();operation?.Cancel();microphone?.Dispose();microphone=null;worker.Dispose();recording=busy=false;amplitude=0;
        DownloadProgress.Visibility=Visibility.Collapsed;StatusText.Text="Canceled · load the local model to begin again";ModelStatus.Text="Model unloaded. No microphone is active.";SetControls();
    }
    private void SetControls()
    {
        RecordButton.IsEnabled=worker.Ready&&!busy;RecordButton.Label=recording?"Finish":"Record here";CancelButton.IsEnabled=recording||busy;
        DownloadButton.IsEnabled=LoadButton.IsEnabled=UnloadButton.IsEnabled=!busy&&!recording;
        ReadyLabel.Text=recording?"● LISTENING":worker.Ready?"● LOCAL & READY":"○ MODEL NOT LOADED";bar?.Update(recording,amplitude);
    }
    private async Task LoadModelAsync()
    {
        if(busy||recording||closing)return;
        int session=++generation;busy=true;SetControls();
        operation=CancellationTokenSource.CreateLinkedTokenSource(lifetime.Token);operation.CancelAfter(TimeSpan.FromMinutes(3));
        ModelStatus.Text="Verifying and warming the local model…";StatusText.Text="Preparing local speech recognition…";
        try
        {
            await Task.Run(()=>SpeechPack.VerifyAsync(SpeechPack.DefaultDirectory,operation.Token),operation.Token);
            await worker.LoadAsync(operation.Token);if(session!=generation||closing)return;
            ModelStatus.Text="Ready · Parakeet v3 · CPU · English + Nederlands. No microphone is active.";StatusText.Text="Ready. Open Notepad and hold F8, or record here.";Notice.IsOpen=false;
        }
        catch(OperationCanceledException){if(session==generation)StatusText.Text="Model preparation canceled.";}
        catch(Exception e){if(session==generation&&!closing)ShowError(e.Message);}
        finally{if(session==generation){busy=false;operation?.Dispose();operation=null;SetControls();}}
    }
    private async void Download_Click(object sender,RoutedEventArgs e)
    {
        if(busy||recording)return;
        var dialog=new ContentDialog{XamlRoot=Root.XamlRoot,Title="Install your local speech model?",Content="Download 670 MB from Hugging Face. Speech stays on this PC. NVIDIA Parakeet v3 is licensed under CC BY 4.0; ONNX conversion by csukuangfj. No account needed.",PrimaryButtonText="Download",CloseButtonText="Not now",DefaultButton=ContentDialogButton.Primary};
        if(await dialog.ShowAsync()!=ContentDialogResult.Primary)return;
        int session=++generation;busy=true;SetControls();DownloadProgress.Visibility=Visibility.Visible;operation=CancellationTokenSource.CreateLinkedTokenSource(lifetime.Token);
        try
        {
            await ModelInstaller.InstallAsync(new Progress<double>(value=>{DownloadProgress.Value=value*100;ModelStatus.Text=$"Installing local model · {value:P0}";}),operation.Token);
            if(session!=generation||closing)return;ModelStatus.Text="Download verified. Preparing the model…";
        }
        catch(OperationCanceledException){if(session==generation)ModelStatus.Text="Download canceled. Verified files can be reused.";return;}
        catch(Exception error){if(session==generation&&!closing)ShowError("Model installation stopped. "+error.Message);return;}
        finally{if(session==generation){busy=false;operation?.Dispose();operation=null;DownloadProgress.Visibility=Visibility.Collapsed;SetControls();}}
        if(session==generation)await LoadModelAsync();
    }
    private async void Load_Click(object sender,RoutedEventArgs e)=>await LoadModelAsync();
    private async void Unload_Click(object sender,RoutedEventArgs e){await worker.ShutdownAsync();ModelStatus.Text="Model unloaded. No microphone is active.";StatusText.Text="Model unloaded. Load it when you’re ready.";SetControls();}
    private async void Record_Click(object sender,RoutedEventArgs e){if(recording)await FinishAsync();else await BeginAsync();}
    private void Cancel_Click(object sender,RoutedEventArgs e)=>CancelSession();
    private void Copy_Click(object sender,RoutedEventArgs e){if(string.IsNullOrWhiteSpace(Transcript.Text))return;var package=new DataPackage();package.SetText(Transcript.Text);Clipboard.SetContent(package);StatusText.Text="Copied. Paste where you want your words.";}
    private void Clear_Click(object sender,RoutedEventArgs e){Transcript.Text="";TimingText.Text="Transcripts stay in memory for this session. Audio is never saved.";}
    private void OpenModelFolder_Click(object sender,RoutedEventArgs e){Directory.CreateDirectory(SpeechPack.DefaultDirectory);Process.Start(new ProcessStartInfo("explorer.exe",SpeechPack.DefaultDirectory){UseShellExecute=true});}
    private void MicrophoneSettings_Click(object sender,RoutedEventArgs e)=>Process.Start(new ProcessStartInfo("ms-settings:privacy-microphone"){UseShellExecute=true});
    private void ShowError(string message){Notice.Title="Far Flow needs a moment";Notice.Message=message;Notice.IsOpen=true;StatusText.Text=message;}
    private void Navigation_SelectionChanged(NavigationView sender,NavigationViewSelectionChangedEventArgs args)
    {
        if(DictationPanel is null)return;
        var tag=(args.SelectedItem as NavigationViewItem)?.Tag?.ToString()??"dictation";
        DictationPanel.Visibility=tag=="dictation"?Visibility.Visible:Visibility.Collapsed;ModelPanel.Visibility=tag=="model"?Visibility.Visible:Visibility.Collapsed;GuidePanel.Visibility=tag=="guide"?Visibility.Visible:Visibility.Collapsed;
        PageTitle.Text=tag=="model"?"A voice of your own.":tag=="guide"?"Make yourself at home.":"Room for your thoughts.";PageLabel.Text=tag=="model"?"PRIVATE BY DESIGN":tag=="guide"?"THE FIRST SESSION":"YOUR DAILY PRACTICE";
    }
    private void Theme_Changed(object sender,SelectionChangedEventArgs args){if(Root is not null&&sender is ComboBox choice)Root.RequestedTheme=choice.SelectedIndex switch{0=>ElementTheme.Light,1=>ElementTheme.Dark,_=>ElementTheme.Default};}
    private void Artwork_Opened(object sender,RoutedEventArgs args)
    {
        if(sender==HeroArtwork)heroLoaded=true;if(sender==BrandMark)markLoaded=true;
        if(heroLoaded&&markLoaded&&Environment.GetCommandLineArgs().Contains("--window-smoke"))
            File.WriteAllText(Path.Combine(AppContext.BaseDirectory,"ui-smoke.json"),"{\"heroLoaded\":true,\"logoLoaded\":true}");
    }
}
