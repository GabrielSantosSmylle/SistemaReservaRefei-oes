using IngestaoCardapio.Mail;
using IngestaoCardapio.Utils;

namespace IngestaoCardapio
{
    class Program
    {
        static void Main(string[] args)
        {
            Log log = new Log();

            new AtualizadorCardapio(log).AtualizarCardapio();
            new GestorMail(log).EnviarEmail();
        }
    }
}