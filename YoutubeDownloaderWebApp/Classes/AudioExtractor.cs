using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class AudioExtractor
{
    private readonly string _ffmpegPath;

    public AudioExtractor(string ffmpegPath)
    {
        _ffmpegPath = ffmpegPath;
    }

    public async Task ExtractAudioAsync(string inputVideoPath, string outputAudioPath)
    {
        var arguments = $"-i \"{inputVideoPath}\" -vn -acodec libmp3lame \"{outputAudioPath}\"";

        var processStartInfo = new ProcessStartInfo
        {
            FileName = _ffmpegPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = processStartInfo })
        {
            process.Start();

            // Read output and error streams asynchronously
            var outputTask = Task.Run(() => process.StandardOutput.ReadToEndAsync());
            var errorTask = Task.Run(() => process.StandardError.ReadToEndAsync());

            process.WaitForExit(); // Wait for the process to exit

            // Get the output and error messages
            string output = await outputTask;
            string error = await errorTask;

            if (process.ExitCode != 0)
            {
                throw new Exception($"FFmpeg process exited with code {process.ExitCode}: {error}");
            }
        }
    }
}
