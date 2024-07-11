using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.ClosedCaptions;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using YoutubeExplode.Common;
using System.Text.RegularExpressions;
using System.Globalization;

public class TranscriptionManager
{
    private readonly YoutubeClient _youtubeClient;

    public TranscriptionManager()
    {
        _youtubeClient = new YoutubeClient();
    }

    public async Task<(string Title, string ThumbnailUrl)> GetVideoInfoAsync(string videoUrl)
    {
        var videoId = ExtractVideoId(videoUrl);
        var video = await _youtubeClient.Videos.GetAsync(videoId);
        return (video.Title, video.Thumbnails.GetWithHighestResolution().Url);
    }

    public async Task<string> GetVideoTranscriptionAsync(string videoUrl)
    {
        var videoId = ExtractVideoId(videoUrl);
        var trackManifest = await _youtubeClient.Videos.ClosedCaptions.GetManifestAsync(videoId);

        if (trackManifest == null || !trackManifest.Tracks.Any())
            throw new Exception("Transcrição não disponível para este vídeo.");

        var trackInfo = trackManifest.Tracks.FirstOrDefault(t => t.Language.Code == "en") ?? trackManifest.Tracks.First();
        var captions = await _youtubeClient.Videos.ClosedCaptions.GetAsync(trackInfo);

        return ConvertToSrt(captions);
    }

    private string ConvertToSrt(ClosedCaptionTrack captions)
    {
        var srt = new StringBuilder();
        int count = 1;

        foreach (var caption in captions.Captions)
        {
            var start = TimeSpanToString(caption.Offset);
            var end = TimeSpanToString(caption.Offset + caption.Duration);
            srt.AppendLine(count.ToString());
            srt.AppendLine($"{start} --> {end}");
            srt.AppendLine(caption.Text);
            srt.AppendLine();
            count++;
        }

        return srt.ToString();
    }

    private string TimeSpanToString(TimeSpan time)
    {
        return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2},{time.Milliseconds:D3}";
    }

    public async Task SaveTranscriptionAsSrtAsync(string videoUrl, string directory)
    {
        var transcription = await GetVideoTranscriptionAsync(videoUrl);
        var videoInfo = await GetVideoInfoAsync(videoUrl);
        var fileName = $"{TratarTextos.SanitizeFileName(videoInfo.Title)}.srt";
        var filePath = Path.Combine(directory, fileName);

        Directory.CreateDirectory(directory); // Ensure the directory exists

        using (var writer = new StreamWriter(filePath, false))
        {
            await writer.WriteAsync(transcription);
        }
    }

    public async Task EmbedSubtitlesAsync(string videoFilePath, string subtitleFilePath, string outputFilePath, string ffmpegPath)
    {
        // Ensure the file paths are correctly formatted
        videoFilePath = TratarTextos.FormatFilePath(videoFilePath);
       // subtitleFilePath = TratarTextos.FormatFilePath(subtitleFilePath);
        outputFilePath = TratarTextos.FormatFilePath(outputFilePath);

        //var args = $"-i \"{videoFilePath}\" -vf \"subtitles={subtitleFilePath}\" -c:a copy \"{outputFilePath}\"";
        var args = $"-i \"{videoFilePath}\" -vf \"subtitles={subtitleFilePath}\" -c:a copy \"{outputFilePath}\"";



        // Log the arguments to check them
        Debug.WriteLine($"FFmpeg arguments: {args}");
      

        var processStartInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = args,
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

            // Log the output and error to help debug
            Debug.WriteLine($"FFmpeg output: {output}");
        
            Debug.WriteLine($"FFmpeg error: {error}");
           

            if (process.ExitCode != 0)
            {
                throw new Exception($"FFmpeg process exited with code {process.ExitCode}: {error}");
            }
        }
    }



    private string ExtractVideoId(string videoUrl)
    {
        var uri = new Uri(videoUrl);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        return query["v"] ?? uri.Segments.Last(); // Lida com URLs padrão e curtos
    }
   
}
