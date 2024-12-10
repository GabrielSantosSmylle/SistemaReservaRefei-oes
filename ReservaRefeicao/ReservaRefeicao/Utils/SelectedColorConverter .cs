using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.Linq;
using ReservaRefeicao.Model;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Query;

namespace ReservaRefeicao.Utils
{
    public class SelectedColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var refeicao = value as Refeicao;
            var selectedItems = parameter as ObservableCollection<Refeicao>;

            if (refeicao == null || selectedItems == null)
                return Colors.Transparent;

            // Se "CAFÉ" está selecionado, use laranja
            if (refeicao.Nome.Contains("CAFÉ") && selectedItems.Contains(refeicao))
            {
                return Colors.Orange;
            }
            // Para outros itens selecionados, use verde
            else if (selectedItems.Contains(refeicao))
            {
                return Colors.Green;
            }

            // Não selecionado
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Colors.Transparent;
        }
    }
}
