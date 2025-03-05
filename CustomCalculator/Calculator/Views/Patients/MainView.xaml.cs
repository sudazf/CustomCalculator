using System.Windows.Controls;
using Calculator.Service.Services.App;
using Calculator.ViewModel.ViewModels.Applications;
using Jg.wpf.core.Service;

namespace Calculator.Views.Patients
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl
    {
        private readonly IWindowService _windowService;

        public MainView()
        {
            InitializeComponent();

            _windowService = ServiceManager.GetService<IWindowService>();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != sender)
            {
                return;
            }

            if (DataContext is MainWindowViewModel mainVm)
            {
                if (mainVm.SelectPatient == null || mainVm.SelectPatient.Days == null)
                {
                    return;
                }

                var dirty = false;
                foreach (var day in mainVm.SelectPatient.Days)
                {
                    if (day.ShowDirtyMarker)
                    {
                        dirty = true;
                        break;
                    }
                }

                if (mainVm.SelectPatient.IsDirty)
                {
                    dirty = true;
                }

                if (dirty)
                {
                    if (!(e.AddedItems[0] is PatientCalculationView))
                    {
                        _windowService.ShowDialog("系统提示", new ConfirmViewModel($"检测到修改，是否自动保存?"));
                        if (_windowService.Result is bool confirm)
                        {
                            if (confirm)
                            {
                                mainVm.SaveVariablesCommand.Execute(null);
                            }

                            foreach (var day in mainVm.SelectPatient.Days)
                            {
                                day.ShowDirtyMarker = false;
                            }

                            mainVm.SelectPatient.IsDirty = false;
                        }
                    }
                }
            }
        }
    }
}
