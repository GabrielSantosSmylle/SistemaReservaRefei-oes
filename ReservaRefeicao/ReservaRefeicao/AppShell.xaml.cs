using ReservaRefeicao.Views;

namespace ReservaRefeicao
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("AuthenticationView", typeof(AuthenticationView));
            Routing.RegisterRoute(nameof(CardapioView), typeof(CardapioView));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Navegar para a tela de autenticação após o Shell estar pronto
            try
            {
            await GoToAsync("AuthenticationView");
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Ignorar exceção
            }
        }

    }
}
