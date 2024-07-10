using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

public class SubtitleManager
{
    public void GenerateSrtFile(string transcription, string outputPath)
    {
        var lines = transcription.Split('\n');
        var srtContent = new StringBuilder();
        int counter = 1;

        foreach (var line in lines)
        {
            var parts = line.Split(new[] { ": " }, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                var times = parts[0].Split(new[] { " - " }, StringSplitOptions.None);
                if (times.Length == 2)
                {
                    srtContent.AppendLine(counter.ToString());
                    srtContent.AppendLine($"{FormatSrtTime(times[0])} --> {FormatSrtTime(times[1])}");
                    srtContent.AppendLine(parts[1]);
                    srtContent.AppendLine();
                    counter++;
                }
            }
        }

        File.WriteAllText(outputPath, srtContent.ToString());
    }

    private string FormatSrtTime(string time)
    {
        var ts = TimeSpan.Parse(time);
        return ts.ToString(@"hh\:mm\:ss\,fff");
    }

    public async Task AddSubtitlesToVideoAsync(string videoPath, string srtPath, string outputPath)
    {
        var conversion = await FFmpeg.Conversions.FromSnippet.AddSubtitle(videoPath, srtPath, outputPath);
        await conversion.Start();
    }
}
