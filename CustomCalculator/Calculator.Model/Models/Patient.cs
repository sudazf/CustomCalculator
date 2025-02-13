using System.Collections.ObjectModel;

namespace Calculator.Model.Models
{
    public class Patient
    {
        public ObservableCollection<Formula> Formulas { get; }

        public Patient()
        {
            Formulas = new ObservableCollection<Formula>();

        }

        public void Test()
        {
            Formulas.Add(new Formula()
            {
                Variables =
                {
                    new Variable("(", "("),
                    new Variable("一", "1"),
                    new Variable("加", "+"),
                    new Variable("二", "2"),
                    new Variable(")", ")"),
                    new Variable("*", "*"),
                    new Variable("三点义务", "3.15"),
                }
            });
        }

        public string Build()
        {
            return Formulas[0].Build();
        }
    }
}
