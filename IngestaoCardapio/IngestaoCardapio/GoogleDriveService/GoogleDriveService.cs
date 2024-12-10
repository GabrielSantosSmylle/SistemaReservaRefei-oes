using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using IngestaoCardapio.Config;
using IngestaoCardapio.Utils;
using System;
using System.IO;
using System.Threading;

namespace IngestaoCardapio.GoogleService
{

    public class GoogleDriveService
    {
        private Configuracao Config = Configuracao.ObterInstancia();
        private string[] Scopes = { DriveService.Scope.DriveReadonly };
        private string ApplicationName = "Drive API .NET Quickstart";

        private string pdfPath;
        private string sharedDriveId;
        private Log Log;
        public GoogleDriveService(Log _log)
        {
            Log = _log;
            sharedDriveId = Config.ObterConfiguracao("sharedDriveId");
            pdfPath = Config.ObterConfiguracao("pdfName") + ".pdf";
        }

        private DriveService GetDriveService()
        {
            try
            {
                UserCredential credential;

                string pathClient = Config.ObterConfiguracao("client");

                using (var stream = new FileStream(pathClient, FileMode.Open, FileAccess.Read))
                {
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                // Cria o serviço Drive API
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                return service;
            }
            catch (Exception ex)
            {
                Log.Error("Erro ao obter serviço do Google Drive: " + ex.Message);
                return null;
            }
        }

        public void DownloadFile()
        {
            try
            {
                var service = GetDriveService();

                // Define a query para buscar o arquivo pelo nome no Drive Compartilhado
                var request = service.Files.List();
                request.Q = $"name = '{pdfPath}' and trashed = false";
                request.Spaces = "drive";
                request.Corpora = "drive";
                request.DriveId = sharedDriveId;
                request.IncludeItemsFromAllDrives = true;
                request.SupportsAllDrives = true;

                var result = request.Execute();

                // Verifica se algum arquivo foi encontrado
                if (result.Files.Count == 0)
                {
                    Console.WriteLine("Arquivo não encontrado.");
                    return;
                }

                // Baixa o primeiro arquivo encontrado (supondo que só há um arquivo com esse nome)
                var fileId = result.Files[0].Id;

                var downloadRequest = service.Files.Get(fileId);
                var stream = new MemoryStream();

                downloadRequest.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
                {
                    if (progress.Status == Google.Apis.Download.DownloadStatus.Completed)
                    {
                        Console.WriteLine("Download concluído.");
                    }
                    else if (progress.Status == Google.Apis.Download.DownloadStatus.Failed)
                    {
                        Console.WriteLine("Download falhou.");
                    }
                };

                downloadRequest.Download(stream);

                using (var file = new FileStream(pdfPath, FileMode.Create, FileAccess.Write))
                {
                    if(Directory.Exists(pdfPath))
                        Directory.Delete(pdfPath, true);
                    stream.WriteTo(file);
                }

                Log.Info("Arquivo baixado com sucesso. Convertendo para PNG...");

                PdfToPng pdfToPng = new PdfToPng(pdfPath, Config.ObterConfiguracao("pdfName"));
                pdfToPng.Convert();

            }
            catch (Exception ex)
            {
                Log.Error("Erro ao baixar arquivo do Google Drive: " + ex.Message);
            }
        }
    }
}