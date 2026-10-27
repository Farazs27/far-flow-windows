using Microsoft.UI.Xaml;
namespace FarFlow_Windows;
public partial class App : Application
{
    private Window? window;
    public App()
    {
        UnhandledException += (_,args)=>WriteStartupError(args.Exception);
        try { InitializeComponent(); } catch(Exception e) {WriteStartupError(e);throw;}
    }
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    { try {window=new MainWindow();window.Activate();}catch(Exception e){WriteStartupError(e);throw;} }
    private static void WriteStartupError(Exception e)
    {
        try
        {
            var folder=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"FarFlowWindows");Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder,"startup-error.txt"),e.ToString());
        }catch(Exception){}
    }
}
