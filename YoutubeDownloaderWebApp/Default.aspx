<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="YoutubeDownloaderWebApp.Default" Async="true" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>M-Tube</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <!-- Bootstrap CSS -->
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Bootstrap Icons -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.8.1/font/bootstrap-icons.min.css" rel="stylesheet" />
    <style>
       body {
            min-height: 90vh;
        }
        .thumbnail {
            width: 100%;
            max-width: 300px;
            height: auto;
        }
        .video-details-container {
            display: flex;
            align-items: center;
        }
        .thumbnail-container {
            margin-right: 10px; /* Adjust margin as needed */
        }

    </style>
</head>
<body class="bg-dark d-flex justify-content-center align-items-center rounded">
    <form id="form1" runat="server">
        <div class="container">

            <div class="text-center bg-light p-4">
                <h2>M-Tube</h2>
                <div class="input-group mb-3">
                    <asp:TextBox ID="txtVideoUrl" runat="server" CssClass="form-control" placeholder="Insira a url do Vídeo"></asp:TextBox>                 
                    <asp:LinkButton ID="btnGetInfo" runat="server" CssClass="btn btn-primary" OnClick="BtnGetInfo_Click">
                        <i class="bi bi-info-circle"></i> Pegar Informações
                    </asp:LinkButton>
                </div>

                <asp:Label ID="lblStatus" runat="server" ForeColor="Red"></asp:Label>
            </div>

 
           <div class="bg-secondary p-2 video-details-container">
            <div class="thumbnail-container">
                <asp:Image ID="imgThumbnail" runat="server" CssClass="thumbnail mb-3" Visible="false" />
            </div>
            <div class="video-details text-center">
                <asp:Label ID="lblVideoTitle" runat="server" CssClass="text-light"></asp:Label>
                <div class="input-group mt-2">
                    <asp:LinkButton ID="btnDownload" runat="server" CssClass="btn btn-success ml-2" OnClick="BtnDownload_Click" style="display: none;">
                        <i class="bi bi-download"></i> Baixar Vídeo
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnRecortar" runat="server" CssClass="btn btn-warning ml-2" OnClick="BtnRecortar_Click" style="display: none;">
                        <i class="bi bi-scissors"></i> Recortar Vídeo
                    </asp:LinkButton>
                      <asp:LinkButton ID="btnExtrairAudio" runat="server" CssClass="btn btn-warning ml-2" OnClick="BtnExtrairAudio_Click" style="display: none;">
                          <i class="bi bi-music"></i> Extrair Áudio
                      </asp:LinkButton>
                    <asp:LinkButton ID="btnTranscrever" runat="server" CssClass="btn btn-info ml-2" OnClick="BtnTranscrever_Click" style="display: none;">
                        <i class="bi bi-file-earmark-text"></i> Baixar Transcrição
                    </asp:LinkButton>
                       <asp:LinkButton ID="btnInserirLegenda" runat="server" CssClass="btn btn-info ml-2" OnClick="BtnInserirLegenda_Click" style="display: none;">
                        <i class="bi bi-file-earmark-text"></i> Inserir Legenda em Video
                    </asp:LinkButton>
                </div>

            </div>
        </div>




                <asp:Label ID="lblTranscription" runat="server" CssClass="text-info" Text=""></asp:Label>
            </div>
        
    </form>
    <script src="https://code.jquery.com/jquery-3.7.1.slim.min.js" integrity="sha256-kmHvs0B+OpCW5GVHUNjv9rOmY0IvSIRcf7zGUDTDQM8=" crossorigin="anonymous"></script>
    <!-- Bootstrap JS (optional) -->
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js"></script>
    <script>
        function showDownloadButton() {
            document.getElementById('<%= btnDownload.ClientID %>').style.display = 'inline-block';
        }

        function showRecortarButton() {
            document.getElementById('<%= btnRecortar.ClientID %>').style.display = 'inline-block';
        }

        function showExtrairAudioButton() {
            document.getElementById('<%= btnExtrairAudio.ClientID %>').style.display = 'inline-block';
        }


        function showTranscreverButton() {
            document.getElementById('<%= btnTranscrever.ClientID %>').style.display = 'inline-block';
        }

        function showInserirLegendaButton() {
            document.getElementById('<%= btnInserirLegenda.ClientID %>').style.display = 'inline-block';
         }
    </script>
</body>
</html>
