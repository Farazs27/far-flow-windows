using System.Diagnostics;
using System.IO.Pipes;
using System.Text.Json;
using FarFlow.Core;
using FarFlow_Windows.Services;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

if(args.Length==1&&args[0]=="--notepad-smoke"){await NotepadSmoke.RunAsync();return;}

if(args.Length==2&&args[0]=="--hold-worker")
{
 string name="FarFlow-"+Guid.NewGuid().ToString("N");
 using var pipe=new NamedPipeServerStream(name,PipeDirection.InOut,1,PipeTransmissionMode.Byte,PipeOptions.Asynchronous|PipeOptions.CurrentUserOnly);
 using var owned=new OwnedWorker(Path.Combine(AppContext.BaseDirectory,"ModelHost","FarFlow.ModelHost.exe"),name);
 await pipe.WaitForConnectionAsync().WaitAsync(TimeSpan.FromSeconds(20));
 await File.WriteAllTextAsync(args[1],JsonSerializer.Serialize(new{parent=Environment.ProcessId,child=owned.ProcessId}));
 await Task.Delay(Timeout.Infinite);return;
}
if(args.Contains("--install-model"))
{
 int previous=-1;
 await ModelInstaller.InstallAsync(new Progress<double>(x=>{int bucket=(int)(x*10);if(bucket!=previous){previous=bucket;Console.WriteLine($"Model download {bucket*10}%");}}),CancellationToken.None);
 Console.WriteLine("All approved model hashes verified.");return;
}
if(args.Length<2||args[0]!="--audio")throw new ArgumentException("Use --install-model, --hold-worker <pidfile>, or --audio <wave files>");
using var worker=new WorkerClient();using var timeout=new CancellationTokenSource(TimeSpan.FromMinutes(5));
var load=Stopwatch.StartNew();await worker.LoadAsync(timeout.Token);load.Stop();
Console.WriteLine($"Local CPU model loaded in {load.Elapsed.TotalSeconds:F2}s; PID {worker.ProcessId}");
var reports=new List<object>();
foreach(string file in args.Skip(1))
{
 using var reader=new AudioFileReader(file);
 ISampleProvider source=reader;
 if(source.WaveFormat.Channels==2)source=source.ToMono();
 if(source.WaveFormat.Channels!=1)throw new InvalidDataException("Test fixture must be mono/stereo.");
 if(source.WaveFormat.SampleRate!=16000)source=new WdlResamplingSampleProvider(source,16000);
 var samples=new List<float>();var buffer=new float[16000];int n;
 while((n=source.Read(buffer,0,buffer.Length))>0){samples.AddRange(buffer.Take(n));if(samples.Count>16000*90)throw new InvalidDataException("Fixture too long");}
 var result=await worker.TranscribeAsync(samples.ToArray(),timeout.Token);
 if(string.IsNullOrWhiteSpace(result.Text))throw new Exception("Empty transcript for "+Path.GetFileName(file));
 Console.WriteLine($"{Path.GetFileName(file)}: {result.Text}");
 reports.Add(new{fixture=Path.GetFileName(file),seconds=samples.Count/16000.0,milliseconds=result.Milliseconds,characters=result.Text.Length,rtf=result.Milliseconds/1000/(samples.Count/16000.0)});
}
var silence=await worker.TranscribeAsync(new float[16000],timeout.Token);
if(silence.Text!="")throw new Exception("Silence produced text");
int pid=worker.ProcessId!.Value;await worker.ShutdownAsync();
await Task.Delay(500);
bool alive;
try{using var process=Process.GetProcessById(pid);alive=!process.HasExited;}catch(ArgumentException){alive=false;}
if(alive)throw new Exception("Worker survived shutdown");
var evidence=new{utc=DateTime.UtcNow,provider="cpu",loadMilliseconds=load.Elapsed.TotalMilliseconds,fixtures=reports,silenceRejected=true,workerStopped=true,sourceCommit=Environment.GetEnvironmentVariable("GITHUB_SHA")};
Directory.CreateDirectory("artifacts/evidence");
await File.WriteAllTextAsync("artifacts/evidence/asr-smoke.json",JsonSerializer.Serialize(evidence,new JsonSerializerOptions{WriteIndented=true}));
Console.WriteLine("Speech smoke, silence rejection, and graceful worker shutdown passed.");
