using Xabe.FFmpeg;
using System.IO;
using System.Threading.Tasks;
using System;

public class VideoCutter
{
    public async Task CutVideoAsync(string filePath, string outputDirectory, TimeSpan duration)
    {
        Directory.CreateDirectory(outputDirectory);
        var mediaInfo = await FFmpeg.GetMediaInfo(filePath);

        int segmentIndex = 1;
        for (int i = 0; i < mediaInfo.Duration.TotalMinutes; i += (int)duration.TotalMinutes)
        {
            var outputSegmentPath = Path.Combine(outputDirectory, $"segment_{segmentIndex++}.mp4");
            var startTime = TimeSpan.FromMinutes(i);

            await FFmpeg.Conversions.New()
                .AddParameter($"-i \"{filePath}\"")
                .AddParameter($"-ss {startTime} -t {duration}")
                .SetOutput(outputSegmentPath)
                .Start();
        }
    }
}
