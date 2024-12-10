using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace ReservaRefeicao.ModelView
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        // Evento que é disparado sempre que uma propriedade é alterada
        public event PropertyChangedEventHandler PropertyChanged;

        // Método para notificar quando uma propriedade é alterada
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Método utilitário para atualizar o valor da propriedade e disparar o evento PropertyChanged
        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // Propriedade para indicar que uma operação está em andamento (usada para mostrar feedback ao usuário, como um "carregando")
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        // Propriedade que pode ser usada para exibir mensagens de erro
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
    }
}
