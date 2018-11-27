using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiagramDesigner.UserControls
{
    /// <summary>
    /// Interaction logic for UCCenter.xaml
    /// </summary>
    public partial class UCCenter : UserControl
    {
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(UCCenter), new PropertyMetadata(Brushes.Transparent));

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }
        
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(UCCenter), new PropertyMetadata(Brushes.Black));

        public UCCenter()
        {
            InitializeComponent();
        }
    }
}
