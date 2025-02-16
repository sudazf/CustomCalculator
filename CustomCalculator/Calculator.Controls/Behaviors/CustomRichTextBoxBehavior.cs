using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Jg.wpf.controls.Behaviors;
using System.Windows.Controls;
using Calculator.Model.Models;
using System.Windows.Documents;

namespace Calculator.Controls.Behaviors
{
    public class CustomRichTextBoxBehavior: JgBehavior<RichTextBox>
    {
        public bool IsAssociatedObjectUnloaded { get; set; }

        public bool ShowName { get; set; }

        public ObservableCollection<Variable> Variables
        {
            get => (ObservableCollection<Variable>)GetValue(VariablesProperty);
            set => SetValue(VariablesProperty, value);
        }

        public static readonly DependencyProperty VariablesProperty =
            DependencyProperty.Register("Variables", typeof(ObservableCollection<Variable>), 
                typeof(CustomRichTextBoxBehavior), new PropertyMetadata(default, OnVariablesChanged));

        private static void OnVariablesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomRichTextBoxBehavior behavior)
            {
                if (e.NewValue is ObservableCollection<Variable> variableCollection)
                {
                    variableCollection.CollectionChanged += behavior.OnVariableCollectionChanged;
                }
            }
        }

        private void OnVariableCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var richTextBox = AssociatedObject;
            var paragraph = (Paragraph)richTextBox.Document.Blocks.FirstBlock;
            if (paragraph == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newVariable = (Variable)e.NewItems[0];
                    var inlineUiContainer = new InlineUIContainer();
                    var textBlock = new TextBlock()
                    {
                        Text = ShowName ? newVariable.Name : newVariable.Value,
                        Tag = newVariable.Id
                    };
                    textBlock.Unloaded += TextBlock_Unloaded;
                    inlineUiContainer.Child = textBlock;
                    paragraph.Inlines.Add(inlineUiContainer);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var removes = e.OldItems;
                    foreach (Variable variable in removes)
                    {
                        var uiContainer = paragraph.Inlines.FirstOrDefault(u=>(((TextBlock)((InlineUIContainer)u).Child).Tag.ToString() == variable.Id));
                        if (uiContainer != null)
                        {
                            paragraph.Inlines.Remove(uiContainer);
                        }
                    }
                    break;
            }
        }

        private void TextBlock_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock textBlock && !IsAssociatedObjectUnloaded)
            {
                textBlock.Unloaded -= TextBlock_Unloaded;
                var removed = Variables.FirstOrDefault(v => v.Id == (string)textBlock.Tag);
                if (removed != null)
                {
                    Variables.Remove(removed);
                }
            }
        }

        protected override void OnAssociatedObjectLoaded()
        {
            IsAssociatedObjectUnloaded = false;
        }

        protected override void OnAssociatedObjectUnloaded()
        {
            IsAssociatedObjectUnloaded = true;
        }
    }
}
