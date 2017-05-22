using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace FileExplorer.Controls
{
    /// <summary>
    /// Interaction logic for UCAngle.xaml
    /// </summary>
    public partial class UCAngle : UserControl
    {
        public string RotateAngle
        {
            get { return (string)GetValue(RotateAngleProperty); }
            set { SetValue(RotateAngleProperty, value); }
        }
        public static readonly DependencyProperty RotateAngleProperty =
            DependencyProperty.Register("RotateAngle", typeof(string), typeof(UCAngle), new PropertyMetadata("30"));

        public UCAngle()
        {
            InitializeComponent();
            rotateUp.DataContext = pathUp;
            rotateUp.OnRotate += rotateUp_OnRotate;
            rotateDown.DataContext = pathBase;
            rotateDown.OnRotate += rotateDown_OnRotate;
            this.IsVisibleChanged += UCAngle_IsVisibleChanged;
            this.Loaded += (sender, e) => { this.Reset(); };
        }

        private void UCAngle_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool visible = (bool)e.NewValue;
            if (!visible)
            {
                this.Reset();
            }
        }

        const int upDefaultAngle = -30;
        const int downDefaultAngle = 0;
        private void rotateUp_OnRotate(object sender, EventArgs<int> e)
        {
            if (null == e)
            {
                return;
            }
            this.SetAngle(false);

            arcAngle.StartAngle = 90 - rotateUp.RotateAngle;
            arcAngle.EndAngle = 90 + (360 - rotateDown.RotateAngle);

            //Logger.Instance.Info("/**** upAngle:{0},downAngle:{1}    startAngel:{2},endAngle:{3}".StrFormat(
            //    rotateUp.RotateAngle, rotateDown.RotateAngle, arcAngle.StartAngle, arcAngle.EndAngle));
            SetThumbPos(rotateUp.RotateAngle, rotateUp);
        }

        private void rotateDown_OnRotate(object sender, EventArgs<int> e)
        {
            if (null == e)
            {
                return;
            }
            this.SetAngle(true);
            arcAngle.StartAngle = 90 - rotateUp.RotateAngle;
            arcAngle.EndAngle = 90 + (360 - rotateDown.RotateAngle);

            //Logger.Instance.Info("/**** upAngle:{0},downAngle:{1}    startAngel:{2},endAngle:{3}".StrFormat(
            //                      rotateUp.RotateAngle, rotateDown.RotateAngle, arcAngle.StartAngle, arcAngle.EndAngle));
            SetThumbPos(rotateDown.RotateAngle, rotateDown);
        }

        private void SetAngle(bool isDown)
        {
            int angle = 0;
            if (isDown)
            {
                if (rotateDown.RotateAngle < 0)
                {
                    angle = Math.Abs(rotateUp.RotateAngle) - Math.Abs(rotateDown.RotateAngle);
                }
                else
                {
                    angle = Math.Abs(rotateUp.RotateAngle) + (360 - rotateDown.RotateAngle);
                }
            }
            else
            {
                if (rotateUp.RotateAngle < 0)
                {
                    angle = Math.Abs(rotateUp.RotateAngle) - Math.Abs(rotateDown.RotateAngle);
                }
                else
                {
                    angle = Math.Abs(rotateUp.RotateAngle) + (360 - rotateDown.RotateAngle);
                }
            }
            angle %= 360;
            this.RotateAngle = angle.ToString("f0");
        }

        private void SetThumbPos(double angle, AngleRotateThumb thumb)
        {
            double centerX = this.canvas.ActualWidth / 2;
            double centerY = this.canvas.ActualHeight / 2;

            double radius = this.pathUp.ActualWidth;
            double oldLeft = Canvas.GetLeft(thumb);
            double oldTop = Canvas.GetTop(thumb);

            double left = centerX + Math.Cos(angle % 360 / 180 * Math.PI) * radius;
            double top = centerY - Math.Sin(angle % 360 / 180 * Math.PI) * radius;

            Canvas.SetLeft(thumb, left);
            Canvas.SetTop(thumb, top);
        }

        public void Reset()
        {
            this.RotateAngle = "30";
            arcAngle.StartAngle = 60;
            arcAngle.EndAngle = 90;
            rotateUp.Reset(upDefaultAngle);
            rotateUp.RotateAngle = Math.Abs(upDefaultAngle);
            rotateDown.Reset(downDefaultAngle);
            SetThumbPos(Math.Abs(upDefaultAngle), rotateUp);
            SetThumbPos(downDefaultAngle, rotateDown);
        }
    }

    public class AngleRotateThumb : Thumb
    {
        public event EventHandler<EventArgs<int>> OnRotate;
        private Point centerPoint;

        private RotateTransform rotateTransform;

        public Canvas ParentCanvas { get { return this.Parent as Canvas; } }
        public FrameworkElement RotateElement { get; set; }

        private int rotateAngle = 0;
        public int RotateAngle
        {
            get
            {
                return this.rotateAngle;
            }
            set { this.rotateAngle = value; }
        }

        public AngleRotateThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.RotateThumb_DragDelta);
            DragStarted += new DragStartedEventHandler(this.RotateThumb_DragStarted);
        }

        private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.RotateElement = DataContext as FrameworkElement;

            if (this.RotateElement == null || this.ParentCanvas == null)
            {
                return;
            }

            this.centerPoint = this.RotateElement.TranslatePoint(
                new Point(this.RotateElement.ActualWidth * this.RotateElement.RenderTransformOrigin.X,
                          this.RotateElement.ActualHeight * this.RotateElement.RenderTransformOrigin.Y),
                          this.ParentCanvas);

            Point startPoint = Mouse.GetPosition(this.ParentCanvas);

            this.rotateTransform = this.RotateElement.RenderTransform as RotateTransform;
            if (this.rotateTransform == null)
            {
                this.rotateTransform = new RotateTransform(this.RotateAngle);
                this.RotateElement.RenderTransform = this.rotateTransform;
                this.RenderTransform = this.rotateTransform;
            }
        }

        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.RotateElement == null || this.ParentCanvas == null)
            {
                return;
            }
            Point currentPoint = Mouse.GetPosition(this.ParentCanvas);
            Vector deltaVector = Point.Subtract(currentPoint, this.centerPoint);
            this.RotateAngle = GetAngle(centerPoint, currentPoint);
            RotateTransform rotateTransform = this.RotateElement.RenderTransform as RotateTransform;
            rotateTransform.Angle = -this.RotateAngle;
            this.RotateElement.InvalidateMeasure();
            if (OnRotate != null)
            {
                OnRotate(this, new EventArgs<int>(this.RotateAngle));
            }
        }

        private int GetAngle(Point startPoint, Point endPoint)
        {
            double atan = ((endPoint.Y - startPoint.Y) / (endPoint.X - startPoint.X));
            double solution = Math.Atan(atan);
            int angle = Convert.ToInt32(Math.Round(solution * 180 / Math.PI));

            if (endPoint.X > centerPoint.X && endPoint.Y < centerPoint.Y)
            {  //第一项限
                angle = Math.Abs(angle);

            }
            else if (endPoint.X < centerPoint.X && endPoint.Y < centerPoint.Y)
            {  //第二项限
                angle = 90 + (90 - Math.Abs(angle));

            }
            else if (endPoint.X < centerPoint.X && endPoint.Y > centerPoint.Y)
            { //第三项限
                angle = 180 + Math.Abs(angle);
            }
            else if (endPoint.X > centerPoint.X && endPoint.Y > centerPoint.Y)
            {//第四项限
                angle = 270 + (90 - Math.Abs(angle));
            }
            return angle;
        }

        public void Reset(int angle)
        {
            this.RotateAngle = angle;
            if (this.rotateTransform != null)
            {
                rotateTransform.Angle = this.RotateAngle;
                this.RotateElement.InvalidateMeasure();
            }
        }
    }

    public class EventArgs<T1> : EventArgs
    {
        public T1 Item1 { get; private set; }
        public EventArgs(T1 item1)
        {
            this.Item1 = item1;
        }
    }
}
