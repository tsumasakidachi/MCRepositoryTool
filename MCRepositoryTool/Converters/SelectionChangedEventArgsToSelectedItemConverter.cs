using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MCRepositoryTool.Converters
{
    public sealed class SelectionChangedEventArgsToSelectedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            SelectionChangedEventArgs ea = (SelectionChangedEventArgs)value;

            var addedItems = ea.AddedItems;

            return addedItems;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return null;
        }
    }
}
