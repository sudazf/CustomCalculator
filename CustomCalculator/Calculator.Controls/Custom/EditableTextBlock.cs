using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator.Controls.Custom
{
    public class EditableTextBlock : Control
    {
        static EditableTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableTextBlock), new FrameworkPropertyMetadata(typeof(EditableTextBlock)));
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBlock), new PropertyMetadata(string.Empty, OnTextChanged));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty WaterMarkProperty =
            DependencyProperty.Register("WaterMark", typeof(string), typeof(EditableTextBlock), new PropertyMetadata(string.Empty));

        public string WaterMark
        {
            get => (string)GetValue(WaterMarkProperty);
            set => SetValue(WaterMarkProperty, value);
        }
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null || string.IsNullOrEmpty(e.NewValue.ToString()))
            {
                if (d is EditableTextBlock box)
                {
                    if (box._textBlock != null)
                    {
                        box._textBlock.Visibility = Visibility.Collapsed;
                    }
                    if (box._waterMarker != null)
                    {
                        box._waterMarker.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                if (d is EditableTextBlock box)
                {
                    if (box._textBlock != null)
                    {
                        box._textBlock.Visibility = Visibility.Visible;
                    }
                    if (box._waterMarker != null)
                    {
                        box._waterMarker.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private TextBox _textBox;
        private TextBlock _waterMarker;
        private TextBlock _textBlock;

        public EditableTextBlock()
        {
            Loaded += OnControlLoaded;
            Unloaded += OnControlUnloaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBlock = GetTemplateChild("PART_TextBlock") as TextBlock;
            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
            _waterMarker = GetTemplateChild("PART_WaterMarker") as TextBlock;
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            //在 loaded 中订阅事件，不要在 OnApplyTemplate 中订阅
            if (_textBlock != null)
            {
                _textBlock.MouseDown += TextBlock_MouseDown;
            }

            if (_textBox != null)
            {
                _textBox.LostFocus += TextBox_LostFocus;
            }

            if (_waterMarker != null)
            {
                _waterMarker.MouseDown += TextBlock_MouseDown;
            }

            UpdateUi();
        }

        private void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            if (_textBlock != null)
            {
                _textBlock.MouseDown -= TextBlock_MouseDown;
            }

            if (_textBox != null)
            {
                _textBox.LostFocus -= TextBox_LostFocus;
            }

            if (_waterMarker != null)
            {
                _waterMarker.MouseDown -= TextBlock_MouseDown;
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                _waterMarker.Visibility = Visibility.Collapsed;
                _textBlock.Visibility = Visibility.Collapsed;
                _textBox.Visibility = Visibility.Visible;
                _textBox.Focus();
                _textBox.SelectAll();

                //注意这里一定要设置为 true 否则可能被父类控件接管，导致焦点失败
                e.Handled = true;
            }
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _textBox.Visibility = Visibility.Collapsed;

            UpdateUi();

            Text = _textBox.Text;
        }

        private void UpdateUi()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                if (_textBlock != null)
                {
                    _textBlock.Visibility = Visibility.Visible;
                }
                if (_waterMarker != null)
                {
                    _waterMarker.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (_textBlock != null)
                {
                    _textBlock.Visibility = Visibility.Collapsed;
                }
                if (_waterMarker != null)
                {
                    _waterMarker.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
