using System.Buffers.Binary;
using System.Text.Json;

namespace FarFlow.Core;

public sealed record WorkerRequest(string Session, string Action, string? ModelDirectory = null, string? Audio = null);
public sealed record WorkerReply(string Session, bool Success, string Text = "", string? Error = null, double Milliseconds = 0);

public static class PipeProtocol
{
    public const int MaximumFrameBytes = 12 * 1024 * 1024;
    public static async Task WriteAsync<T>(Stream stream, T value, CancellationToken token = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        if (bytes.Length > MaximumFrameBytes) throw new InvalidDataException("Message exceeds the local IPC limit.");
        var header = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(header, bytes.Length);
        await stream.WriteAsync(header, token);
        await stream.WriteAsync(bytes, token);
        await stream.FlushAsync(token);
    }
    public static async Task<T> ReadAsync<T>(Stream stream, CancellationToken token = default)
    {
        var header = new byte[4];
        await stream.ReadExactlyAsync(header, token);
        int length = BinaryPrimitives.ReadInt32LittleEndian(header);
        if (length <= 0 || length > MaximumFrameBytes) throw new InvalidDataException("Invalid local IPC message length.");
        var bytes = new byte[length];
        await stream.ReadExactlyAsync(bytes, token);
        return JsonSerializer.Deserialize<T>(bytes) ?? throw new InvalidDataException("Invalid local IPC message.");
    }
}
