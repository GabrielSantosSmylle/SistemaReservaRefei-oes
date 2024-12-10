using Microsoft.EntityFrameworkCore;
using ReservaRefeicao.Model;
using ReservaRefeicao.Services;
using ReservaRefeicao.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReservaRefeicao.ModelView
{
    public class AuthenticationViewModel : ViewModelBase
    {
        private decimal? _codigoFuncionario;
        public decimal? CodigoFuncionario
        {
            get => _codigoFuncionario;
            set => SetProperty(ref _codigoFuncionario, value);
        }

        public string CurrentDate { get; set; } = DateTime.Now.ToString("D");

        public List<RefeicaoViewModel> CardapioDoDia
        {
            get => _cardapioDoDia;
            set
            {
                _cardapioDoDia = value;
                OnPropertyChanged();
            }
        }
        // Propriedade que controla se a autenticação está em andamento
        private bool _isAuthenticating;
        public bool IsAuthenticating
        {
            get => _isAuthenticating;
            set => SetProperty(ref _isAuthenticating, value);
        }

        private readonly IAlertService _alertService;
        private readonly GestorSessaoService _gestorDeSessao;
        private readonly GestorCardapioService _gestorCardapio;
        private readonly Sessao _sessaoUsuario;
        private List<RefeicaoViewModel> _cardapioDoDia;

        public ICommand AutenticarCommand { get; }

        public AuthenticationViewModel(IAlertService alertService,GestorSessaoService gestorDeReserva, GestorCardapioService gestoCardapioService,Sessao sessaoUsuario)
        {
            _alertService = alertService;
            _gestorDeSessao = gestorDeReserva;
            _sessaoUsuario = sessaoUsuario;
            _gestorCardapio = gestoCardapioService;
            _codigoFuncionario = null;
            CarregarCardapioDoDia();
            AutenticarCommand = new Command(async () => await Autenticar());
        }

        private async Task CarregarCardapioDoDia()
        {
            var Cardapios = await _gestorCardapio.ObterCardapioDoDia();
            CardapioDoDia = Cardapios.Select(c => new RefeicaoViewModel { Refeicao = c }).ToList();

            if (CardapioDoDia.Count == 0)
                    await _alertService.DisplayAlertAsync("Erro ao carregar cardápio", $"Nenhum cardápio encontrado para {DateTime.Today.ToString("D")}", "OK");
        }

        public async Task Autenticar()
        {
            // Lógica de autenticação, usando _gestorDeReserva
            
            if (CodigoFuncionario != null)
            {
                decimal Codigo = (decimal)CodigoFuncionario;
                Funcionario funcionario;
                if (Codigo.ToString().Length <= 4)
                {
                    funcionario = await _gestorDeSessao.ObterFuncionarioPorCodigo((int)Codigo);
                }
                else
                {
                    funcionario = await _gestorDeSessao.ObterFuncionarioPorCartao(Codigo);

                }
                    

                if (funcionario != null)
                {
                    var reservas = await _gestorCardapio.ObterReservasSemanalFuncionario(funcionario.Repreg);
                    _sessaoUsuario.IniciarSessao(funcionario,reservas);
                    if (Shell.Current != null)
                       await Shell.Current.GoToAsync($"{nameof(CardapioView)}?nomeFuncionario={Uri.EscapeDataString(funcionario.Nome)}");
                            
                }
                else
                {   
                    _codigoFuncionario = null;
                    OnPropertyChanged();
                    await _alertService.DisplayAlertAsync("Autenticação", "Funcionario Não Encontrado", "OK");
                }
            }
        }

        public async Task Limpar()
        {
            CodigoFuncionario = null;
            await _sessaoUsuario.EncerrarSessao();
        }
    }
}
