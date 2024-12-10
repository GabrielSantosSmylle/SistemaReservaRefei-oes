using IngestaoCardapio.Config;
using IngestaoCardapio.Entidades;
using IngestaoCardapio.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IngestaoCardapio.Mail
{
    public class GestorMail
    {
        private Log _log;

        private MailMessage mail = new MailMessage();

        private SmtpClient smtp = new SmtpClient(Configuracao.ObterInstancia().ObterConfiguracao("smtpServer"));

        private MensagemEmail Mensagem;

        public GestorMail(Log log) => _log = log;

        public void EnviarEmail()
        {
                _log.Info("Notificando Atualização do cardápio");
                email();
        }

        public void EnviarErro()
        {

           _log.Info("Notificando Erro na Atualização do cardápio");
            emailError();
        }

        private void AdicionarAoDicionario(Dictionary<string, List<string>> tabela, string cabecalho, string informacao)
        {
            if (tabela.ContainsKey(cabecalho))
            {
                tabela[cabecalho].Add(informacao);
            }
            else
            {
                List<string> novo = new List<string>();
                novo.Add(informacao);
                tabela.Add(cabecalho, novo);
            }
        }

        private void emailError()
        {
            if(_log.Exceptions.Count > 0)
            {
                Mensagem = new MensagemEmail("Erro na Atualização", "Revise os Logs para mais informações.");
                Dictionary<string, List<string>> ex= new Dictionary<string, List<string>>();
                foreach (var exception in _log.Exceptions)
                {
                    AdicionarAoDicionario(ex, "Erro", exception);
                }
                Mensagem.montarTabela("Erros Encontrados", ex);
                AnexarArquivodeLog();
                enviarEmail("Erro na Atualização") ;
            }
        }

        private void email()
        {
            if (_log.Exceptions.Count > 0)
            { Mensagem = new MensagemEmail("Ocorreu erros durante  a atualização do Cardápio", "Revise os Logs para mais informações!"); AnexarArquivodeLog(); }
            else
                Mensagem = new MensagemEmail("Atualização de Cardápio com sucesso", "Atualização do cárdapio semanal ocorreu com Sucesso! Segue informações atualizadas.");

            Dictionary<string, List<string>> tabela = new Dictionary<string, List<string>>();

            List<Refeicao> insertionSucess = _log.GetSucessInsertion();

            List<Refeicao> updateSucess = _log.GetSucessUpdate();

            if (insertionSucess != null)
            {
                foreach (var refeicao in insertionSucess)
                {
                    AdicionarAoDicionario(tabela, "Dia da Semana", refeicao.Data.ToString("dddd", new CultureInfo("pt-BR")));
                    AdicionarAoDicionario(tabela, "Data", refeicao.Data.ToString("M"));
                    AdicionarAoDicionario(tabela, "Nome", refeicao.Nome);
                    AdicionarAoDicionario(tabela, "Tipo", refeicao.Tipo);
                    AdicionarAoDicionario(tabela, "Cardapio", refeicao.Cardapio.Replace(",", "<br>"));
                }
                Mensagem.montarTabela("Tabelas Adicionadas com Sucesso", tabela);
            }

            if (updateSucess != null)
            {
                tabela = new Dictionary<string, List<string>>();
                foreach (var refeicao in updateSucess)
                {
                    AdicionarAoDicionario(tabela, "Dia da Semana", refeicao.Data.ToString("dddd", new CultureInfo("pt-BR")));
                    AdicionarAoDicionario(tabela, "Data", refeicao.Data.ToString("M"));
                    AdicionarAoDicionario(tabela, "Nome", refeicao.Nome);
                    AdicionarAoDicionario(tabela, "Tipo", refeicao.Tipo);
                    AdicionarAoDicionario(tabela, "Cardapio", refeicao.Cardapio.Replace(",", "<br>"));
                }
                Mensagem.montarTabela("Tabelas Atualizadas com Sucesso", tabela);
            }
            enviarEmail("Atualização do Cardápio");
        }

        private void AnexarArquivodeLog()
        {
            _log.close();
            Attachment anexo = new Attachment(_log.filePath);
            this.mail.Attachments.Add(anexo);
        }

        private void enviarEmail(string ass)
        {
            this.mail.From = new MailAddress(Configuracao.ObterInstancia().ObterConfiguracao("MailRemetente"), "Atualizador SistemaTramontina");
            this.mail.To.Add(Configuracao.ObterInstancia().ObterConfiguracao("MailDestinatario"));
            this.mail.Subject = ass;
            this.mail.IsBodyHtml = true;
            this.mail.Body = Mensagem.MontarEmail();
            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }



    }
}
