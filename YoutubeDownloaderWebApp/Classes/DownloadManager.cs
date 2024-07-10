using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;

public class DownloadManager
{
    private readonly YoutubeClient _youtubeClient;

    public DownloadManager()
    {
        _youtubeClient = new YoutubeClient();
    }

    public async Task<string> DownloadVideoAsync(string videoUrl, string outputPath)
    {
        var videoId = ExtractVideoId(videoUrl);
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoId);

        var streamInfo = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).FirstOrDefault();
        if (streamInfo == null)
            throw new Exception("Nenhum stream de vídeo encontrado.");

        await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, outputPath);
        return outputPath;
    }

    private string ExtractVideoId(string videoUrl)
    {
        var uri = new Uri(videoUrl);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        return query["v"] ?? uri.Segments.Last(); // Lida com URLs padrão e curtos
    }
}
