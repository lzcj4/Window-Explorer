using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace FileExplorer.Controls
{
    /// <summary>
    /// Interaction logic for UCRect.xaml
    /// </summary>
    public partial class UCRect : UserControl
    {
        public Point PointLeftTop { get { return segmentLeftTop.Point; } }
        public Point PointRightTop { get { return segmentRightTop.Point; } }
        public Point PointLeftBottom { get { return segmentLeftBottom.Point; } }
        public Point PointRightBottom { get { return segmentRigthBottm.Point; } }

        LineSegment segmentLeftTop;
        LineSegment segmentRightTop;
        LineSegment segmentLeftBottom;
        LineSegment segmentRigthBottm;

        public UCRect()
        {
            InitializeComponent();
            this.Loaded += UCRect_Loaded;
        }

        const int thumbSize = 10;
        private void UCRect_Loaded(object sender, RoutedEventArgs e)
        {
            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point(50, 50);

            segmentRightTop = this.CreatePoint(new Point(200, 50), myPathFigure);
            myPathFigure.Segments.Add(segmentRightTop);

            segmentRigthBottm = this.CreatePoint(new Point(200, 200), myPathFigure);
            myPathFigure.Segments.Add(segmentRigthBottm);

            segmentLeftBottom = this.CreatePoint(new Point(50, 200), myPathFigure);
            myPathFigure.Segments.Add(segmentLeftBottom);

            segmentLeftTop = this.CreatePoint(new Point(50, 50), myPathFigure, true);
            myPathFigure.Segments.Add(segmentLeftTop);

            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures.Add(myPathFigure);
            pathRect.Data = myPathGeometry;
        }

        private LineSegment CreatePoint(Point point, PathFigure parentPath, bool isStartPoint = false)
        {
            LineSegment lineSeg = new LineSegment(point, true);
            DragPointThumb thumb = new DragPointThumb();
            thumb.Width = thumbSize;
            thumb.Height = thumbSize;
            thumb.Background = new SolidColorBrush(Colors.MediumBlue);
            thumb.LineSegment = lineSeg;
            thumb.StartPoint = point;
            thumb.isStartPoint = isStartPoint;
            thumb.PathFigure = parentPath;
            thumb.ParentCanvas = canvasDraw;
            canvasDraw.Children.Add(thumb);
            Canvas.SetLeft(thumb, point.X - thumbSize / 2);
            Canvas.SetTop(thumb, point.Y - thumbSize / 2);

            return lineSeg;
        }

        public class DragPointThumb : Thumb
        {
            public Canvas ParentCanvas { get; set; }
            public LineSegment LineSegment { get; set; }
            public PathFigure PathFigure { get; set; }
            public bool isStartPoint { get; set; }
            public Point StartPoint { get; set; }
            Point lastPoint;

            public DragPointThumb()
            {
                DragDelta += new DragDeltaEventHandler(this.DragPointThumb_DragDelta);
                DragStarted += new DragStartedEventHandler(this.DragPointThumb_DragStarted);
            }

            private void DragPointThumb_DragStarted(object sender, DragStartedEventArgs e)
            {
                double oldLeft = Canvas.GetLeft(this);
                double oldTop = Canvas.GetTop(this);
                lastPoint = Mouse.GetPosition(this.ParentCanvas);
            }

            private void DragPointThumb_DragDelta(object sender, DragDeltaEventArgs e)
            {
                if (this.ParentCanvas == null)
                {
                    return;
                }
                Point currentPoint = Mouse.GetPosition(this.ParentCanvas);
                Vector vector = currentPoint - lastPoint;
                this.SetPos(vector);
                lastPoint = currentPoint;
            }

            private void SetPos(Vector vetor)
            {
                double oldLeft = Canvas.GetLeft(this);
                double oldTop = Canvas.GetTop(this);

                double left = oldLeft + vetor.X;
                double top = oldTop + vetor.Y;
                SetPos(new Point(left, top));
            }

            private void SetPos(Point point)
            {
                Canvas.SetLeft(this, point.X);
                Canvas.SetTop(this, point.Y);

                this.LineSegment.Point = point;
                if (this.isStartPoint)
                {
                    this.PathFigure.StartPoint = point;
                }
            }

            public void Reset()
            {
                SetPos(this.StartPoint);
            }
        }
    }

}
