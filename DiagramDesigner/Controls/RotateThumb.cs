using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace DiagramDesigner.Controls
{
    public class RotateThumb : Thumb
    {
        private Point centerPoint;
        private Vector startVector;
        private double initialAngle;
        private ContentControl designerItem;
        private RotateTransform rotateTransform;
        private bool isByUser;

        public double Rotate
        {
            get { return (double)GetValue(RotateProperty); }
            set { SetValue(RotateProperty, value); }
        }

        public static readonly DependencyProperty RotateProperty =
            DependencyProperty.Register("Rotate", typeof(double), typeof(RotateThumb), new PropertyMetadata(0.0, (d, e) =>
            {
                RotateThumb thumb = d as RotateThumb;
                if (thumb != null && !thumb.isByUser)
                {
                    ContentControl designerItem = thumb.DataContext as ContentControl;
                    thumb.rotateTransform = designerItem.RenderTransform as RotateTransform;
                    if (thumb.rotateTransform == null)
                    {
                        thumb.rotateTransform = new RotateTransform(0, designerItem.ActualWidth / 2, designerItem.ActualHeight / 2);
                        designerItem.RenderTransform = thumb.rotateTransform;
                    }
                    
                    thumb.rotateTransform.Angle = thumb.Rotate;
                }
            }));

        public RotateThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.RotateThumb_DragDelta);
            DragStarted += new DragStartedEventHandler(this.RotateThumb_DragStarted);
            DragCompleted += RotateThumb_DragCompleted;
        }

        private void RotateThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isByUser = false;
        }

        private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = DataContext as ContentControl;
            if (this.designerItem == null)
            {
                return;
            }

            this.centerPoint = new Point(designerItem.ActualWidth / 2, designerItem.ActualHeight / 2);
            Point startPoint = Mouse.GetPosition(this.designerItem);
            this.startVector = Point.Subtract(startPoint, this.centerPoint);

            this.rotateTransform = designerItem.RenderTransform as RotateTransform;
            if (this.rotateTransform == null)
            {
                designerItem.RenderTransform = new RotateTransform(0, this.centerPoint.X, this.centerPoint.Y);
                this.initialAngle = 0;
            }
            else
            {
                this.rotateTransform.CenterX = this.centerPoint.X;
                this.rotateTransform.CenterY = this.centerPoint.Y;
                this.initialAngle = this.rotateTransform.Angle;
            }
            isByUser = true;
        }

        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null)
            {
                Point currentPoint = Mouse.GetPosition(this.designerItem);
                Vector deltaVector = Point.Subtract(currentPoint, this.centerPoint);
                double angle = Vector.AngleBetween(this.startVector, deltaVector);

                RotateTransform rotateTransform = this.designerItem.RenderTransform as RotateTransform;
                this.Rotate = this.initialAngle + Math.Round(angle, 0);
                rotateTransform.Angle = this.Rotate;
                this.designerItem.InvalidateMeasure();
            }
        }
    }
}
