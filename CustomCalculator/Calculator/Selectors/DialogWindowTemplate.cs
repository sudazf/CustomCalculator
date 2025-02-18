using Calculator.ViewModel.ViewModels.Patients;
using System.Windows.Controls;
using System.Windows;

namespace Calculator.Selectors
{
    public class DialogWindowTemplate : DataTemplateSelector
    {
        public DataTemplate PatientVariablesSettingDataTemplate { get; set; }
        public DataTemplate PatientVariablesCalculationDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is PatientVariablesSettingViewModel)
            {
                return PatientVariablesSettingDataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
