using System.Windows.Controls;
using System.Windows;
using Calculator.ViewModel.ViewModels.Applications;
using Calculator.ViewModel.ViewModels.Patients;

namespace Calculator.Selectors
{
    public class DialogWindowTemplate : DataTemplateSelector
    {
        public DataTemplate ConfirmDataTemplate { get; set; }
        public DataTemplate GetTemplateNameDataTemplate { get; set; }
        public DataTemplate SelectTemplateNameDataTemplate { get; set; }
        public DataTemplate FollowVariablesSettingDataTemplate { get; set; }
        public DataTemplate VariableExpressionSettingDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ConfirmViewModel)
            {
                return ConfirmDataTemplate;
            }
            else if (item is GetTemplateNameViewModel)
            {
                return GetTemplateNameDataTemplate;
            }
            else if (item is SelectTemplateNameViewModel)
            {
                return SelectTemplateNameDataTemplate;
            }
            else if (item is FollowVariablesSettingViewModel)
            {
                return FollowVariablesSettingDataTemplate;
            }
            else if (item is VariableExpressionViewModel)
            {
                return VariableExpressionSettingDataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
