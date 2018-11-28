using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiagramDesigner.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UCArrow : UserControl
    {

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(UCArrow), new PropertyMetadata(Brushes.Black));

        public UCArrow()
        {
            InitializeComponent();
        }
    }
}
