using ReservaRefeicao.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservaRefeicao.ModelView
{
    public class RefeicaoViewModel : INotifyPropertyChanged
    {
        private Color _corExibicao;
        public Refeicao Refeicao { get; set; }

        public string? CardapioFormatado
        {
            get => Refeicao.Cardapio.Replace(";", "\n").Replace(" \n", "\n").Replace("\n ", "\n");
            set
            {
                Refeicao.Cardapio = value.Replace(";", "\n").Replace(" \n", "\n").Replace("\n ", "\n");
                OnPropertyChanged();
            }
        }

        public Color CorExibicao
        {
            get => _corExibicao;
            set
            {
                _corExibicao = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
