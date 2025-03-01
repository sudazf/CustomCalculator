using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator.Views.Patients
{
    /// <summary>
    /// VariableTemplatesMaintain.xaml 的交互逻辑
    /// </summary>
    public partial class VariableTemplatesMaintain : UserControl
    {
        public VariableTemplatesMaintain()
        {
            InitializeComponent();
        }

        private void ListBoxItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem)
            {
                listBoxItem.IsSelected = true;
            }
        }
    }
}
