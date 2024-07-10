using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            string sanitizedTitle = SanitizeFileName(_videoTitle);
            _outputPath = Server.MapPath($"~/Downloads/{sanitizedTitle}.mp4");

            try
            {
                lblStatus.Text = "Iniciando download...";
                await _downloadManager.DownloadVideoAsync(txtVideoUrl.Text.Trim(), _outputPath);
                if (File.Exists(_outputPath))
                {
                    lblStatus.Text = "Download foi efetuado com sucesso!";
                    ScriptManager.RegisterStartupScript(this, GetType(), "showRecortarButton", "showRecortarButton();", true);

                    btnRecortar.Style["display"] = "inline-block"; // Mostrar o botão de recorte
                    btnTranscrever.Style["display"] = "inline-block"; // Mostrar o botão de transcrição
                    btnExtrairAudio.Style["display"] = "inline-block"; // Mostrar o botão de extração de áudio

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
                lblStatus.Text = "Iniciando extração do áudio...";
                Debug.WriteLine("Iniciando extração do áudio...");

                string downloadsPath = Server.MapPath("~/Downloads");
                string audioPath = Path.Combine(downloadsPath, $"{Path.GetFileNameWithoutExtension(_outputPath)}.mp3");
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
                var sanitizedTitle = SanitizeFileName(_videoTitle);
                var videoFilePath = Path.Combine(downloadDirectory, $"{sanitizedTitle}.mp4");
                var subtitleFilePath = Path.Combine(downloadDirectory, $"{sanitizedTitle}.srt");

                if (!File.Exists(videoFilePath) || !File.Exists(subtitleFilePath))
                {
                    lblStatus.Text = "O vídeo ou a transcrição não foram encontrados.";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                var outputFilePath = Path.Combine(downloadDirectory, $"{sanitizedTitle}_com_legenda.mp4");
                // var ffmpegPath = Server.MapPath("~/ffmpeg/ffmpeg.exe"); // Caminho para o executável ffmpeg
                // _transcriptionManager.EmbedSubtitles(videoFilePath, subtitleFilePath, outputFilePath, ffmpegPath);

                var conversion = await FFmpeg.Conversions.FromSnippet.AddSubtitle(videoFilePath, subtitleFilePath, outputFilePath);
                await conversion.Start();

                lblStatus.Text = "Legenda inserida no vídeo com sucesso!";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Erro ao inserir legenda: {ex.Message}";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private string SanitizeFileName(string fileName)
        {
            // Remove caracteres inválidos do nome do arquivo
            string invalidChars = new string(Path.GetInvalidFileNameChars());
            string regexPattern = $"[{Regex.Escape(invalidChars)}]";
            return Regex.Replace(fileName, regexPattern, "_");
        }
    }
}
