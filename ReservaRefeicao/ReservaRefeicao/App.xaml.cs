using Microsoft.Extensions.DependencyInjection;
using ReservaRefeicao.Model;
using ReservaRefeicao.ModelView;
using ReservaRefeicao.Views;

namespace ReservaRefeicao
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
