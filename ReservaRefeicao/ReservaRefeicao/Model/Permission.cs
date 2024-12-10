using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservaRefeicao.Model
{
    public class Permission
    {

        private Dictionary<string, bool> _permissions = new Dictionary<string, bool>
        {
            {"Encomendar", false },
            { "PavilhaoNoturno", false },
            { "PavilhaoDiurno", false }

        };

        public Permission(Funcionario func)
        {
            DefinePermission(func);
        }

        private void DefinePermission(Funcionario func)
        {
            if(func.Secao.CodPredio == 9)
                _permissions["Encomendar"] = true;
            if(func.Turno == "N")
                _permissions["PavilhaoNoturno"] = true;
            else
                _permissions["PavilhaoDiurno"] = true;
        }


        public bool CanOrder()
        {
            return _permissions["Encomendar"];
        }

        public bool CanAccessNight()
        {
            return _permissions["PavilhaoNoturno"];
        }

        public bool CanAccessDay()
        {
            return _permissions["PavilhaoDiurno"];
        }
    }
}
