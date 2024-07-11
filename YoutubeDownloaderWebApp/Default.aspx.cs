using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
using Xabe.FFmpeg;

namespace YoutubeDownloaderWebApp
{
    public partial class Default : Page
    {
        private static string _videoTitle;
        private static string _outputPath;
        private static string _videoThumbnailUrl;

        private readonly DownloadManager _downloadManager;
        private readonly VideoCutter _videoCutter;
        private readonly TranscriptionManager _transcriptionManager;
        private readonly SubtitleManager _subtitleManager;

        public Default()
        {
            _downloadManager = new DownloadManager();
            _videoCutter = new VideoCutter();
            _transcriptionManager = new TranscriptionManager();
            _subtitleManager = new SubtitleManager();
        }

        protected async void BtnGetInfo_Click(object sender, EventArgs e)
        {
            string videoUrl = txtVideoUrl.Text.Trim();

            try
            {
                lblStatus.Text = "Buscando informações do vídeo...";
                var videoInfo = await _transcriptionManager.GetVideoInfoAsync(videoUrl);
                _videoTitle = videoInfo.Title;
                _videoThumbnailUrl = videoInfo.ThumbnailUrl;
                lblVideoTitle.Text = $"Título do Vídeo: {_videoTitle}";
                imgThumbnail.ImageUrl = _videoThumbnailUrl;
                imgThumbnail.Visible = true;
                lblStatus.Text = "Informações obtidas com sucesso!";
                ScriptManager.RegisterStartupScript(this, GetType(), "showDownloadButton", "showDownloadButton();", true);
                ScriptManager.RegisterStartupScript(this, GetType(), "showTranscreverButton", "showTranscreverButton();", true);
                btnDownload.Style["display"] = "inline-block"; // Mostrar o botão de download
                btnTranscrever.Style["display"] = "inline-block"; // Mostrar o botão de transcrição
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Erro ao buscar informações do vídeo: {ex.Message}";
            }
        }

        protected async void BtnDownload_Click(object sender, EventArgs e)
        {
            string sanitizedTitle = TratarTextos.SanitizeFileName(_videoTitle);
            _outputPath = Server.MapPath($"~/Downloads/{sanitizedTitle}.mp4");

            try
            {
                if (File.Exists(_outputPath))
                {
                    lblStatus.Text = "O vídeo já existe na pasta.";
                    btnRecortar.Style["display"] = "inline-block";
                    btnTranscrever.Style["display"] = "inline-block";
                    btnExtrairAudio.Style["display"] = "inline-block";
                    return;
                }

                lblStatus.Text = "Iniciando download...";
                await _downloadManager.DownloadVideoAsync(txtVideoUrl.Text.Trim(), _outputPath);
                if (File.Exists(_outputPath))
                {
                    lblStatus.Text = "Download foi efetuado com sucesso!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showRecortarButton", "showRecortarButton();", true);

                    btnRecortar.Style["display"] = "inline-block";
                    btnTranscrever.Style["display"] = "inline-block";
                    btnExtrairAudio.Style["display"] = "inline-block";
                }
                else
                {
                    lblStatus.Text = "Download não concluiu corretamente. Arquivo não encontrado.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Erro ao baixar vídeo: {ex.Message}";
            }
        }

