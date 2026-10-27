using FarFlow.Core;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace FarFlow_Windows.Services;

public sealed class MicrophoneCapture : IDisposable
{
    private WasapiCapture? capture;
    private readonly MemoryStream audio = new();
    private TaskCompletionSource stopped = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private Exception? failure;
    public event Action<float>? LevelChanged;
    public event Action? LimitReached;
    public string NativeFormat { get; private set; } = "";
    public void Start()
    {
        if (capture is not null) throw new InvalidOperationException("A recording is already active.");
        audio.SetLength(0); failure = null;
        stopped = new(TaskCreationOptions.RunContinuationsAsynchronously);
        capture = new WasapiCapture();
        NativeFormat = capture.WaveFormat.ToString();
        // WASAPI shared-mode AutoConvertPcm/SrcDefaultQuality performs the explicit
        // endpoint-native -> 16 kHz mono Float32 conversion. Device support is tested at start.
        capture.WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(AudioRules.SampleRate, 1);
        capture.DataAvailable += OnData;
        capture.RecordingStopped += (_, e) => { failure = e.Exception; stopped.TrySetResult(); };
        try { capture.StartRecording(); }
        catch { capture.Dispose(); capture = null; throw; }
    }
    private void OnData(object? sender, WaveInEventArgs args)
    {
        if (audio.Length + args.BytesRecorded > AudioRules.SampleRate * AudioRules.MaximumSeconds * 4L)
        { LimitReached?.Invoke(); return; }
        audio.Write(args.Buffer, 0, args.BytesRecorded);
        double energy=0;
        for (int i=0;i+4<=args.BytesRecorded;i+=4) { float value=BitConverter.ToSingle(args.Buffer,i); energy+=value*value; }
        var level=args.BytesRecorded>0 ? (float)Math.Sqrt(energy/(args.BytesRecorded/4)) : 0;
        LevelChanged?.Invoke(float.IsFinite(level) ? Math.Clamp(level * 8, 0, 1) : 0);
    }
    public async Task<float[]> StopAsync()
    {
        if (capture is null) return [];
        capture.StopRecording();
        await stopped.Task.WaitAsync(TimeSpan.FromSeconds(3));
        capture.DataAvailable -= OnData; capture.Dispose(); capture = null;
        if (failure is not null) throw new InvalidOperationException("The microphone changed or stopped. Select a working Windows input device and try again.", failure);
        var result=AudioRules.FromBytes(audio.ToArray());
        audio.SetLength(0);
        return result;
    }
    public void Dispose()
    {
        capture?.StopRecording(); capture?.Dispose(); capture=null;
        audio.Dispose();
    }
}
