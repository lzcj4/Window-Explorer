using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApplication3
{
    public static class VisualTreeExtentions
    {
        public static T TryFindParent<T>(this DependencyObject d) where T : class
        {
            DependencyObject item = VisualTreeHelper.GetParent(d);

            while (item != null)
            {
                T itemAsT = item as T;
                if (itemAsT != null) return itemAsT;
                item = VisualTreeHelper.GetParent(item);
            }

            return null;
        }
    }
    /// <summary>
    /// Interaction logic for UCEditableTextBlock.xaml
    /// </summary>
    public partial class UCEditableTextBlock : UserControl
    {
        #region DP

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(UCEditableTextBlock), new PropertyMetadata(""));

        public bool IsEditMode
        {
            get { return (bool)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }

        public static readonly DependencyProperty IsEditModeProperty =
            DependencyProperty.Register("IsEditMode", typeof(bool), typeof(UCEditableTextBlock), new PropertyMetadata(true));

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(UCEditableTextBlock), new PropertyMetadata(false));

        public VerticalAlignment TextVerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(TextVerticalContentAlignmentProperty); }
            set { SetValue(TextVerticalContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty TextVerticalContentAlignmentProperty =
            DependencyProperty.Register("TextVerticalContentAlignment", typeof(VerticalAlignment), typeof(UCEditableTextBlock), new PropertyMetadata(VerticalAlignment.Center));

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(UCEditableTextBlock), new PropertyMetadata(TextWrapping.WrapWithOverflow));


        public static readonly DependencyProperty EditBorderBrushProperty =
            DependencyProperty.Register("EditBorderBrush", typeof(Brush), typeof(UCEditableTextBlock), new PropertyMetadata(Brushes.Wheat));


        public Brush EditBorderBrush
        {
            get { return (Brush)GetValue(EditBorderBrushProperty); }
            set { SetValue(EditBorderBrushProperty, value); }
        }
        /// <summary>
        /// 是左键双击还是单击打开编辑
        /// </summary>
        public bool IsEditByDoubleClick
        {
            get { return (bool)GetValue(IsEditByDoubleClickProperty); }
            set { SetValue(IsEditByDoubleClickProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditByDoubleClick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditByDoubleClickProperty =
            DependencyProperty.Register("IsEditByDoubleClick", typeof(bool), typeof(UCEditableTextBlock), new PropertyMetadata(false, (d, e) =>
            {
                (d as UCEditableTextBlock).UpdateEditEvents();
            }));

        #endregion

        Window win;
        public UCEditableTextBlock()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                win = this.TryFindParent<Window>();
                txtContent.PreviewKeyUp += TxtContent_PreviewKeyUp;
            };
            UpdateEditEvents();
        }

        private void UpdateEditEvents()
        {
            labContent.PreviewMouseDoubleClick -= labContent_MouseDoubleClick;
            labContent.PreviewMouseLeftButtonUp -= labContent_PreviewMouseLeftButtonUp;
            if (this.IsEditByDoubleClick)
            {
                labContent.PreviewMouseDoubleClick += labContent_MouseDoubleClick;
            }
            else
            {
                labContent.PreviewMouseLeftButtonUp += labContent_PreviewMouseLeftButtonUp;
            }
        }

        private void TxtContent_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartEdit();
                e.Handled = true;
            }
        }

        private void labContent_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StartEdit();
        }

        private void labContent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StartEdit();
        }

        private void StartEdit(bool isBinding = false)
        {
            if (!isBinding)
            {
                this.IsEditing = !this.IsEditing;
            }

            this.txtContent.Focus();
            this.txtContent.CaretIndex = this.txtContent.Text.Length;

            if (win == null)
            {
                win = this.TryFindParent<Window>();
                if (win == null)
                {
                    return;
                }
            }

            if (this.IsEditing)
            {
                win.PreviewMouseLeftButtonDown += Win_PreviewMouseLeftButtonDown;
            }
            else
            {
                win.PreviewMouseLeftButtonDown -= Win_PreviewMouseLeftButtonDown;
            }
        }

        /// <summary>
        /// 点击到编辑区以外，结束编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Win_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(win);
            Rect rect = this.TransformToAncestor(win).TransformBounds(new Rect(0, 0, this.ActualWidth, this.ActualHeight));

            if (!rect.Contains(point))
            {
                StartEdit();
            }
        }
    }
}
