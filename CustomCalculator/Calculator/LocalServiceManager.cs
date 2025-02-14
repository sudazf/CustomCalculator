using Calculator.LocalService;
using Jg.wpf.core.Service;

namespace Calculator
{
    internal class LocalServiceManager
    {
        public static void Init()
        {
            ServiceManager.RegisterService("PatientService", new PatientService());
        }
    }
}
