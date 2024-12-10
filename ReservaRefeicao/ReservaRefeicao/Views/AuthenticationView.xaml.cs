using ReservaRefeicao.Model;
using ReservaRefeicao.ModelView;
using ReservaRefeicao.Services;

namespace ReservaRefeicao.Views;

public partial class AuthenticationView : ContentPage
{
    private readonly AuthenticationViewModel _viewModel;

    // Construtor que recebe dependências injetadas
    public AuthenticationView(AuthenticationViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        codigoFuncionarioEntry.Focus();
        _viewModel.Limpar(); // Limpa a sessão do usuário
    }

    // Evento acionado quando o usuário pressiona Enter
    private async void OnCompleted(object sender, EventArgs e)
    {
        // Chama o comando de autenticação
        await _viewModel.Autenticar();
    }
}