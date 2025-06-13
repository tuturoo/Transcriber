using CommandWrapper.Core.Abstractions;

namespace CommandWrapper.Sox.Types;

public sealed class SoxProcess : CommandProcess
{
    internal SoxProcess(string executePath, string arguments) : base(executePath, arguments)
    { }

    public async Task ProcessAudioAsync(MemoryStream stream, CancellationToken token)
    {
        await stream.CopyToAsync(CreatedProcess.StandardInput.BaseStream, token);
        CreatedProcess.StandardInput.Close();

        stream.SetLength(0);
        stream.Position = 0;

        await CreatedProcess.StandardOutput.BaseStream.CopyToAsync(stream, token);
    }

    public async Task<byte[]> ProcessAudioAsync(byte[] data, CancellationToken token)
    { 
        using var transformedDataStream = new MemoryStream(data.Length);

        await CreatedProcess.StandardInput.BaseStream.WriteAsync(data, token);
        CreatedProcess.StandardInput.Close();

        await CreatedProcess.StandardOutput.BaseStream.CopyToAsync(transformedDataStream, token);

        return transformedDataStream.ToArray();
    }
}