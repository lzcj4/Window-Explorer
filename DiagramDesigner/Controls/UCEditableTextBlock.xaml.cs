using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiagramDesigner.Controls
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
            DependencyProperty.Register("Text", typeof(string), typeof(UCEditableTextBlock), new PropertyMetadata(""));

        public bool IsEditMode
        {
            get { return (bool)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }

        public static readonly DependencyProperty IsEditModeProperty =
            DependencyProperty.Register("IsEditMode", typeof(bool), typeof(UCEditableTextBlock), new PropertyMetadata(false));

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(UCEditableTextBlock), new PropertyMetadata(false));

        #endregion

        Window win;
        public UCEditableTextBlock()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => {
                win = this.GetVisualAncestor<Window>();
            };
        }

        private void labContent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StartEdit();
        }

        private void StartEdit()
        {
            this.IsEditing = !this.IsEditing;
            this.txtContent.Focus();
            this.txtContent.CaretIndex = this.txtContent.Text.Length;
            if (win==null)
            {
                win = this.GetVisualAncestor<Window>();
                if (win==null)
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
