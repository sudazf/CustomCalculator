using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Model.Events
{
    public class VariableEditEventArgs
    {
        public string MetaExpression { get; }
        public string Expression { get; }
        public bool IsCancel { get;  }

        public VariableEditEventArgs(bool isCancel, string expression, string metaExpression)
        {
            IsCancel = isCancel;
            Expression = expression;
            MetaExpression = metaExpression;
        }
    }
}
