using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MCRepositoryTool.Converters
{
    /// <summary>
    /// Value converter that translates NOT null to <see cref="Visibility.Visible"/> and the opposite to <see cref="Visibility.Collapsed"/> 
    /// </summary>
    public sealed class NotNullToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            return (value != null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return (value is Visibility && (Visibility)value == Visibility.Visible) ? true : false;
        }
    }
}
