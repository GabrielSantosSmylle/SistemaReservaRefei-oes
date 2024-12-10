using Microsoft.Extensions.Configuration;

namespace ReservaRefeicao.Utils
{
    public class Configuracao
    {
        private readonly IConfiguration _configuration;

        // Injeção de dependência do IConfiguration
        public Configuracao(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ObterConfiguracao(string nomeConfiguracao)
        {
            var valor = _configuration["AppSettings:" + nomeConfiguracao];
            if (string.IsNullOrEmpty(valor))
            {
                throw new Exception("Você deve inserir a configuração \"" + nomeConfiguracao + "\" no appsettings.json!");
            }
            return valor;
        }

        public string ObterConnectionString(string nomeConnectionString)
        {
            var connectionString = _configuration.GetConnectionString(nomeConnectionString);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Você deve inserir a connectionString \"" + nomeConnectionString + "\" no appsettings.json!");
            }
            return connectionString;
        }
    }
}
