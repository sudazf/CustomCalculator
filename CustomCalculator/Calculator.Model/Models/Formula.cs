using System.Collections.ObjectModel;
using System.Linq;

namespace Calculator.Model.Models
{
    public class Formula
    {
        public ObservableCollection<Variable> Variables { get; }
        public Formula()
        {
            Variables = new ObservableCollection<Variable>();


        }

        public string Build()
        {
           return string.Join("", Variables.Select(v => v.Value));
        }
    }
}
