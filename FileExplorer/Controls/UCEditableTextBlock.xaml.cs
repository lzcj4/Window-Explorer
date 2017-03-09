using FileExplorer.Helper;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileExplorer.Controls
{
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
            DependencyProperty.Register("Text", typeof(string), typeof(UCEditableTextBlock), new PropertyMetadata("新建信息"));

        public bool IsEditMode
        {
            get { return (bool)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }

        public static readonly DependencyProperty IsEditModeProperty =
            DependencyProperty.Register("IsEditMode", typeof(bool), typeof(UCEditableTextBlock), new PropertyMetadata(false));

        #endregion

        Window win;
        public UCEditableTextBlock()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => { win = this.TryFindParent<Window>(); };
        }

        private void labContent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetEditable();
        }

        private void SetEditable()
        {
            this.IsEditMode = !this.IsEditMode;
            if (win.IsNull())
            {
                win = this.TryFindParent<Window>();
                if (win.IsNull())
                {
                    return;
                }
            }
            if (this.IsEditMode)
            {
                win.PreviewMouseLeftButtonDown += Win_PreviewMouseLeftButtonDown;
            }
            else
            {
                win.PreviewMouseLeftButtonDown -= Win_PreviewMouseLeftButtonDown;
            }
        }

        private void Win_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(win);
            Rect rect = this.TransformToAncestor(win).TransformBounds(new Rect(0, 0, this.ActualWidth, this.ActualHeight));

            if (!rect.Contains(point))
            {
                SetEditable();
            }
        }
    }
}
