using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jg.wpf.core.Notify;

namespace Calculator.Model.Models
{
    public class Formula : ViewModelBase, ICloneable
    {
        private string _expression;
        private ObservableCollection<ExpressionItem> _expressionItems;

        public string Id { get; set; }
        //去除分隔符的
        public string Expression
        {
            get => _expression;
            set
            {
                if (value == _expression) return;
                _expression = value;
                RaisePropertyChanged(nameof(Expression));
            }
        }
        //未去除分隔符的
        public string MetaExpression { get; set; }


        public ObservableCollection<ExpressionItem> ExpressionItems
        {
            get => _expressionItems;
            set
            {
                if (Equals(value, _expressionItems)) return;
                _expressionItems = value;
                RaisePropertyChanged(nameof(ExpressionItems));
            }
        }

        public Formula()
        {

        }
        public Formula(string metaExpression)
        {
            Id = Guid.NewGuid().ToString();
            MetaExpression = metaExpression;
            Expression = metaExpression.Replace(",", "");

            ExpressionItems = new ObservableCollection<ExpressionItem>();
        }

        public object Clone()
        {
            var clone = new Formula();
            clone.Id = Id;
            clone.Expression = Expression;
            clone.MetaExpression = MetaExpression;

            var expressionItems = new ObservableCollection<ExpressionItem>();
            foreach (var item in ExpressionItems)
            {
                expressionItems.Add((ExpressionItem)item.Clone());
            }
            clone.ExpressionItems = expressionItems;

            return clone;
        }
    }
}
