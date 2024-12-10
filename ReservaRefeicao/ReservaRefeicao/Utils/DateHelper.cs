
namespace ReservaRefeicao.Utils
{
    public class DateHelper
    {
        private static DateHelper _instance = null;

        public DateTime GetFirstDayOfWeek(DateTime date)
        {
            // Calcula o deslocamento para a segunda-feira
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date; // Retorna o primeiro dia (segunda-feira)
        }

        public DateTime GetLastDayOfWeek(DateTime date)
        {
            // Calcula o último dia (domingo) a partir da segunda-feira
            return GetFirstDayOfWeek(date).AddDays(6); // Adiciona 6 dias para chegar no domingo
        }

        public static DateHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DateHelper();
                return _instance;
            }
        }

    }
}