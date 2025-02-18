using System;

namespace Calculator.Model.Models
{
    public class Formula : ICloneable
    {
        public string Id { get; set; }

        //去除分隔符的
        public string Expression { get; set; }

        //未去除分隔符的
        public string MetaExpression { get; set; }

        public Formula()
        {

        }

        public Formula(string metaExpression)
        {
            Id = Guid.NewGuid().ToString();
            MetaExpression = metaExpression;
            Expression = metaExpression.Replace(",", "");
        }

        public object Clone()
        {
            var clone = new Formula();
            clone.Id = Id;
            clone.Expression = Expression;
            clone.MetaExpression = MetaExpression;
            return clone;
        }
    }
}
