using System.IO.Pipes;
using FarFlow.Core;

namespace FarFlow_Windows.Services;

public sealed class WorkerClient : IDisposable
{
    private NamedPipeServerStream? pipe;
    private OwnedWorker? worker;
    public bool Ready { get; private set; }
    public int? ProcessId => worker?.ProcessId;
    private readonly SemaphoreSlim gate = new(1,1);
    public async Task LoadAsync(CancellationToken token)
    {
        await gate.WaitAsync(token);
        try
        {
            Dispose();
            var name = "FarFlow-" + Guid.NewGuid().ToString("N");
            pipe = new NamedPipeServerStream(name, PipeDirection.InOut, 1, PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
            var executable = Path.Combine(AppContext.BaseDirectory, "ModelHost", "FarFlow.ModelHost.exe");
            worker = new OwnedWorker(executable, name);
            await pipe.WaitForConnectionAsync(token).WaitAsync(TimeSpan.FromSeconds(20), token);
            await RequestCoreAsync(new WorkerRequest(Guid.NewGuid().ToString("N"), "load", SpeechPack.DefaultDirectory), token);
            Ready = true;
        }
        catch { Dispose(); throw; }
        finally { gate.Release(); }
    }
    public async Task<WorkerReply> TranscribeAsync(float[] audio, CancellationToken token)
    {
        await gate.WaitAsync(token);
        try
        {
            if (!Ready) throw new InvalidOperationException("Load the local model first.");
            return await RequestCoreAsync(new WorkerRequest(Guid.NewGuid().ToString("N"), "transcribe",
                Audio: Convert.ToBase64String(AudioRules.ToBytes(audio))), token);
        }
        catch { Dispose(); throw; }
        finally { gate.Release(); }
    }
    private async Task<WorkerReply> RequestCoreAsync(WorkerRequest request, CancellationToken token)
    {
        var connection = pipe ?? throw new InvalidOperationException("Speech worker is not connected.");
        await PipeProtocol.WriteAsync(connection, request, token);
        var reply = await PipeProtocol.ReadAsync<WorkerReply>(connection, token);
        if (reply.Session != request.Session) throw new InvalidDataException("Stale speech session rejected.");
        if (!reply.Success) throw new InvalidOperationException(reply.Error ?? "Local speech worker failed.");
        return reply;
    }
    public async Task ShutdownAsync()
    {
        Ready = false;
        if (gate.Wait(0))
        {
            try
            {
                using var timeout = new CancellationTokenSource(400);
                if (pipe?.IsConnected == true)
                    await PipeProtocol.WriteAsync(pipe, new WorkerRequest("exit", "shutdown"), timeout.Token);
                await Task.Delay(80);
            }
            catch (Exception) { /* Job disposal below is the bounded shutdown fallback. */ }
            finally { gate.Release(); }
        }
        Dispose();
    }
    public void Dispose()
    {
        Ready = false;
        worker?.Dispose(); worker = null;
        pipe?.Dispose(); pipe = null;
    }
}
