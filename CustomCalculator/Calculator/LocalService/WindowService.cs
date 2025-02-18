using System.Windows;
using Calculator.Service.Services.App;
using Calculator.Views.Windows;

namespace Calculator.LocalService
{
    internal class WindowService : IWindowService
    {
        private Window _window;
        public void ShowDialog(string title, object viewModel)
        {
            _window = new DialogWindow()
            {
                Owner = Application.Current.MainWindow,
                Title = title,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                DataContext = viewModel
            };

            _window.ShowDialog();
        }

        public void Close()
        {
            if (_window != null && _window.IsActive)
            {
                _window.Close();
            }
        }
    }
}
