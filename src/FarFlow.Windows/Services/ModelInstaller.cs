using FarFlow.Core;

namespace FarFlow_Windows.Services;

public static class ModelInstaller
{
    // The only network path in the preview: an explicitly requested, pinned model download.
    public static async Task InstallAsync(IProgress<double> progress, CancellationToken token)
    {
        Directory.CreateDirectory(SpeechPack.DefaultDirectory);
        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(25) };
        long completed = 0;
        foreach (var file in SpeechPack.Files)
        {
            var path = Path.Combine(SpeechPack.DefaultDirectory, file.Name);
            try
            {
                await SpeechPack.VerifyFileAsync(path, file, token);
                completed += file.Bytes; progress.Report((double)completed / SpeechPack.TotalBytes); continue;
            }
            catch (InvalidDataException) { }
            var temporary = path + ".part";
            try
            {
                using var response = await client.GetAsync(SpeechPack.DownloadUri(file), HttpCompletionOption.ResponseHeadersRead, token);
                response.EnsureSuccessStatusCode();
                if (response.Content.Headers.ContentLength is long size && size != file.Bytes)
                    throw new InvalidDataException($"Unexpected download size for {file.Name}.");
                await using (var input = await response.Content.ReadAsStreamAsync(token))
                await using (var output = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None, 128 * 1024, true))
                {
                    var buffer = new byte[128 * 1024]; long written = 0; int count;
                    while ((count = await input.ReadAsync(buffer, token)) > 0)
                    {
                        written += count;
                        if (written > file.Bytes) throw new InvalidDataException("Download exceeded its approved size.");
                        await output.WriteAsync(buffer.AsMemory(0, count), token);
                        progress.Report((double)(completed + written) / SpeechPack.TotalBytes);
                    }
                }
                await SpeechPack.VerifyFileAsync(temporary, file, token);
                File.Move(temporary, path, true);
                completed += file.Bytes;
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
        await File.WriteAllTextAsync(Path.Combine(SpeechPack.DefaultDirectory, "ATTRIBUTION.txt"),
            "NVIDIA Parakeet TDT 0.6B v3, CC BY 4.0. ONNX conversion by csukuangfj.\n" +
            "Source: https://huggingface.co/nvidia/parakeet-tdt-0.6b-v3\n" +
            $"Conversion: https://huggingface.co/{SpeechPack.Repository}/tree/{SpeechPack.Revision}\n" +
            "License: https://creativecommons.org/licenses/by/4.0/\n" +
            "Conversion/quantization changes the format and precision. No endorsement by model authors is implied.\n", token);
    }
}
