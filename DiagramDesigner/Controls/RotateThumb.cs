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

        private double rotateAngle = 0;
        public double RotateAngle
        {
            get
            {
                return this.rotateAngle;
            }
            private set { this.rotateAngle = value; }
        }

        public RotateThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.RotateThumb_DragDelta);
            DragStarted += new DragStartedEventHandler(this.RotateThumb_DragStarted);
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
                designerItem.RenderTransform = new RotateTransform(0, designerItem.ActualWidth / 2, designerItem.ActualHeight / 2);
                this.initialAngle = 0;
            }
            else
            {
                this.initialAngle = this.rotateTransform.Angle;
            }
        }

        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null)
            {
                Point currentPoint = Mouse.GetPosition(this.designerItem);
                Vector deltaVector = Point.Subtract(currentPoint, this.centerPoint);
                double angle = Vector.AngleBetween(this.startVector, deltaVector);

                RotateTransform rotateTransform = this.designerItem.RenderTransform as RotateTransform;
                this.RotateAngle = this.initialAngle + Math.Round(angle, 0);
                rotateTransform.Angle = this.RotateAngle;
                this.designerItem.InvalidateMeasure();
            }
        }
    }
}
