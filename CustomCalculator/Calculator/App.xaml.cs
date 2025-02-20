using Calculator.Service;
using Jg.wpf.core.Service;
using Jg.wpf.core.Service.ThemeService;
using System.Windows;

namespace Calculator
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceManager.Init(Application.Current.Dispatcher);
            LocalServiceManager.Init();
            CalculatorServiceManager.Init();

            var isLight = true;
            var themes = ServiceManager.GetService<IThemeService>();
            themes.Apply(isLight);

            base.OnStartup(e);
        }
    }
}
