<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="YoutubeDownloaderWebApp.Default" Async="true" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>M-Tube</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.8.1/font/bootstrap-icons.min.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@400;600&display=swap" rel="stylesheet"/>
    <link href="Assets/style.css" rel="stylesheet" />
    <link rel="icon" type="image/png" href="Assets/favicon2.png" />

</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="card text-center p-2">
              
                <div class="text-center mb-2">
                    <asp:Image ID="imgLogo" runat="server" ImageUrl="Assets/logo.png" CssClass="img-fluid logo" />
                </div>

                    <h1>M-Tube</h1>

                <div class="input-group mb-4">
                    <asp:TextBox ID="txtVideoUrl" runat="server" CssClass="form-control" placeholder="Insira a url do Vídeo"></asp:TextBox>
                    <div class="input-group-append">
                        <asp:LinkButton ID="btnGetInfo" runat="server" CssClass="btn btn-primary" OnClick="BtnGetInfo_Click">
                            <i class="bi bi-info-circle"></i> Pegar Informações
                        </asp:LinkButton>
                    </div>
                </div>

                <div class="alert alert-success text-center" role="alert">
                    <asp:Label ID="lblStatus" runat="server" ></asp:Label>
                </div>

            </div>
            <div class="row align-items-center">
                <div class="col-md-12 text-center mt-2">
                    <asp:Image ID="imgThumbnail" runat="server" CssClass="thumbnail mb-3" Visible="false" />
                </div>
                <div class="col-md-12 text-center mt-2">
                    <h5>Título do Vídeo:</h5>
                    <asp:Label ID="lblVideoTitle" runat="server" CssClass="text-light"></asp:Label>
                    <div class="d-flex flex-wrap justify-content-center">
                        <asp:LinkButton ID="btnDownload" runat="server" CssClass="btn btn-success ml-2 btn-custom" OnClick="BtnDownload_Click" Style="display: none;">
                            <i class="bi bi-download"></i> Baixar Vídeo
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnTranscrever" runat="server" CssClass="btn btn-success ml-2 btn-custom" OnClick="BtnTranscrever_Click" Style="display: none;">
                            <i class="bi bi-file-earmark-text"></i> Baixar Transcrição
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnRecortar" runat="server" CssClass="btn btn-warning ml-2 btn-custom" OnClick="BtnRecortar_Click" Style="display: none;">
                            <i class="bi bi-scissors"></i> Recortar Vídeo
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnExtrairAudio" runat="server" CssClass="btn btn-warning ml-2 btn-custom" OnClick="BtnExtrairAudio_Click" Style="display: none;">
                            <i class="bi bi-music-note"></i> Extrair Áudio
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnInserirLegenda" runat="server" CssClass="btn btn-info ml-2 btn-custom" OnClick="BtnInserirLegenda_Click" Style="display: none;">
                            <i class="bi bi-file-earmark-text"></i> Inserir Legenda em Video
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
            <asp:Label ID="lblTranscription" runat="server" CssClass="text-info" Text=""></asp:Label>
        </div>
    </form>
    <script src="https://code.jquery.com/jquery-3.5.1.min.js" integrity="sha256-9/aliU8dGd2tb6OSsuzixeV4y/faTqgFtohetphbbj0=" crossorigin="anonymous"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.bundle.min.js"></script>
    <script src="Assets/footer.js"></script>
</body>
</html>
