using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FileExplorer.Controls
{
    public class TiangleMeasure : Canvas
    {
        const int clickOffset = 20;
        Point downPoint;
        SolidColorBrush brushBlack = new SolidColorBrush(Colors.Black);

        public override void EndInit()
        {
            base.EndInit();
            AddDragDelta();
        }
        

        double rectWidth = 5;
        private void AddDragDelta()
        {
            double drawWidth = this.ActualWidth;
            double drawHeight = this.ActualHeight;
            double radius = Math.Min(drawWidth, drawHeight) / 2;
            Point centerPoint = new Point(drawWidth / 2, drawHeight / 2);
            double left = centerPoint.X + radius * Math.Cos(angle / 180 * Math.PI);
            double top = centerPoint.Y - radius * Math.Sin(angle / 180 * Math.PI);

            Ellipse rectangle = new Ellipse() { Width = rectWidth, Height = rectWidth };
            rectangle.Stroke = brushBlack;
            rectangle.Fill = brushBlack;
            this.Children.Add(rectangle);
            Canvas.SetTop(rectangle, top);
            Canvas.SetLeft(rectangle, left);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            downPoint = e.GetPosition(this);
        }

        double angle = 45;
        Pen pen = new Pen(Brushes.Red, 2);
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            double drawWidth = this.ActualWidth;
            double drawHeight = this.ActualHeight;
            double radius = Math.Min(drawWidth, drawHeight) / 2;
            Point centerPoint = new Point(drawWidth / 2, drawHeight / 2);
            dc.DrawLine(pen, centerPoint, new Point(centerPoint.X + radius, centerPoint.Y));
            dc.DrawLine(pen, centerPoint, new Point(centerPoint.X + radius * Math.Cos(angle / 180 * Math.PI),
                                                    centerPoint.Y - radius * Math.Sin(angle / 180 * Math.PI)));
        }
    }
}
