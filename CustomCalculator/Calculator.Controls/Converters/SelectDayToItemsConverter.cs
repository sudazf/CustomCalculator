using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Calculator.Model.Models;

namespace Calculator.Controls.Converters
{
    public class SelectDayToItemsConverter : MarkupExtension, IValueConverter
    {
        private static SelectDayToItemsConverter _converter;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new SelectDayToItemsConverter());
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DailyInfo dailyInfo)
            {
                return dailyInfo.Variables;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class SelectDayToTextConverter : MarkupExtension, IValueConverter
    {
        private static SelectDayToTextConverter _converter;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new SelectDayToTextConverter());
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DailyInfo dailyInfo)
            {
                var items = dailyInfo.SelectedVariable.FollowVariables;
                return string.Join(",", items);
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}
