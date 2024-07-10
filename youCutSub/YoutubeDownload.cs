using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;

public class YoutubeDownloader
{
    private readonly YoutubeClient youtubeClient;

    public YoutubeDownloader()
    {
        youtubeClient = new YoutubeClient();
    }

    public async Task DownloadVideoAsync(string videoUrl, string outputPath)
    {
        var videoId = ExtractVideoId(videoUrl);
        var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(videoId);

        // Get the highest quality video stream available
        var streamInfo = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).FirstOrDefault();

        // Download the video stream to a file
        if (streamInfo != null)
        {
            await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, outputPath);
        }
        else
        {
            throw new Exception("No video streams found.");
        }
    }

    private string ExtractVideoId(string videoUrl)
    {
        // Example: https://www.youtube.com/watch?v=xb0Z7HmqjV4
        var uri = new Uri(videoUrl);
        var query = uri.Query;
        var videoId = query.Substring(query.IndexOf("=") + 1);
        return videoId;
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        var downloader = new YoutubeDownloader();

        // Replace this with the YouTube video URL you want to download
        var videoUrl = "https://www.youtube.com/watch?v=xb0Z7HmqjV4";

        // Replace this with your desired output file path
        var outputPath = @"C:\Downloads\video.mp4";

        try
        {
            await downloader.DownloadVideoAsync(videoUrl, outputPath);
            Console.WriteLine("Video downloaded successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading video: {ex.Message}");
        }
    }
}
