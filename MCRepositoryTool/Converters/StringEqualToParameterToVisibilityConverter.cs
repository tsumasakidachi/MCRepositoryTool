using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MCRepositoryTool.Converters
{
    public sealed class StringEqualToParameterToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            bool flag = false;

            string x = value as string;
            string y = parameter as string;

            if (String.IsNullOrWhiteSpace(x) || String.IsNullOrWhiteSpace(y))
            {
                flag = false;
            }
            else
            {
                flag = (string)value == (string)parameter;
            }

            return flag ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return value is Visibility && (Visibility)value != Visibility.Visible;
        }
    }
}
