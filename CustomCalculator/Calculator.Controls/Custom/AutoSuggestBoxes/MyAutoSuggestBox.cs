﻿using System.Collections;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Calculator.Controls.Custom.AutoSuggestBoxes
{
    [TemplatePart(Name = AutoSuggestBoxListPart, Type = typeof(ListBox))]
    public class MyAutoSuggestBox : TextBox
    {
        private const string AutoSuggestBoxListPart = "PART_AutoSuggestBoxList";

        protected ListBox _autoSuggestBoxList;

        #region Dependency Properties

        public IEnumerable Suggestions
        {
            get => (IEnumerable)GetValue(SuggestionsProperty);
            set => SetValue(SuggestionsProperty, value);
        }

        public static readonly DependencyProperty SuggestionsProperty =
            DependencyProperty.Register(nameof(Suggestions), typeof(IEnumerable), typeof(MyAutoSuggestBox), new PropertyMetadata(null));


        public string ValueMember
        {
            get => (string)GetValue(ValueMemberProperty);
            set => SetValue(ValueMemberProperty, value);
        }
        public static readonly DependencyProperty ValueMemberProperty =
            DependencyProperty.Register(nameof(ValueMember), typeof(string), typeof(MyAutoSuggestBox), new PropertyMetadata(default(string)));


        public string DisplayMember
        {
            get => (string)GetValue(DisplayMemberProperty);
            set => SetValue(DisplayMemberProperty, value);
        }
        public static readonly DependencyProperty DisplayMemberProperty =
            DependencyProperty.Register(nameof(DisplayMember), typeof(string), typeof(MyAutoSuggestBox), new PropertyMetadata(default(string)));

        public Brush DropDownBackground
        {
            get => (Brush)GetValue(DropDownBackgroundProperty);
            set => SetValue(DropDownBackgroundProperty, value);
        }
        public static readonly DependencyProperty DropDownBackgroundProperty =
            DependencyProperty.Register(nameof(DropDownBackground), typeof(Brush), typeof(MyAutoSuggestBox), new PropertyMetadata(default(Brush)));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(MyAutoSuggestBox), new PropertyMetadata(default(DataTemplate)));

        public Style ItemContainerStyle
        {
            get => (Style)GetValue(ItemContainerStyleProperty);
            set => SetValue(ItemContainerStyleProperty, value);
        }
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(MyAutoSuggestBox), new PropertyMetadata(default(Style)));

        public Elevation DropDownElevation
        {
            get => (Elevation)GetValue(DropDownElevationProperty);
            set => SetValue(DropDownElevationProperty, value);
        }
        public static readonly DependencyProperty DropDownElevationProperty =
            DependencyProperty.Register(nameof(DropDownElevation), typeof(Elevation), typeof(MyAutoSuggestBox), new PropertyMetadata(default(Elevation)));

        public double DropDownMaxHeight
        {
            get => (double)GetValue(DropDownMaxHeightProperty);
            set => SetValue(DropDownMaxHeightProperty, value);
        }
        public static readonly DependencyProperty DropDownMaxHeightProperty =
            DependencyProperty.Register(nameof(DropDownMaxHeight), typeof(double), typeof(MyAutoSuggestBox), new PropertyMetadata(200.0));


        public bool IsSuggestionOpen
        {
            get => (bool)GetValue(IsSuggestionOpenProperty);
            set => SetValue(IsSuggestionOpenProperty, value);
        }
        public static readonly DependencyProperty IsSuggestionOpenProperty =
            DependencyProperty.Register(nameof(IsSuggestionOpen), typeof(bool), typeof(MyAutoSuggestBox), new PropertyMetadata(default(bool)));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(MyAutoSuggestBox), new PropertyMetadata(default(object)));


        public object SelectedValue
        {
            get => GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(object), typeof(MyAutoSuggestBox), new PropertyMetadata(default(object)));

        public static readonly RoutedEvent SuggestionChosenEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SuggestionChosen),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<object>),
                typeof(MyAutoSuggestBox));

        public event RoutedPropertyChangedEventHandler<object> SuggestionChosen
        {
            add => AddHandler(SuggestionChosenEvent, value);
            remove => RemoveHandler(SuggestionChosenEvent, value);
        }

        #endregion

        static MyAutoSuggestBox() => DefaultStyleKeyProperty.OverrideMetadata(typeof(MyAutoSuggestBox), new FrameworkPropertyMetadata(typeof(MyAutoSuggestBox)));

        #region Override methods

        public override void OnApplyTemplate()
        {
            if (_autoSuggestBoxList != null)
            {
                _autoSuggestBoxList.PreviewMouseDown -= AutoSuggestionListBox_PreviewMouseDown;
            }

            if (GetTemplateChild(AutoSuggestBoxListPart) is ListBox listBox)
            {
                _autoSuggestBoxList = listBox;

                base.OnApplyTemplate();

                listBox.PreviewMouseDown += AutoSuggestionListBox_PreviewMouseDown;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (_autoSuggestBoxList is null) return;
            switch (e.Key)
            {
                case Key.Down:
                    IncrementSelection();
                    break;
                case Key.Up:
                    DecrementSelection();
                    break;
                case Key.Enter:
                    CommitValueSelection();
                    break;
                case Key.Escape:
                    CloseAutoSuggestionPopUp();
                    break;
                case Key.Tab:
                    CommitValueSelection();
                    break;
                default:
                    return;
            }
            e.Handled = true;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            CloseAutoSuggestionPopUp();
        }
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (_autoSuggestBoxList is null)
                return;
            if ((Text.Length == 0 || _autoSuggestBoxList.Items.Count == 0) && IsSuggestionOpen)
                IsSuggestionOpen = false;
            else if (Text.Length > 0 && !IsSuggestionOpen && IsFocused && _autoSuggestBoxList.Items.Count > 0)
                IsSuggestionOpen = true;
        }

        #endregion

        #region Callback handlers

        private void AutoSuggestionListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_autoSuggestBoxList is null || !(e.OriginalSource is FrameworkElement element))
                return;

            var selectedItem = element.DataContext;
            if (!_autoSuggestBoxList.Items.Contains(selectedItem))
                return;

            e.Handled = true;

            if (!Equals(_autoSuggestBoxList.SelectedItem, selectedItem))
            {
                void OnSelectionChanged(object s, SelectionChangedEventArgs args)
                {
                    _autoSuggestBoxList.SelectionChanged -= OnSelectionChanged;
                    CommitValueSelection();
                }

                _autoSuggestBoxList.SelectionChanged += OnSelectionChanged;
                _autoSuggestBoxList.SelectedItem = selectedItem;
            }
            else
            {
                _autoSuggestBoxList.SelectedItem = selectedItem;
                CommitValueSelection();
            }
        }



        #endregion

        #region Methods

        private void CloseAutoSuggestionPopUp()
        {
            IsSuggestionOpen = false;
        }

        private void CommitValueSelection()
            => CommitValueSelection(_autoSuggestBoxList?.SelectedValue);

        private void CommitValueSelection(object selectedValue)
        {
            string oldValue = Text;
            Text = selectedValue?.ToString() ?? string.Empty;
            if (Text != null)
            {
                CaretIndex = Text.Length;
            }
            CloseAutoSuggestionPopUp();
            var args = new RoutedPropertyChangedEventArgs<object>(oldValue, Text)
            {
                RoutedEvent = SuggestionChosenEvent
            };
            RaiseEvent(args);
        }

        private void DecrementSelection()
        {
            if (_autoSuggestBoxList is null || Suggestions is null)
                return;
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(Suggestions);
            if (collectionView.IsCurrentBeforeFirst)
                collectionView.MoveCurrentToLast();
            else
                collectionView.MoveCurrentToPrevious();
            _autoSuggestBoxList.ScrollIntoView(_autoSuggestBoxList.SelectedItem);
        }

        private void IncrementSelection()
        {
            if (_autoSuggestBoxList is null || Suggestions is null)
                return;
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(Suggestions);
            if (collectionView.IsCurrentAfterLast)
                collectionView.MoveCurrentToFirst();
            else
                collectionView.MoveCurrentToNext();
            _autoSuggestBoxList.ScrollIntoView(_autoSuggestBoxList.SelectedItem);
        }

        #endregion
    }
}
