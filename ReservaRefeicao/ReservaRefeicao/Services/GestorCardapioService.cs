using Microsoft.EntityFrameworkCore;
using ReservaRefeicao.Model;
using ReservaRefeicao.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservaRefeicao.Services
{
    public class GestorCardapioService
    {
        private readonly DbContextServices _dbContext;
        private DateHelper _dateHelper;

        public GestorCardapioService(DbContextServices dbContext)
        {
            _dbContext = dbContext;
            _dateHelper = new DateHelper();

        }

        public List<Refeicao> ObterCardapioDaSemana()
        {
            DateTime InitWeek = _dateHelper.GetFirstDayOfWeek(DateTime.Now);
            DateTime EndWeek = _dateHelper.GetLastDayOfWeek(DateTime.Now);

            return _dbContext.refeicaos
                .Where(r => r.Data >= InitWeek && r.Data <= EndWeek)
                .ToList();
        }

        public async Task<List<Refeicao>> ObterCardapioDoDia()
        {
            _dbContext.CheckConnection();
            return await _dbContext.refeicaos
                .Where(r => r.Data == DateTime.Today)
                .ToListAsync();
        }

        public async Task AtualizarReserva(Reserva reservaExistente)
        {
            _dbContext.reservas.Update(reservaExistente);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AdicionarReserva(Reserva reserva)
        {
            _dbContext.reservas.Add(reserva);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoverReserva(Reserva reserva)
        {
            _dbContext.reservas.Remove(reserva);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Reserva>> ObterReservasSemanalFuncionario(int codigo)
        {
            var InitWeek = _dateHelper.GetFirstDayOfWeek(DateTime.Now);
            var EndWeek = _dateHelper.GetLastDayOfWeek(DateTime.Now);

            return await _dbContext.reservas
                .Include(r => r.Refeicao)
                .Where(r => r.Repreg == codigo
                    && r.DataReserva >= InitWeek
                    && r.DataReserva <= EndWeek)
                .ToListAsync();
        }
    }
}
