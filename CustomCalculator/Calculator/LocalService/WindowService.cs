using System.Windows;
using Calculator.Service.Services.App;
using Calculator.Views.Windows;

namespace Calculator.LocalService
{
    internal class WindowService : IWindowService
    {
        private Window _window;
        public object Result { get; private set; }

        public void ShowDialog(string title, object viewModel)
        {
            _window = new DialogWindow()
            {
                Owner = Application.Current.MainWindow,
                Title = title,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            if (viewModel is IResult resultViewModel)
            {
                resultViewModel.OnResultChanged += ResultViewModel_OnResultChanged;
            }
            _window.DataContext = viewModel;
            _window.Closing += Window_Closing;

            _window.ShowDialog();
        }

        public void Close()
        {
            if (_window != null && _window.IsActive)
            {
                _window.Close();
            }
        }

        private void ResultViewModel_OnResultChanged(object sender, object e)
        {
            Result = e;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_window.DataContext is IResult resultViewModel)
            {
                resultViewModel.OnResultChanged -= ResultViewModel_OnResultChanged;
            }
        }
    }
}
