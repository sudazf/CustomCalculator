using System.Windows;
using System.Windows.Controls;
using Calculator.ViewModel.ViewModels.Patients;

namespace Calculator.Selectors
{
    public class PatientDialogContentSelector : DataTemplateSelector
    {
        public DataTemplate AddPatientDataTemplate { get; set; }
        public DataTemplate EditPatientDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is AddPatientViewModel)
            {
                return AddPatientDataTemplate;
            }

            return EditPatientDataTemplate;
        }
    }
}
