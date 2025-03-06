using Calculator.Model.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Calculator.Converters
{
    public class PatientEnableToBrushConverter : MarkupExtension, IValueConverter
    {
        private PatientEnableToBrushConverter _converter;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter = _converter ?? new PatientEnableToBrushConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEnable)
            {
                if (!isEnable)
                {
                    return Brushes.DarkGray;
                }

                return Brushes.Black;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
