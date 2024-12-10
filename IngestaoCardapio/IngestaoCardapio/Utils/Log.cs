using IngestaoCardapio.Config;
using IngestaoCardapio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestaoCardapio.Utils
{
    public class Log
    {
        public string filePath { get; }

        private string cabecalhoError = "[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] - ( FAILED ): ";

        private string cabecalhoInfo = "[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] - ( INFO ): ";

        private string cabecalhoSucess = "[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] - (SUCESS): ";

        public List<string> Exceptions = new List<string>();

        private Dictionary<string, List<Refeicao>> Refeicoes = new Dictionary<string, List<Refeicao>>();
        
        private StreamWriter file;

        public int Repeat { get; set; }

        public Log()
        {
            this.filePath = Configuracao.ObterInstancia().ObterConfiguracao("LogPath") + "log- " + DateTime.Today.ToString("yyyy-MM-dd") + ".log";
            try
            {
                file = new StreamWriter(filePath, true);
            }
            catch
            {
                this.filePath = "./log- " + DateTime.Today.ToString("yyyy-MM-dd") + ".log";
                file = new StreamWriter(filePath, true);
                this.Error("Erro ao criar arquivo de log, criando arquivo na pasta do executável");
            }
            Repeat = 0;
        }

        public void SucessInsertion(Refeicao refeicao)
        {
            if (!this.Refeicoes.ContainsKey("Insertion"))
            {
                this.Refeicoes.Add("Insertion", new List<Refeicao>());
            }
            else
            {
                this.Refeicoes["Insertion"].Add(refeicao);
            }
        }

        public void SucessUpdate(Refeicao refeicao)
        {
            if (!this.Refeicoes.ContainsKey("Update"))
            {
                this.Refeicoes.Add("Update", new List<Refeicao>());
            }
            else
            {
                this.Refeicoes["Update"].Add(refeicao);
            }
        }

        public List<Refeicao> GetSucessInsertion()
        {
            return this.Refeicoes.ContainsKey("Insertion") ? this.Refeicoes["Insertion"] : null;
        }

        public List<Refeicao> GetSucessUpdate()
        {
            return this.Refeicoes.ContainsKey("Update") ? this.Refeicoes["Update"] : null;
        }

        public void Sucessul(string message)
        {
            this.write(cabecalhoSucess+message);
        }

        public void Error(string message)
        {
            Exceptions.Add(message);
            this.write(cabecalhoError + message);
        }

        public void Info(string message)
        {
            this.write(cabecalhoInfo + message);
        }

        private void write(string message)
        {
            Console.WriteLine(message);
            file.WriteLine(cabecalhoInfo + message);

        }

        public void close()
        {
            file.Close();
        }
    }
}
