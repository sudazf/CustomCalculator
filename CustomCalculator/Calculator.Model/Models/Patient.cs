using System;
using System.Collections.ObjectModel;
using System.Linq;
using Jg.wpf.core.Command;

namespace Calculator.Model.Models
{
    public class Patient
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Weight { get; set; }

        public ObservableCollection<Variable> Variables { get; }
        public ObservableCollection<Formula> Formulas { get; }

        public JCommand AddVariableCommand { get; }
        public JCommand AddFormulaCommand { get; }

        public Patient(string name, DateTime birthday, string weight)
        {
            Birthday = birthday;
            Weight = weight;
            Name = name;

            Variables = new ObservableCollection<Variable>();
            Formulas = new ObservableCollection<Formula>();

            AddVariableCommand = new JCommand("AddVariableCommand", OnAddVariable);
            AddFormulaCommand = new JCommand("AddFormulaCommand", OnAddFormula);
        }

        private void OnAddVariable(object obj)
        {

        }

        private void OnAddFormula(object obj)
        {
            Formulas.Add(new Formula(Variables.ToList()));
        }

        public void Test()
        {
            
        }

        public string Build()
        {
            return Formulas[0].Build();
        }
    }
}
