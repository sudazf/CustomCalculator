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
using Microsoft.Xaml.Behaviors;

namespace Calculator.Controls.Behaviors
{
    public class CustomRichTextBoxBehavior: JgBehavior<RichTextBox>
    {
        public static List<RichTextBox> SyncCaretRiches = new List<RichTextBox>();

        public bool IsAssociatedObjectUnloaded { get; private set; }
        public bool ShowName { get; set; }
        public bool SyncCaret { get; set; }

        public ObservableCollection<ExpressionItem> Variables
        {
            get => (ObservableCollection<ExpressionItem>)GetValue(VariablesProperty);
            set => SetValue(VariablesProperty, value);
        }

        public static readonly DependencyProperty VariablesProperty =
            DependencyProperty.Register("Variables", typeof(ObservableCollection<ExpressionItem>), 
                typeof(CustomRichTextBoxBehavior), new PropertyMetadata(default, OnVariablesChanged));

        private static void OnVariablesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomRichTextBoxBehavior behavior)
            {
                if (e.NewValue is ObservableCollection<ExpressionItem> variableCollection)
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
                    var newVariable = (ExpressionItem)e.NewItems[0];

                    var inlineContainer = new InlineUIContainer();
                    var textBlock = new TextBlock()
                    {
                        Text = ShowName ? $"{newVariable.Name}" : $"{newVariable.Value}",
                        Tag = newVariable.Id
                    };
                    textBlock.Unloaded += TextBlock_Unloaded;
                    inlineContainer.Child = textBlock;

                    // 获取当前光标位置
                    var caretPosition = richTextBox.CaretPosition;
                    // 找到光标位置之前的 Inline 元素
                    Inline previousInline = null;

                    foreach (Inline inline in paragraph.Inlines)
                    {
                        if (inline is Run)
                        {
                            continue;
                        }
                        var compareResultPreview = caretPosition.CompareTo(inline.ContentEnd);
                        if (compareResultPreview < 0)
                        {
                            paragraph.Inlines.InsertBefore(inline, inlineContainer);
                            return;
                        }
                        else
                        {
                            var compareResultNext = 1;
                            var nextInline = inline.NextInline;
                            while (nextInline != null)
                            {
                                if (!(nextInline is InlineUIContainer))
                                {
                                    nextInline = nextInline.NextInline;
                                }
                                else
                                {
                                    compareResultNext = caretPosition.CompareTo(nextInline.ContentStart);
                                    break;
                                }
                            }

                            //1 在后面，-1 在前面
                            if (compareResultNext <= 0)
                            {
                                previousInline = inline;
                                break;
                            }
                        }
                    }
                    // 插入 InlineUIContainer
                    if (previousInline != null)
                    {
                        paragraph.Inlines.InsertAfter(previousInline, inlineContainer);
                    }
                    else
                    {
                        paragraph.Inlines.Add(inlineContainer);
                        richTextBox.CaretPosition = richTextBox.Document.ContentEnd;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var removes = e.OldItems;
                    foreach (ExpressionItem variable in removes)
                    {
                        var uiContainer = paragraph.Inlines.FirstOrDefault(u =>
                        {
                            if (u is InlineUIContainer)
                            {
                                return (((TextBlock)((InlineUIContainer)u).Child).Tag.ToString() == variable.Id);
                            }
                    
                            return false;
                        });

                        if (uiContainer != null)
                        {
                            paragraph.Inlines.Remove(uiContainer);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    paragraph.Inlines.Clear();
                    break;
            }
        }

        protected override void OnAssociatedObjectLoaded()
        {
            IsAssociatedObjectUnloaded = false;
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;

            if (SyncCaret)
                SyncCaretRiches.Add(AssociatedObject);

            var richTextBox = AssociatedObject;
            var paragraph = (Paragraph)richTextBox.Document.Blocks.FirstBlock;
            if (paragraph == null)
            {
                return;
            }

            foreach (var variable in Variables)
            {
                var inlineContainer = new InlineUIContainer();
                var textBlock = new TextBlock()
                {
                    Text = ShowName ? $"{variable.Name}" : $"{variable.Value}",
                    Tag = variable.Id
                };
                textBlock.Unloaded += TextBlock_Unloaded;
                inlineContainer.Child = textBlock;
                paragraph.Inlines.Add(inlineContainer);
            }
        }

        protected override void OnAssociatedObjectUnloaded()
        {
            IsAssociatedObjectUnloaded = true;
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;

            if (SyncCaret)
                SyncCaretRiches.Remove(AssociatedObject);

            var richTextBox = AssociatedObject;
            var paragraph = (Paragraph)richTextBox.Document.Blocks.FirstBlock;
            if (paragraph == null)
            {
                return;
            }
            paragraph.Inlines.Clear();
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



        private void AssociatedObject_SelectionChanged(object sender, RoutedEventArgs e)
        {
            foreach (var richTextBox in SyncCaretRiches)
            {
                if (richTextBox.Equals(AssociatedObject))
                {
                    continue;
                }
                else
                {
                    SyncCaretPosition(AssociatedObject, richTextBox);
                }
            }
        }

        private void SyncCaretPosition(RichTextBox source, RichTextBox target)
        {
            // 获取源 RichTextBox 的光标位置
            TextPointer caretPosition = source.CaretPosition;

            // 计算光标在文档中的偏移量
            int offset = GetCaretOffset(source, caretPosition);

            // 在目标 RichTextBox 中设置光标位置
            SetCaretOffset(target, offset);
        }
        private int GetCaretOffset(RichTextBox richTextBox, TextPointer caretPosition)
        {
            // 获取文档的起始位置
            TextPointer startPosition = richTextBox.Document.ContentStart;

            // 计算光标相对于文档起始位置的偏移量
            return startPosition.GetOffsetToPosition(caretPosition);
        }
        private void SetCaretOffset(RichTextBox richTextBox, int offset)
        {
            // 获取文档的起始位置
            TextPointer startPosition = richTextBox.Document.ContentStart;

            // 根据偏移量获取目标光标位置
            TextPointer targetPosition = startPosition.GetPositionAtOffset(offset, LogicalDirection.Forward);

            if (targetPosition != null && richTextBox.CaretPosition.CompareTo(targetPosition) != 0)
            {
                // 设置目标 RichTextBox 的光标位置
                richTextBox.CaretPosition = targetPosition;
            }
        }
    }
}
