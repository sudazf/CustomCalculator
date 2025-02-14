using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Calculator.Model.Models;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;

namespace Calculator.ViewModel.ViewModels.Formulas
{
    public class FormulaConfigViewModel : ViewModelBase
    {
        public List<JCommand> Commands { get; }
        public List<Variable> Variables { get; }
        public ObservableCollection<Variable> SettingVariables { get; }

        public FormulaConfigViewModel(List<Variable> variables)
        {
            SettingVariables = new ObservableCollection<Variable>();
            Variables = variables;
            Commands = new List<JCommand>();

            foreach (var variable in Variables)
            {
                Commands.Add(new JCommand(variable.Id, OnCommandExecute, null, variable.Name));
            }
        }

        private void OnCommandExecute(object obj)
        {
            if (obj is string id)
            {
                var variable = Variables.FirstOrDefault(v => v.Id == id);
                if (variable != null)
                {
                    SettingVariables.Add(variable);
                }
            }
        }
    }
}