        protected async void BtnRecortar_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Iniciando recorte do vídeo...";
                await _videoCutter.CutVideoAsync(_outputPath, Server.MapPath("~/Downloads/Segments"), TimeSpan.FromMinutes(1));
                lblStatus.Text = "Recorte do vídeo concluído com sucesso!";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Erro ao recortar vídeo: {ex.Message}";
            }
        }

        protected async void BtnExtrairAudio_Click(object sender, EventArgs e)
        {
            try
            {
                string downloadsPath = Server.MapPath("~/Downloads");
                string audioPath = Path.Combine(downloadsPath, $"{Path.GetFileNameWithoutExtension(_outputPath)}.mp3");

                if (File.Exists(audioPath))
                {
                    lblStatus.Text = "O áudio já existe na pasta.";
                    return;
                }

                lblStatus.Text = "Iniciando extração do áudio...";
                Debug.WriteLine("Iniciando extração do áudio...");

                string ffmpegPath = @"C:\ffmpeg\bin\ffmpeg.exe"; // Altere para o caminho real do seu ffmpeg

                AudioExtractor extractor = new AudioExtractor(ffmpegPath);
                await extractor.ExtractAudioAsync(_outputPath, audioPath);
                lblStatus.Text = "Áudio extraído do vídeo concluído com sucesso!";
                Debug.WriteLine("Áudio extraído do vídeo concluído com sucesso!");
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Erro ao extrair áudio do vídeo: {ex.Message}";
                Debug.WriteLine($"Erro ao extrair áudio do vídeo: {ex.Message}");
            }
            finally
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "hideLoading", "hideLoading();", true);
                Debug.WriteLine("Finalizando extração de áudio.");
            }
        }

        protected async void BtnTranscrever_Click(object sender, EventArgs e)
        {
            try
            {
                var videoUrl = txtVideoUrl.Text;
                var downloadDirectory = Server.MapPath("~/Downloads/");
                var sanitizedTitle = TratarTextos.SanitizeFileName(_videoTitle);
                var srtPath = Path.Combine(downloadDirectory, $"{sanitizedTitle}.srt");

                if (File.Exists(srtPath))
                {
                    lblStatus.Text = "A transcrição já existe na pasta.";
                    lblStatus.ForeColor = System.Drawing.Color.Green;
                    ScriptManager.RegisterStartupScript(this, GetType(), "showInserirLegendaButton", "showInserirLegendaButton();", true);
                    btnInserirLegenda.Style["display"] = "inline-block"; // Mostrar o botão de inserir legenda
                    return;
                }

                await _transcriptionManager.SaveTranscriptionAsSrtAsync(videoUrl, downloadDirectory);

                lblStatus.Text = "Transcrição baixada com sucesso!";
                lblStatus.ForeColor = System.Drawing.Color.Green;

                ScriptManager.RegisterStartupScript(this, GetType(), "showInserirLegendaButton", "showInserirLegendaButton();", true);

                btnInserirLegenda.Style["display"] = "inline-block"; // Mostrar o botão de inserir legenda
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Erro ao transcrever vídeo: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected async void BtnInserirLegenda_Click(object sender, EventArgs e)
        {
            try
            {
                var videoUrl = txtVideoUrl.Text;
                var downloadDirectory = Server.MapPath("~/Downloads/");
                var sanitizedTitle = TratarTextos.SanitizeFileName(_videoTitle);
                var videoFilePath = Path.Combine(downloadDirectory, $"{sanitizedTitle}.mp4");
                var subtitleFilePath = Path.Combine(downloadDirectory, $"{sanitizedTitle}.srt");
                var outputFilePathWithSubtitle = Path.Combine(downloadDirectory, $"{sanitizedTitle}_com_legenda.mp4");
                var outputFilePathBurned = Path.Combine(downloadDirectory, $"{sanitizedTitle}_com_legenda_embutida.mp4");

                if (!File.Exists(videoFilePath))
                {
                    lblStatus.Text = $"O vídeo não foi encontrado: {videoFilePath}";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                if (!File.Exists(subtitleFilePath))
                {
                    lblStatus.Text = $"A transcrição não foi encontrada: {subtitleFilePath}";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                // Initialize FFmpeg
                FFmpeg.SetExecutablesPath(@"C:\ffmpeg\bin");

                // Change current directory to the download directory
                Directory.SetCurrentDirectory(downloadDirectory);

                // Log current directory
                Debug.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");

                // Create conversion task for soft subtitles
            /*    var conversionWithSubtitle = FFmpeg.Conversions.New()
                    .AddParameter($"-i \"{sanitizedTitle}.mp4\"")
                    .AddParameter($"-i \"{sanitizedTitle}.srt\"")
                    .AddParameter("-c:v copy")
                    .AddParameter("-c:a copy")
                    .AddParameter("-c:s mov_text")
                    .SetOutput($"{sanitizedTitle}_com_legenda.mp4");

                // Log the FFmpeg command
                Debug.WriteLine($"FFmpeg Command (soft subtitles): {conversionWithSubtitle.Build()}");

                // Execute the conversion for soft subtitles
                await conversionWithSubtitle.Start();*/

                // Create conversion task for hard subtitles
                var conversionBurnedSubtitle = FFmpeg.Conversions.New()
                    .AddParameter($"-i \"{sanitizedTitle}.mp4\"")
                    .AddParameter($"-vf subtitles={sanitizedTitle}.srt") // Ensure this correctly points to the embedded subtitles
                    .AddParameter("-c:a copy")
                    .SetOutput($"{sanitizedTitle}_com_legenda.mp4");

                // Log the FFmpeg command
                Debug.WriteLine($"FFmpeg Command (burned subtitles): {conversionBurnedSubtitle.Build()}");

                // Execute the conversion for hard subtitles
                await conversionBurnedSubtitle.Start();

                lblStatus.Text = "Legenda inserida no vídeo com sucesso!";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Erro ao inserir legenda: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
                Debug.WriteLine($"Erro ao inserir legenda: {ex.Message}");
            }
        }



    }
}
