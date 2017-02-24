using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FileExplorer.Controls
{
    /// <summary>
    /// Interaction logic for UCImage.xaml
    /// </summary>
    public partial class UCImage : UserControl
    {
        public UCImage()
        {
            InitializeComponent();
            img.PreviewMouseWheel += image_PreviewMouseWheel;
            img.PreviewMouseLeftButtonDown += (sender, e) => { isMoving = true; };
            img.PreviewMouseLeftButtonUp += (sender, e) => { isMoving = false; };
            img.PreviewMouseMove += Img_PreviewMouseMove;
        }

        bool isMoving = false;

        private Point lastPoint = new Point(-1, -1);
        private void Img_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!isMoving) return;

            Window win = null;
            FrameworkElement pa = this.Parent as FrameworkElement;
            while (true)
            {
                if (pa != null)
                {
                    if (pa is Window)
                    {
                        win = pa as Window;
                        break;
                    }
                    pa = pa.Parent as FrameworkElement;

                }
            };

            if (win == null)
            {
                return;
            }
            
            Point p = e.GetPosition(win);
            if (lastPoint.X < 0)
            {
                lastPoint = p;
            }
            else
            {
                Point np = new Point((p.X - lastPoint.X), (p.Y - lastPoint.Y));
                this.RenderTransform = new TranslateTransform(np.X, np.Y);
                this.CaptureMouse();
            }
        }

        private void image_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Debug.WriteLine(e.Delta);
            double rate = 1.0;
            if (e.Delta > 0)
            {
                rate = e.Delta / 100.0;
            }
            else
            {
                rate = (2 + e.Delta / 100.0);
            }
            this.Width *= rate;
            this.Height *= rate;
        }
    }
}
