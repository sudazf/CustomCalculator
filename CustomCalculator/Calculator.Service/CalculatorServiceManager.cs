using Calculator.Service.Services.Database;
using Calculator.Service.Services.Parser;
using Jg.wpf.core.Service;

namespace Calculator.Service
{
    public class CalculatorServiceManager
    {
        public static void Init()
        {
            ServiceManager.RegisterService("ArithmeticParser", new ArithmeticParser());
            ServiceManager.RegisterService("SQLiteDataService", new SQLiteDataService());
        }
    }
}
