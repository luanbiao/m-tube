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