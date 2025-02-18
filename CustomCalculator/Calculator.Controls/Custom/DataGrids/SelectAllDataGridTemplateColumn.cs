using System.Windows.Controls;
using System.Windows;
using Calculator.Controls.Utilities;

namespace Calculator.Controls.Custom.DataGrids
{
    public class SelectAllDataGridTemplateColumn : DataGridTemplateColumn
    {
        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            var contentPresenter = editingElement as ContentPresenter;

            var textBox = VisualUtility.GetChildByFrameworkType<TextBox>(contentPresenter);

            if (textBox != null)
            {
                textBox.Focus();
                textBox.SelectAll();
            }

            return null;
        }
    }
}
