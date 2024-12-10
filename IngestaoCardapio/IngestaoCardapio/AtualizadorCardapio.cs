using IngestaoCardapio.Config;
using IngestaoCardapio.Context;
using IngestaoCardapio.Entidades;
using IngestaoCardapio.GoogleService;
using IngestaoCardapio.Utils;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using IngestaoCardapio.Mail;

namespace IngestaoCardapio
{
    public class AtualizadorCardapio
    {
        private Log log;

        private DbContextCardapio Context;

        private List<Refeicao> Refeicoes;

        private Configuracao Config = Configuracao.ObterInstancia();

        private GoogleDriveService GoogleDriveService;

        public AtualizadorCardapio(Log _log)
        {
            log = _log;
            Context = TestConnectionInstance();
            GoogleDriveService = new GoogleDriveService(log);
        }

        private DbContextCardapio TestConnectionInstance()
        {
            return new DbContextCardapio();
        }

        public void AtualizarCardapio()
        {
            try
            {
                GetCardapioFromDrive();
                if (Refeicoes.Count() > 0)
                {
                    foreach (var refeicao in Refeicoes)
                    {
                        if (Context.Refeicoes.Where(
                                    r => r.Data.Day == refeicao.Data.Day
                            && r.Data.Month == refeicao.Data.Month
                            && r.Data.Year == refeicao.Data.Year
                            && r.Tipo.Trim().ToLower() == refeicao.Tipo.Trim().ToLower()
                         ).Count() == 0)
                        {
                            Context.Refeicoes.Add(refeicao);
                            log.SucessInsertion(refeicao);
                        }
                        else
                        {
                            Refeicao refeicaoExistente = Context.Refeicoes.Where(
                                    r => r.Data.Day == refeicao.Data.Day
                                    && r.Data.Month == refeicao.Data.Month
                                    && r.Data.Year == refeicao.Data.Year
                                    && r.Nome.Trim().ToLower() == refeicao.Nome.Trim().ToLower()
                                    && r.Tipo.Trim().ToLower() == refeicao.Tipo.Trim().ToLower()
                                    ).FirstOrDefault();
                            refeicaoExistente.Cardapio = refeicao.Cardapio;
                            Context.Refeicoes.Update(refeicaoExistente);
                            log.SucessUpdate(refeicaoExistente);
                        }
                    }
                    Context.SaveChanges();
                    Context.Dispose();
                }
            }
            catch (Exception e)
            {
                if (Context != null)
                    Context.Dispose();

                if (log.Repeat == 6)
                {
                    throw new Exception("Erro na execução da atualização. Encerrando loop de tentativas - " + e.Message);
                }
                log.Repeat += 1;
                log.Error("Erro na execução da atualização: " + e.Message);
                Thread.Sleep(5000);
                log.Info("Tentando novamente...");
                new GestorMail(log).EnviarErro();
                new AtualizadorCardapio(log).AtualizarCardapio();
            }
            
        }

        private async void GetCardapioFromDrive()
        {
            GoogleDriveService.DownloadFile();
            ApiClient apiClient = new ApiClient(log);
            var Result = apiClient.Api();

            string JsonContent =  new Utils.JsonReader(Result.Result.ToString()).ReadJsonNode("content");
            string cardapio = JsonContent.ToString().Replace("\n", " ").Replace('\'', ' ');
            JObject cardapioJson = JObject.Parse(cardapio);
            var refeicoesWrapper = JsonConvert.DeserializeObject<RefeicoesWrapper>(cardapioJson.ToString());
            Refeicoes = refeicoesWrapper.Refeicoes;
        }
    }
}
