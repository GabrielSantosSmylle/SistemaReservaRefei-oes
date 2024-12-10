using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservaRefeicao.Services
{
    public interface IAlertService
    {
        Task DisplayAlertAsync(string title, string message, string cancel);
    }
}
