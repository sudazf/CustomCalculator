using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Calculator.Controls.Utilities
{
    public class VisualUtility
    {
        public static T GetChildByFrameworkType<T>(DependencyObject target)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(target);
            for (int i = 0; i < childCount; i++)
            {
                var newTarget = VisualTreeHelper.GetChild(target, i);
                if (newTarget is T element)
                {
                    return element;
                }

                var next = GetChildByFrameworkType<T>(newTarget);
                if (next != null)
                {
                    return next;
                }
            }

            return default(T);
        }
    }
}
