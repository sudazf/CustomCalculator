using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Media;

namespace Calculator.Controls.Converters
{
    public class VariableRangeCompareConverter : MarkupExtension, IValueConverter, IMultiValueConverter
    {
        private static VariableRangeCompareConverter _converter;

        public static VariableRangeCompareConverter Converter => _converter ?? (_converter = new VariableRangeCompareConverter());

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new VariableRangeCompareConverter());
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length == 3)
            {
                if (values[0] != null && values[1] != null && values[2] != null)
                {
                    if (double.TryParse(values[0].ToString(), out double current) &&
                        double.TryParse(values[1].ToString(), out double min) &&
                        double.TryParse(values[2].ToString(), out double max))
                    {
                        if (current >= min && current <= max)
                        {
                            return Brushes.Black;
                        }

                        return Brushes.Red;
                    }
                }
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(new[] { value }, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
