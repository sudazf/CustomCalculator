﻿using System;
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
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newVariable = (Variable)e.NewItems[0];
                    var paragraph = (Paragraph)richTextBox.Document.Blocks.FirstBlock;
                    if (paragraph != null)
                    {
                        var inlineUiContainer = new InlineUIContainer();
                        var textBlock = new TextBlock() { Text = newVariable.Name };
                        textBlock.Unloaded += TextBlock_Unloaded;
                        inlineUiContainer.Child = textBlock;
                        paragraph.Inlines.Add(inlineUiContainer);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
            }
        }

        private void TextBlock_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock textBlock && !IsAssociatedObjectUnloaded)
            {
                textBlock.Unloaded -= TextBlock_Unloaded;
                var removed = Variables.FirstOrDefault(v => v.Name == textBlock.Text);
                if (removed != null)
                {
                    Variables.Remove(removed);
                }
                MessageBox.Show($"TextBlock removed: {textBlock.Text}");
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
