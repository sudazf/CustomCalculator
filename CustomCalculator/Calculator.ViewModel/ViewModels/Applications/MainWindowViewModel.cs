using System;
using Calculator.Model.Models;
using Calculator.Service.Services.Parser;
using Jg.wpf.core.Service;

namespace Calculator.ViewModel.ViewModels.Applications
{
    public class MainWindowViewModel
    {
        private readonly IParser _parser;
        public MainWindowViewModel()
        {
            _parser = ServiceManager.GetService<IParser>();

            var patient = new Patient();
            patient.Test();
            var expression = patient.Build();

            Console.WriteLine(_parser.Parse(expression));
        }
    }
}
