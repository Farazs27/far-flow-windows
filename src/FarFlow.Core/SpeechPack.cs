using System.Security.Cryptography;

namespace FarFlow.Core;

public sealed record ModelFile(string Name, long Bytes, string Sha256);

public static class SpeechPack
{
    public const string Id = "parakeet-v3-int8";
    public const string Revision = "2bda32ec70b097a55adaa07d9a7173915b43cc78";
    public const string Repository = "csukuangfj/sherpa-onnx-nemo-parakeet-tdt-0.6b-v3-int8";
    public static readonly ModelFile[] Files =
    [
        new("encoder.int8.onnx", 652184281, "acfc2b4456377e15d04f0243af540b7fe7c992f8d898d751cf134c3a55fd2247"),
        new("decoder.int8.onnx", 11845275, "179e50c43d1a9de79c8a24149a2f9bac6eb5981823f2a2ed88d655b24248db4e"),
        new("joiner.int8.onnx", 6355277, "3164c13fc2821009440d20fcb5fdc78bff28b4db2f8d0f0b329101719c0948b3"),
        new("tokens.txt", 93939, "d58544679ea4bc6ac563d1f545eb7d474bd6cfa467f0a6e2c1dc1c7d37e3c35d")
    ];
    public static long TotalBytes => Files.Sum(f => f.Bytes);
    public static Uri DownloadUri(ModelFile file) => new($"https://huggingface.co/{Repository}/resolve/{Revision}/{file.Name}");
    public static string DefaultDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FarFlowWindows", "Models", Id);

    public static async Task VerifyFileAsync(string path, ModelFile file, CancellationToken token = default)
    {
        if (!File.Exists(path) || new FileInfo(path).Length != file.Bytes)
            throw new InvalidDataException($"Missing or incomplete {file.Name}. Install the speech model first.");
        await using var stream = File.OpenRead(path);
        var hash = Convert.ToHexString(await SHA256.HashDataAsync(stream, token));
        if (!hash.Equals(file.Sha256, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException($"{file.Name} failed its integrity check. Remove it and download again.");
    }

    public static async Task VerifyAsync(string directory, CancellationToken token = default)
    {
        foreach (var file in Files) await VerifyFileAsync(Path.Combine(directory, file.Name), file, token);
    }
}
