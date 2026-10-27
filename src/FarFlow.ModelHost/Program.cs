using System.Diagnostics;
using System.IO.Pipes;
using FarFlow.Core;
using SherpaOnnx;

if (args.Length != 2 || args[0] != "--pipe" || !args[1].StartsWith("FarFlow-", StringComparison.Ordinal)) return 2;
using var pipe = new NamedPipeClientStream(".", args[1], PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
await pipe.ConnectAsync(15000);
OfflineRecognizer? recognizer = null;
try
{
    while (pipe.IsConnected)
    {
        WorkerRequest request;
        try { request = await PipeProtocol.ReadAsync<WorkerRequest>(pipe); }
        catch (EndOfStreamException) { break; }
        try
        {
            if (request.Action == "shutdown") break;
            var clock = Stopwatch.StartNew();
            if (request.Action == "load")
            {
                if (request.ModelDirectory is null || Path.GetFullPath(request.ModelDirectory) != Path.GetFullPath(SpeechPack.DefaultDirectory))
                    throw new InvalidDataException("Only the approved local model directory is accepted.");
                await SpeechPack.VerifyAsync(request.ModelDirectory);
                recognizer?.Dispose(); recognizer = null;
                var config = new OfflineRecognizerConfig();
                config.FeatConfig.SampleRate = AudioRules.SampleRate;
                config.FeatConfig.FeatureDim = 128;
                config.ModelConfig.Transducer.Encoder = Path.Combine(request.ModelDirectory, "encoder.int8.onnx");
                config.ModelConfig.Transducer.Decoder = Path.Combine(request.ModelDirectory, "decoder.int8.onnx");
                config.ModelConfig.Transducer.Joiner = Path.Combine(request.ModelDirectory, "joiner.int8.onnx");
                config.ModelConfig.Tokens = Path.Combine(request.ModelDirectory, "tokens.txt");
                config.ModelConfig.ModelType = "nemo_transducer";
                config.ModelConfig.NumThreads = Math.Clamp(Environment.ProcessorCount / 2, 1, 6);
                config.ModelConfig.Provider = "cpu";
                recognizer = new OfflineRecognizer(config);
                // A tiny warm-up evaluates the actual graph before reporting ready.
                using var warm = recognizer.CreateStream();
                warm.AcceptWaveform(AudioRules.SampleRate, new float[AudioRules.SampleRate]);
                recognizer.Decode(warm);
                await PipeProtocol.WriteAsync(pipe, new WorkerReply(request.Session, true, Milliseconds: clock.Elapsed.TotalMilliseconds));
            }
            else if (request.Action == "transcribe")
            {
                if (recognizer is null) throw new InvalidOperationException("Load the speech model first.");
                var samples = AudioRules.FromBytes(Convert.FromBase64String(request.Audio ?? ""));
                var text = "";
                if (AudioRules.HasSpeechEnergy(samples))
                {
                    using var stream = recognizer.CreateStream();
                    stream.AcceptWaveform(AudioRules.SampleRate, samples);
                    recognizer.Decode(stream);
                    text = stream.Result.Text.Trim();
                }
                await PipeProtocol.WriteAsync(pipe, new WorkerReply(request.Session, true, text, Milliseconds: clock.Elapsed.TotalMilliseconds));
            }
            else throw new InvalidDataException("Unknown worker request.");
        }
        catch (Exception error)
        {
            await PipeProtocol.WriteAsync(pipe, new WorkerReply(request.Session, false, Error: error.Message));
        }
    }
}
finally { recognizer?.Dispose(); }
return 0;
