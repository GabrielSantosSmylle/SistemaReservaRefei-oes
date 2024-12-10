using ReservaRefeicao.ModelView;
using ReservaRefeicao.ViewModels;

namespace ReservaRefeicao.Views
{
    public partial class CardapioView : ContentPage
    {
        public CardapioView(CardapioViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            viewModel.AnimarTransicaoEvent += async (paraDireita) => await AnimarTransicao(paraDireita);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Chama a função da ViewModel quando a página aparecer
            if (BindingContext is CardapioViewModel viewModel)
            {
                viewModel.StartTimer();
            }
        }

        private async Task AnimarTransicao(bool paraDireita)
        {
            // Define a direção do deslocamento
            double startTranslation = paraDireita ? 1000 : -1000;
            double endTranslation = 0;

            // Realiza o deslocamento inicial fora da tela
            CardapioCollectionView.TranslationX = startTranslation;
            CardapioCollectionView.Opacity = 0;

            // Anima o deslocamento para o centro da tela com suavidade
            await Task.WhenAll(
                CardapioCollectionView.TranslateTo(endTranslation, 0, 400, Easing.CubicOut),
                CardapioCollectionView.FadeTo(1, 400, Easing.CubicIn)
            );
        }

    }
}
