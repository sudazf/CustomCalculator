using System.ComponentModel;
using Calculator.ViewModel.ViewModels.Applications;
using System.Windows;
using Jg.wpf.core.Service.ThreadService;

namespace Calculator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.InitPatients();
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            TaskManagerFactory.Instance.Close();
        }
    }
}
