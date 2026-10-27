using System.Buffers.Binary;
using System.Security.Cryptography;
using FarFlow.Core;
int passed=0;
void Check(bool condition,string name){if(!condition)throw new Exception(name);Console.WriteLine("PASS "+name);passed++;}
async Task Reject<T>(Func<Task> action,string name) where T:Exception
{try{await action();throw new Exception("Expected "+typeof(T).Name+": "+name);}catch(T){Check(true,name);}}
var hold=new TriggerGesture();
Check(hold.Press(0)==GestureAction.Start,"capture begins on first edge");
Check(hold.Press(50)==GestureAction.None,"auto repeat cannot start a second recording");
Check(hold.Release(300)==GestureAction.Stop&&!hold.Active,"hold release finishes once");
Check(hold.Release(301)==GestureAction.None,"duplicate release ignored");
var taps=new TriggerGesture();taps.Press(0);taps.Release(60);
Check(taps.Tick(410)==GestureAction.None,"double-tap boundary remains open");
Check(taps.Tick(411)==GestureAction.Stop,"single short tap finishes after the window");
var hands=new TriggerGesture();hands.Press(0);hands.Release(80);hands.Press(250);
Check(hands.HandsFree&&hands.Active,"second tap locks hands-free");
Check(hands.Release(300)==GestureAction.None,"second release keeps recording");
Check(hands.Press(700)==GestureAction.Stop&&!hands.Active,"next press stops hands-free");
hands.Reset();Check(hands.Release(710)==GestureAction.None,"cancel invalidates later release");
Check(!AudioRules.HasSpeechEnergy(new float[16000]),"silence produces no inference request");
Check(!AudioRules.HasSpeechEnergy(Enumerable.Repeat(.2f,100).ToArray()),"tiny buffers cannot hallucinate a phrase");
var voice=Enumerable.Range(0,16000).Select(i=>(float)(.1*Math.Sin(i*.1))).ToArray();
Check(AudioRules.HasSpeechEnergy(voice),"normal speech-level signal accepted");
voice[5]=float.NaN;Check(!AudioRules.HasSpeechEnergy(voice),"non-finite samples rejected");
await Reject<InvalidDataException>(()=>Task.Run(()=>AudioRules.FromBytes(new byte[3])),"malformed PCM payload rejected");
var original=new float[]{.25f,-.5f,0};Check(original.SequenceEqual(AudioRules.FromBytes(AudioRules.ToBytes(original))),"audio round-trip preserves floats");
var frame=new MemoryStream();await PipeProtocol.WriteAsync(frame,new WorkerReply("test",true,"Nederlands: één, twee."));frame.Position=0;
Check((await PipeProtocol.ReadAsync<WorkerReply>(frame)).Text=="Nederlands: één, twee.","bounded IPC preserves Dutch Unicode");
var bad=new byte[4];BinaryPrimitives.WriteInt32LittleEndian(bad,PipeProtocol.MaximumFrameBytes+1);
await Reject<InvalidDataException>(()=>PipeProtocol.ReadAsync<WorkerReply>(new MemoryStream(bad)),"oversized IPC rejected before allocation");
await Reject<EndOfStreamException>(()=>PipeProtocol.ReadAsync<WorkerReply>(new MemoryStream(new byte[2])),"truncated IPC rejected");
string path=Path.GetTempFileName();
try
{
 await File.WriteAllBytesAsync(path,[1,2,3]);var expected=new ModelFile("fixture",3,Convert.ToHexString(SHA256.HashData(new byte[]{1,2,3})));
 await SpeechPack.VerifyFileAsync(path,expected);Check(true,"exact model file hash accepted");
 await File.WriteAllBytesAsync(path,[1,2,4]);await Reject<InvalidDataException>(()=>SpeechPack.VerifyFileAsync(path,expected),"same-size model corruption rejected");
 await File.WriteAllBytesAsync(path,[1]);await Reject<InvalidDataException>(()=>SpeechPack.VerifyFileAsync(path,expected),"partial model rejected");
}finally{File.Delete(path);}
Console.WriteLine($"{passed} contract checks passed.");
