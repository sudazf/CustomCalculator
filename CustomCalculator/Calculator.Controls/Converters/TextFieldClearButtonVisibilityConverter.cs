using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Calculator.Controls.Converters
{
    public class TextFieldClearButtonVisibilityConverter : IMultiValueConverter
    {
        public Visibility ContentEmptyVisibility { get; set; } = Visibility.Hidden;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasClearButton = false;
            bool isContentNullOrEmpty = false;

            if (values.Length == 2 && values[0] is bool && values[1] is bool )
            {
                return Visibility.Visible;
            }

            if (!hasClearButton) // TextFieldAssist.HasClearButton
            {
                return Visibility.Collapsed;
            }

            if (isContentNullOrEmpty && values.Length > 2 && values[2] is false) // ComboBox.IsEditable
            {
                return Visibility.Collapsed;
            }

            return isContentNullOrEmpty // Hint.IsContentNullOrEmpty
                ? ContentEmptyVisibility
                : Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
