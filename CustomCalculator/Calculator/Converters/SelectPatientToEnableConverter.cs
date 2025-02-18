using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using Calculator.Model.Models;

namespace Calculator.Converters
{
    public class SelectPatientToEnableConverter : MarkupExtension, IValueConverter
    {
        private SelectPatientToEnableConverter _converter;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter = _converter ?? new SelectPatientToEnableConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Patient )
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
