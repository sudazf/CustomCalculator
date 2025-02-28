using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using Jg.wpf.controls.Behaviors;
using System.Windows.Controls;
using Calculator.Model.Models;
using System.Windows.Documents;
using System.Windows.Input;

namespace Calculator.Controls.Behaviors
{
    public class CustomRichTextBoxBehavior: JgBehavior<RichTextBox>
    {
        public bool IsAssociatedObjectUnloaded { get; private set; }

        public int CaretIndex
        {
            get => (int)GetValue(CaretIndexProperty);
            set => SetValue(CaretIndexProperty, value);
        }

        public static readonly DependencyProperty CaretIndexProperty =
            DependencyProperty.Register(nameof(CaretIndex), typeof(int), typeof(CustomRichTextBoxBehavior), new PropertyMetadata(default(int)));    
        public ObservableCollection<ExpressionItem> ExpressionItems
        {
            get => (ObservableCollection<ExpressionItem>)GetValue(ExpressionItemsProperty);
            set => SetValue(ExpressionItemsProperty, value);
        }

        public static readonly DependencyProperty ExpressionItemsProperty =
            DependencyProperty.Register("ExpressionItems", typeof(ObservableCollection<ExpressionItem>), 
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

                    var newContainer = new InlineUIContainer();
                    var textBlock = new TextBlock()
                    {
                        Text = $"{newVariable.Name}",
                        Tag = newVariable.Id
                    };
                    textBlock.Unloaded += TextBlock_Unloaded;
                    newContainer.Child = textBlock;

                    // 在光标处插入 InlineUIContainer
                    var p = richTextBox.CaretPosition.GetAdjacentElement(LogicalDirection.Backward) as InlineUIContainer;
                    var n = richTextBox.CaretPosition.GetAdjacentElement(LogicalDirection.Forward) as InlineUIContainer;

                    if (p != null)
                    {
                        paragraph.Inlines.InsertAfter(p, newContainer); // 在前一个后面 插入
                    }
                    else
                    {
                        if (n != null)
                        {
                            paragraph.Inlines.InsertBefore(n, newContainer); // 在后一个前面 插入
                        }
                        else
                        {
                            paragraph.Inlines.Add(newContainer);
                        }
                    }

                    richTextBox.CaretPosition = newContainer.ContentEnd;

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
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
            AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
            // 注册命令处理程序以拦截粘贴操作
            CommandManager.AddPreviewExecutedHandler(AssociatedObject, OnPreviewExecuted);

            var richTextBox = AssociatedObject;
            var paragraph = (Paragraph)richTextBox.Document.Blocks.FirstBlock;
            if (paragraph == null)
            {
                return;
            }

            foreach (var expressionItem in ExpressionItems)
            {
                if (expressionItem.Name == "无公式")
                {
                    continue;
                }
                var inlineContainer = new InlineUIContainer();
                var textBlock = new TextBlock()
                {
                    Text = $"{expressionItem.Name}",
                    Tag = expressionItem.Id
                };
                textBlock.Unloaded += TextBlock_Unloaded;
                inlineContainer.Child = textBlock;
                paragraph.Inlines.Add(inlineContainer);
            }

            AssociatedObject.CaretPosition = paragraph.ContentEnd;
        }
        protected override void OnAssociatedObjectUnloaded()
        {
            IsAssociatedObjectUnloaded = true;
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
            CommandManager.RemovePreviewExecutedHandler(AssociatedObject, OnPreviewExecuted);

            var richTextBox = AssociatedObject;
            var paragraph = (Paragraph)richTextBox.Document.Blocks.FirstBlock;
            if (paragraph == null)
            {
                return;
            }
            paragraph.Inlines.Clear();
        }
        private void OnPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 屏蔽粘贴命令
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is RichTextBox richTextBox)
            {
                // 检查是否有选中内容
                bool hasSelection = !richTextBox.Selection.IsEmpty;

                // 允许的按键：Backspace 键、方向键、Home、End
                if (e.Key == Key.Back ||
                    e.Key == Key.Left || e.Key == Key.Right ||
                    e.Key == Key.Home || e.Key == Key.End)
                {
                    // 允许这些按键
                    e.Handled = false;
                }
                // 如果有选中内容且按下的键不是允许的按键，阻止输入,并且光标自动跳到末尾
                else if (hasSelection)
                {
                    richTextBox.CaretPosition = richTextBox.Document.ContentEnd;
                    // 阻止其他按键
                    e.Handled = true;
                }
                // 如果没有选中内容，且是不允许的按键，阻止输入
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void TextBlock_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock textBlock && !IsAssociatedObjectUnloaded)
            {
                textBlock.Unloaded -= TextBlock_Unloaded;

                var removed = ExpressionItems.FirstOrDefault(v => v.Id == (string)textBlock.Tag);
                if (removed != null)
                {
                    ExpressionItems.Remove(removed);
                }
            }
        }

        private void AssociatedObjectOnTextChanged(object sender, TextChangedEventArgs e)
        {
            GetCaretIndex(AssociatedObject);
        }
        private void AssociatedObject_SelectionChanged(object sender, RoutedEventArgs e)
        {
            GetCaretIndex(AssociatedObject);
        }

        private void GetCaretIndex(RichTextBox richTextBox)
        {
            var paragraph = (Paragraph)richTextBox.Document.Blocks.FirstBlock;
            if (paragraph == null)
            {
                return;
            }

            var caretPosition = richTextBox.CaretPosition;
            int index = 0;
            foreach (Inline inline in paragraph.Inlines)
            {
                if (inline is InlineUIContainer)
                {
                    var compare = caretPosition.CompareTo(inline.ContentEnd);
                    if (compare <= 0)
                    {
                        break;
                    }

                    index++;
                }
            }

            this.SetValue(CaretIndexProperty, index);
        }
    }
}
