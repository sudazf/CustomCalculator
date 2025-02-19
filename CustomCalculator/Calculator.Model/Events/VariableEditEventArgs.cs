using System.Collections.Generic;
using Calculator.Model.Models;

namespace Calculator.Model.Events
{
    public class VariableEditEventArgs
    {
        public bool IsCancel { get;  }
        public List<ExpressionItem> ExpressionItems { get;}

        public VariableEditEventArgs(bool isCancel, List<ExpressionItem> expressionItems)
        {
            IsCancel = isCancel;
            ExpressionItems = expressionItems;
        }
    }
}
