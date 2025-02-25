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
            
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                if (vm.PatientChanged)
                {
                    vm.InitSelectPatientDays();
                }
            }
        }

        private void ListBoxItem_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem)
            {
                listBoxItem.IsSelected = true;
            }
        }
    }
}
