using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Shapes;
using Calculator.Model.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Calculator.Controls.Utilities
{
    public static class RichTextBoxHelper
    {
        public static readonly DependencyProperty InlinesProperty =
            DependencyProperty.RegisterAttached(
                "Inlines",
                typeof(List<Variable>),
                typeof(RichTextBoxHelper),
                new PropertyMetadata(null, OnInlinesChanged));

        public static void SetInlines(RichTextBox element, List<Variable> value)
        {
            element.SetValue(InlinesProperty, value);
        }

        public static List<Variable> GetInlines(RichTextBox element)
        {
            return (List<Variable>)element.GetValue(InlinesProperty);
        }

        private static void OnInlinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox richTextBox)
            {
                richTextBox.Document.Blocks.Clear();
                var paragraph = new Paragraph();
                if (e.NewValue is List<Variable> variables)
                {
                    foreach (var variable in variables)
                    {
                        var inlineUiContainer = new InlineUIContainer();
                        var textBlock = new TextBlock() { Text = variable.Name };
                        textBlock.Unloaded += TextBlock_Unloaded;
                        inlineUiContainer.Child = textBlock;

                        paragraph.Inlines.Add(inlineUiContainer);
                    }
                }
                richTextBox.Document.Blocks.Add(paragraph);
            }
        }

        private static void TextBlock_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                textBlock.Unloaded -= TextBlock_Unloaded;

                MessageBox.Show($"TextBlock removed: {textBlock.Text}");
            }
        }
    }
}
