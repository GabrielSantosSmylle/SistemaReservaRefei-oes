using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservaRefeicao.Services
{
    public class AlertService : IAlertService
    {
        public async Task DisplayAlertAsync(string title, string message, string cancel)
        {
            var currentPage = Application.Current.MainPage;
            if(currentPage != null)
                await currentPage.DisplayAlert(title, message, cancel);
        }
    }
}
