using System.CodeDom;
using System.Windows;
using System.Windows.Controls;
using Calculator.ViewModel.ViewModels.Applications;
using Calculator.ViewModel.ViewModels.Patients;

namespace Calculator.Selectors
{
    public class PatientDialogContentSelector : DataTemplateSelector
    {
        public DataTemplate AddPatientDataTemplate { get; set; }
        public DataTemplate EditPatientDataTemplate { get; set; }
        public DataTemplate MessageDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is AddPatientViewModel)
            {
                return AddPatientDataTemplate;
            }
            else if (item is EditPatientViewModel)
            {
                return EditPatientDataTemplate;
            }
            else if (item is MessageViewModel)
            {
                return MessageDataTemplate;
            }

            return base.SelectTemplate(item,container);
        }
    }
}
