using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservaRefeicao.Utils;
using ReservaRefeicao.Services;
using ReservaRefeicao.Model;
using ReservaRefeicao.ModelView;
using ReservaRefeicao.Views;
using ReservaRefeicao.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using ReservaRefeicao.Tests;

namespace ReservaRefeicao
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            // Carregar configurações do appsettings.json
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("ReservaRefeicao.appsettings.json");

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream) // Carrega o appsettings.json
                .Build();

            // Adicionar a configuração ao IServiceCollection
            builder.Configuration.AddConfiguration(config);

            // Registrar o DbContext com a ConnectionString do appsettings.json
            builder.Services.AddDbContext<DbContextServices>(options =>
            {
                var connectionString = config.GetConnectionString("SistemaTramontina");
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddScoped<ConnectionTest>();

            //Registra os serviços
            builder.Services.AddTransient<GestorSessaoService>();
            builder.Services.AddTransient<GestorCardapioService>();

            // Registra a SessaoUsuario
            builder.Services.AddSingleton<Sessao>();
            builder.Services.AddSingleton<IAlertService, AlertService>();

            // Registra as Views e suas ViewModels
            builder.Services.AddTransient<AuthenticationViewModel>();
            builder.Services.AddTransient<AuthenticationView>();
            builder.Services.AddTransient<CardapioViewModel>();
            builder.Services.AddTransient<CardapioView>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
