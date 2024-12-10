using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ReservaRefeicao.Model;
using ReservaRefeicao.ModelView;
using ReservaRefeicao.Services;

namespace ReservaRefeicao.ViewModels
{
    public class CardapioViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly GestorCardapioService _gestorCardapioService;
        private readonly Sessao _sessaoUsuario;
        private string _nomeFuncionario;
        private DateTime _diaAtual;
        private List<Refeicao> _cardapioDaSemana;

        public Refeicao CardapioSelecionado { get; set; }

        public ICommand DiaAnteriorCommand { get; }
        public ICommand DiaProximoCommand { get; }
        public ICommand ReservarCommand { get; }
        public ICommand EntregarPavilhaoCommand { get; }
        public ICommand EncerrarSessaoCommand { get; }

        public event Action<bool> AnimarTransicaoEvent;


        private bool _podeNavegarAnterior;
        private bool _podeNavegarProximo;
        private bool _podeEncomendar;
        private bool _modoSelecaoAtivo;

        public bool ModoSelecaoAtivo
        {
            get => _modoSelecaoAtivo;
            set
            {
                _modoSelecaoAtivo = value;
                OnPropertyChanged();
            }
        }

        public bool PodeEncomendar
        {
            get => _podeEncomendar;
            set
            {
                _podeEncomendar = value;
                OnPropertyChanged();
            }
        }

        public bool PodeNavegarAnterior
        {
            get => _podeNavegarAnterior;
            set
            {
                _podeNavegarAnterior = value;
                OnPropertyChanged();
            }
        }

        public bool PodeNavegarProximo
        {
            get => _podeNavegarProximo;
            set
            {
                _podeNavegarProximo = value;
                OnPropertyChanged();
            }
        }


        public string NomeFuncionario
        {
            get => _nomeFuncionario;
            set
            {
                _nomeFuncionario = value;
                OnPropertyChanged();
            }
        }

        public string DiaAtual
        {
            get
            {
                if (_diaAtual.Date == DateTime.Today.Date)
                {
                    return $"HOJE {_diaAtual:dd/MM}"; 
                }
                else if (_diaAtual.Date == DateTime.Today.AddDays(1).Date)
                {
                    return $"AMANHÃ {_diaAtual:dd/MM}"; 
                }
                else if (_diaAtual.Date == DateTime.Today.AddDays(-1).Date)
                {
                    return $"ONTEM {_diaAtual:dd/MM}"; 
                }
                else
                {
           
                    return _diaAtual.ToString("dddd, dd 'de' MMMM 'de' yyyy");
                }
            }
        }



        // ObservableCollection para o Binding no CollectionView
        public ObservableCollection<RefeicaoViewModel> CardapiosDoDia { get; } = new ObservableCollection<RefeicaoViewModel>();
        public ObservableCollection<RefeicaoViewModel> CardapiosSelecionados { get; } = new ObservableCollection<RefeicaoViewModel>();


        public CardapioViewModel(Sessao sessaoUsuario, GestorCardapioService gestorCardapioService)
        {
            _sessaoUsuario = sessaoUsuario;
            _gestorCardapioService = gestorCardapioService;
            _sessaoUsuario.SessaoEncerrada += OnSessaoEncerrada;
            _podeEncomendar = _sessaoUsuario.Permission.CanOrder();
            DefineActualTiming();
            if(AtualizarRefeicoesFuncionario().IsCompleted)
                CarregarCardapioAsync();
            AtualizarNavegacao();
            DiaAnteriorCommand = new Command(async () => await NavegarDiaAnterior());
            DiaProximoCommand = new Command(async () => await NavegarDiaProximo());
            EncerrarSessaoCommand = new Command(async () => await _sessaoUsuario.EncerrarSessao());
        }

        private void AtivarModoSelecao()
        {
            ModoSelecaoAtivo = true;
        }

        private void DefineActualTiming()
        {
            _diaAtual = DateTime.Now;
            if (_sessaoUsuario.FuncionarioAtual.Turno == "N" && _diaAtual.Hour <= 8 && _diaAtual.Minute <= 30)
            {
                _diaAtual = _diaAtual.AddDays(1);
            }
            else if (_diaAtual.Hour >= 9)
            {
                _diaAtual = _diaAtual.AddDays(1);
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("nomeFuncionario", out var nome))
            {
                NomeFuncionario = Uri.UnescapeDataString(nome.ToString());
            }
        }

        public void OnAppearing()
        {
            _sessaoUsuario.IniciarTimer();
        }

        private async Task OnSessaoEncerradaAsync()
        {
            Dispose();
            await Shell.Current.GoToAsync("//AuthenticationView");
        }

        private void OnSessaoEncerrada()
        {
            _ = OnSessaoEncerradaAsync();
        }


        public void Dispose()
        {
            _sessaoUsuario.SessaoEncerrada -= OnSessaoEncerrada;
        }

        public Task CarregarCardapioAsync()
        {
            _cardapioDaSemana = _gestorCardapioService.ObterCardapioDaSemana();
            return AtualizarCardapios();
        }

