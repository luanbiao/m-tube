using System;
using System.Diagnostics;
using System.Threading.Tasks;
using YoutubeDownloaderWebApp.Classes;

public class AudioExtractor
{
    private string ffmpegPath;

    public AudioExtractor(string ffmpegPath)
    {
        this.ffmpegPath = ffmpegPath;
    }

    public async Task ExtractAudioAsync(string videoPath, string audioPath)
    {
        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(ffmpegPath)
            {
                Arguments = $"-i \"{videoPath}\" -q:a 0 -map a \"{audioPath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();
                Debug.WriteLine("Process started.");
                await process.WaitForExitAsync(); // Usando a função de extensão
                Debug.WriteLine("Process completed.");

                if (process.ExitCode != 0)
                {
                    throw new Exception($"FFmpeg exited with code {process.ExitCode}");
                }
            }

            Debug.WriteLine($"Audio extracted successfully and saved to: {audioPath}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error extracting audio: {ex.Message}");
            throw;
        }
    }
}
