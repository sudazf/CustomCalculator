using Calculator.ViewModel.ViewModels.Applications;
using System.Windows;
using System.Windows.Controls;

namespace Calculator.Views.Patients
{
    /// <summary>
    /// PatientCalculationView.xaml 的交互逻辑
    /// </summary>
    public partial class PatientCalculationView : UserControl
    {
        public PatientCalculationView()
        {
            InitializeComponent();
        }

        private void PatientCalculationView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                if (vm.PatientChanged)
                {
                    vm.InitSelectPatientDays();
                }
            }
        }
    }
}
