using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReservaRefeicao.Services;

namespace ReservaRefeicao.Tests
{
    public class ConnectionTest
    {

        private DbContextServices _dbContext;

        public ConnectionTest(DbContextServices dbContext) => _dbContext = dbContext;

        public bool TestConnection()
        {
            try
            {
                Task.Run(async () => await _dbContext.TestConnection()).Wait();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            {
            }
        }

    }
}
