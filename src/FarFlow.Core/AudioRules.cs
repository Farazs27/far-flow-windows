namespace FarFlow.Core;

public static class AudioRules
{
    public const int SampleRate = 16000;
    public const int MaximumSeconds = 90;
    public static bool HasSpeechEnergy(float[] samples)
    {
        if (samples.Length < SampleRate / 5 || samples.Length > SampleRate * MaximumSeconds) return false;
        double squares = 0; float peak = 0;
        foreach (float value in samples)
        {
            if (!float.IsFinite(value)) return false;
            squares += value * value;
            peak = Math.Max(peak, Math.Abs(value));
        }
        return peak >= .003f && Math.Sqrt(squares / samples.Length) >= .0005;
    }
    public static byte[] ToBytes(float[] samples)
    {
        if (samples.Length > SampleRate * MaximumSeconds) throw new InvalidDataException("Recording is too long.");
        var bytes = new byte[samples.Length * sizeof(float)];
        Buffer.BlockCopy(samples, 0, bytes, 0, bytes.Length);
        return bytes;
    }
    public static float[] FromBytes(byte[] bytes)
    {
        if (bytes.Length % 4 != 0 || bytes.Length > SampleRate * MaximumSeconds * 4)
            throw new InvalidDataException("Invalid recording buffer.");
        var samples = new float[bytes.Length / 4];
        Buffer.BlockCopy(bytes, 0, samples, 0, bytes.Length);
        if (samples.Any(x => !float.IsFinite(x))) throw new InvalidDataException("Non-finite recording samples.");
        return samples;
    }
}