        private async Task AtualizarCardapios()
        {
            // Limpa o ObservableCollection antes de adicionar os itens do dia
            CardapiosDoDia.Clear();

            var cardapiosDoDia = _cardapioDaSemana.FindAll(r => r.Data.Date == _diaAtual.Date);

            // Adiciona cada item individualmente para que o CollectionView seja notificado
            foreach (var refeicao in cardapiosDoDia)
            {
                var refeicaoViewModel = new RefeicaoViewModel { Refeicao = refeicao };
                refeicaoViewModel.CorExibicao = _sessaoUsuario.ReservasSemana.Any(r => r.Refeicao.CodRefeicao == refeicao.CodRefeicao)
                    ? refeicao.Nome.Contains("CAFÉ") ? Colors.Orange : Colors.LightGreen
                    : Color.FromArgb("F0F0F0");

                CardapiosDoDia.Add(refeicaoViewModel);
            }
            OnPropertyChanged(nameof(CardapiosDoDia));
        }

        private Task AtualizarRefeicoesFuncionario()
        {
            // Limpa o ObservableCollection antes de adicionar os itens do dia
            CardapiosSelecionados.Clear();

            var cardapiosSelecionados = _sessaoUsuario.ReservasSemana.FindAll(r => r.Refeicao.Data == _diaAtual.Date).Select(r => r.Refeicao);

            // Adiciona cada item individualmente para que o CollectionView seja notificado
            foreach (var refeicao in cardapiosSelecionados)
            {
                var refeicaoViewModel = new RefeicaoViewModel { Refeicao = refeicao };
                CardapiosSelecionados.Add(refeicaoViewModel);
            }
            OnPropertyChanged();
            return Task.CompletedTask;
        }

        private async Task NavegarDiaAnterior()
        {
            AnimarTransicaoEvent?.Invoke(false); // Falso = deslizar para a direita
            _diaAtual = _diaAtual.AddDays(-1);
            OnPropertyChanged(nameof(DiaAtual));
            await AtualizarCardapios();
            await AtualizarNavegacao();
        }

        private async Task NavegarDiaProximo()
        {
            AnimarTransicaoEvent?.Invoke(true); // Verdadeiro = deslizar para a esquerda
            _diaAtual = _diaAtual.AddDays(1);
            OnPropertyChanged(nameof(DiaAtual));
            await AtualizarCardapios();
            await AtualizarNavegacao();
        }



        public void Reservar()
        {
            if (CardapiosSelecionados.Count > 0)
            {
                foreach (var refeicao in CardapiosSelecionados)
                {
                    if(refeicao != null)
                    OnReservar(refeicao.Refeicao);
                }
            }
            return;
        }

        public void StartTimer()
        {
            // Inicia o timer de sessao de usuario quando a pagina aparecer
            _sessaoUsuario.IniciarTimer();
        }

        private async Task OnReservar(Refeicao refeicaoSelecionada)
        {
            var reservaExistente = _sessaoUsuario.ReservasSemana.FindAll(r => r.Refeicao.Data == _diaAtual.Date);
            var removido = false;

            if (reservaExistente.Count > 0)
            {
                foreach(var reserva in reservaExistente)
                {
                    if(reserva.CodRefeicao == refeicaoSelecionada.CodRefeicao)
                    {
                        await _gestorCardapioService.RemoverReserva(reserva);
                        removido = true;
                        break;
                    }
                }
                if (!removido)
                {

                    foreach (var reserva in reservaExistente)
                    {
                        if (reserva.CodRefeicao != refeicaoSelecionada.CodRefeicao && !refeicaoSelecionada.Tipo.Contains("CAFÉ"))
                        {
                            reserva.CodRefeicao = refeicaoSelecionada.CodRefeicao;
                            await _gestorCardapioService.AtualizarReserva(reserva);
                            break;
                        }
                        else
                        {
                            // Cria nova reserva
                            var novaReserva = new Reserva
                            {
                                Repreg = _sessaoUsuario.FuncionarioAtual.Repreg,
                                CodRefeicao = refeicaoSelecionada.CodRefeicao,
                                DataReserva = DateTime.Now
                            };
                            await _gestorCardapioService.AdicionarReserva(novaReserva);
                            break;
                        }
                    }
                }
            }
            else
            {
                // Cria nova reserva
                var novaReserva =  new Reserva
                {
                    Repreg = _sessaoUsuario.FuncionarioAtual.Repreg,
                    CodRefeicao = refeicaoSelecionada.CodRefeicao,
                    DataReserva = DateTime.Now
                };


                await _gestorCardapioService.AdicionarReserva(novaReserva);
            }
            _sessaoUsuario.AtualizarReservas( await _gestorCardapioService.ObterReservasSemanalFuncionario(_sessaoUsuario.FuncionarioAtual.Repreg));

            // Atualiza a UI
            await AtualizarRefeicoesFuncionario();
            await AtualizarCardapios();
        }

        private Task AtualizarNavegacao()
        {
            // Permitir navegação para o dia anterior se houver cardápio nesse dia
            PodeNavegarAnterior = _cardapioDaSemana.Any(r => r.Data.Date == _diaAtual.AddDays(-1).Date);

            // Permitir navegação para o próximo dia se houver cardápio nesse dia
            PodeNavegarProximo = _cardapioDaSemana.Any(r => r.Data.Date == _diaAtual.AddDays(1).Date);

            return Task.CompletedTask;
        }

    }
}
