using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MCRepositoryTool.Converters
{
    /// <summary>
    /// Value converter that translates null to <see cref="Visibility.Visible"/> and the opposite to <see cref="Visibility.Collapsed"/> 
    /// </summary>
    public sealed class NullOrEmptyToCollapseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            string text = value.ToString();

            return String.IsNullOrEmpty(text) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return (value is Visibility && (Visibility)value == Visibility.Visible) ? false : true;
        }
    }
}
