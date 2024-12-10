namespace IngestaoCardapio.Mail
{
    public class MensagemEmail
    {
        public string Assunto { get; set; }
        public string Corpo { get; set; }
        public string Aviso { get; set; }

        public MensagemEmail(string assunto,  string aviso) => (Assunto,  Aviso) = (assunto,  aviso);

        public MensagemEmail(string assunto) => Assunto = assunto;
        private string Cabecalho()
        {
            return @"<head>
                        <title>" + Assunto + @"</title>       
                        <style type=""text/css"">
                        *
                        {
                            font-family: Calibri, Verdana, Arial;
                        }
                        h1
                        {
                            color:#FFFFFF;
                            text-align: center;
                            font-size: 30px;
                            width: 100%;
                            background-color:#04549C;
                            padding: 10px 0 10px 0;
                        }
                        p
                        {
                            font-size: 16px;
                        }
                        hr
                        {
                            height: 1px;
                        }
                        .rodape
                        {
                            font-size: 12px;
                            color: #888;
                            margin-top: 0;
                            text-align: right;
                        }
                        .tabelaInformacoes, .tabelaInformacoes th, .tabelaInformacoes td
                        {
                           border-collapse: collapse;
                           border: 1px solid #000000; 
                            margin: 0 auto;
                            padding: 5px 10px;
                            text-align: center;
                         }
                         .tabelaInformacoes th
                         {
                             background-color: #CCC;
                             border: 1px solid #000000; 
                         }
                        caption{
                             background-color: #04549C;
                             color: #FFFFFF;
                             padding: 15px 10px;
                             font-size: 20;
                             border: 1px solid #000000; 
                             white-space: nowrap;
                         }
                         .Alerta{
                            background-color:gold;
                            color: #000000;
                         }

                         </style></head>";
        }

        private string GerarBody()
        {
            return $@"<body>
                        <h1>{Assunto} </h1>
                        <h2>Olá <b>Desenvolvimento</b></h2>
                        {(!String.IsNullOrEmpty(Aviso) ? $"<p>{Aviso}</p>" :"")}
                        {Corpo}
                        <hr /><br />
                        <p class=""rodape"">E-mail gerado automaticamente por<b> Atualizador Sistema TramontinaV2 </b>!</p></body>";
        }

        public void montarTabela(string TituloDaTabela, Dictionary<string, List<string>> IndiceInformacao)
        {
            this.montarTabela(TituloDaTabela, IndiceInformacao, null);
        }

        public void montarTabela(string TituloDaTabela, Dictionary<string, List<string>> IndiceInformacao, string classe)
        {
            List<string> cabecalho = new List<string>();
            List<List<string>> Informacoes = new List<List<string>>();


            foreach (var indice in IndiceInformacao)
            {
                cabecalho.Add(indice.Key);
                Informacoes.Add(indice.Value);
            }

            this.Corpo += $@" <table class=""tabelaInformacoes"">
                            <caption {classe}>{TituloDaTabela}</caption>"
             + MontarCabecalho(cabecalho) + MontarDados(Informacoes) +
             @"</table><br /><br /><br /><br /><br />";
        }

        private string MontarDados(List<List<string>> Colunas)
        {
            string dadosTB = null;
            int numeroLinhas = Colunas[0].Count();
            int aux = 0;
            while (aux < numeroLinhas)
            {
                dadosTB += "<tr>";

                for (int j = 0; j < Colunas.Count(); j++)
                {
                    dadosTB += $"<td>{Colunas[j][aux]}</td>";
                }

                dadosTB += "</tr>";
                aux++;
            }
            return dadosTB;
        }

        private string MontarCabecalho(List<string> cabecalho)
        {
            string cabecalhoTB = "<tr>";

            foreach (var cab in cabecalho)
            {
                cabecalhoTB += @"<th>" + cab + @"</th>";
            }
            cabecalhoTB += "</tr>";

            return cabecalhoTB;
        }

        public string MontarEmail()
        {
            if (this.Corpo == null || this.Corpo== "")
                throw new Exception("A mensagem de Email está vazia");
            return "<html>" + Cabecalho() + GerarBody() + "</html>";
        }
    }
}