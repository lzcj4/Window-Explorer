using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DiagramDesigner
{
    public class RuleBorder : Border
    {
        ScaleTransform scaleTrans;
        double scaleRate = 1.2;
        public RuleBorder()
        {
            Padding = new Thickness(20);
            //TransformGroup transGroup = new TransformGroup();
            //scaleTrans = new ScaleTransform();
            //transGroup.Children.Add(scaleTrans);
            //this.Child.RenderTransform = transGroup;

            this.Loaded += (sender, e) =>
            {
                if (this.Child == null) return;
                TransformGroup transGroup = new TransformGroup();
                scaleTrans = new ScaleTransform();
                transGroup.Children.Add(scaleTrans);
                this.Child.RenderTransform = transGroup;
            };
        }

        #region DP

        public int AreaWidth
        {
            get { return (int)GetValue(AreaWidthProperty); }
            set { SetValue(AreaWidthProperty, value); }
        }

        public static readonly DependencyProperty AreaWidthProperty =
            DependencyProperty.Register("AreaWidth", typeof(int), typeof(RuleBorder), new PropertyMetadata(100));

        public int AreaHeight
        {
            get { return (int)GetValue(AreaHeightProperty); }
            set { SetValue(AreaHeightProperty, value); }
        }

        public static readonly DependencyProperty AreaHeightProperty =
            DependencyProperty.Register("AreaHeight", typeof(int), typeof(RuleBorder), new PropertyMetadata(100));

        #endregion

        #region Draw

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            DrawRuleX(dc);
            DrawRuleY(dc);
        }

        int smallStep = 1;
        int smallStepHeight = 5;
        Pen smallStepPen = new Pen(Brushes.Black, 1);

        int bigStep = 5;
        int bigStepHeight = 15;
        Pen bigStepPen = new Pen(Brushes.Black, 1);

        double fontSize = 14;
        string fontTypeFace = "微软雅黑";

        private void DrawRuleX(DrawingContext dc)
        {
            int totalSteps = 100;
            double minX = 0;
            double maxX = this.ActualWidth;

            double stepDistance = (maxX - minX) / totalSteps;

            for (int i = 1; i <= totalSteps; i++)
            {
                if (i % 5 == 0)
                {
                    dc.DrawLine(bigStepPen, new Point(stepDistance * i, 0), new Point(stepDistance * i, bigStepHeight));
                    FormattedText txt = new FormattedText(i.ToString(), CultureInfo.CurrentCulture,
                                                          FlowDirection.LeftToRight, new Typeface(fontTypeFace),
                                                          fontSize, bigStepPen.Brush);
                    dc.DrawText(txt, new Point(stepDistance * i + 2, bigStepHeight - bigStep * 2));
                }
                else
                {
                    dc.DrawLine(smallStepPen, new Point(stepDistance * i, 0), new Point(stepDistance * i, smallStepHeight));
                }
            }
        }

        private void DrawRuleY(DrawingContext dc)
        {
            int totalSteps = 100;
            double minY = 0;
            double maxY = this.ActualHeight;

            double stepDistance = (maxY - minY) / totalSteps;

            for (int i = 1; i <= totalSteps; i++)
            {
                if (i % 5 == 0)
                {
                    dc.DrawLine(bigStepPen, new Point(0, stepDistance * i), new Point(bigStepHeight, stepDistance * i));
                    FormattedText txt = new FormattedText(i.ToString(), CultureInfo.CurrentCulture,
                                                    FlowDirection.LeftToRight, new Typeface(fontTypeFace),
                                                    fontSize, bigStepPen.Brush);
                    dc.DrawText(txt, new Point(bigStepHeight - bigStep, stepDistance * i));
                }
                else
                {
                    dc.DrawLine(bigStepPen, new Point(0, stepDistance * i), new Point(smallStepHeight, stepDistance * i));
                }
            }
        }

        #endregion

        #region  Mouse Actions

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            Point pt = e.GetPosition(this);
            this.scaleTrans.CenterX = pt.X;
            this.scaleTrans.CenterY = pt.Y;

            if (e.Delta > 0)
            {
                this.scaleTrans.ScaleX *= scaleRate;
                this.scaleTrans.ScaleY *= scaleRate;
            }
            else
            {
                this.scaleTrans.ScaleX /= scaleRate;
                this.scaleTrans.ScaleY /= scaleRate;
            }
            e.Handled = true;
        }

        #endregion


    }
}
