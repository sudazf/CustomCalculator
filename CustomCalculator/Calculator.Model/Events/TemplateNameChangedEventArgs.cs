using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator.Model.Models;

namespace Calculator.Model.Events
{
    public class TemplateNameChangedEventArgs
    {
        public string OldName { get; }
        public string NewName { get; }

        public SimpleVariableTemplate Template { get; }
        public TemplateNameChangedEventArgs(string oldName, string newName, SimpleVariableTemplate template)
        {
            OldName = oldName;
            NewName = newName;
            Template = template;
        }
    }
}
