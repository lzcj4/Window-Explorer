using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiagramDesigner
{
    public class RuleCanvas : Grid
    {

        #region DP

        public int AreaWidth
        {
            get { return (int)GetValue(AreaWidthProperty); }
            set { SetValue(AreaWidthProperty, value); }
        }

        public static readonly DependencyProperty AreaWidthProperty =
            DependencyProperty.Register("AreaWidth", typeof(int), typeof(RuleCanvas), new PropertyMetadata(100));

        public int AreaHeight
        {
            get { return (int)GetValue(AreaHeightProperty); }
            set { SetValue(AreaHeightProperty, value); }
        }

        public static readonly DependencyProperty AreaHeightProperty =
            DependencyProperty.Register("AreaHeight", typeof(int), typeof(RuleCanvas), new PropertyMetadata(100));

        #endregion

        #region Override Methods

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
        int bigStepHeight = 10;
        Pen bigStepPen = new Pen(Brushes.Black, 1);

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
                }
                else
                {
                    dc.DrawLine(bigStepPen, new Point(0, stepDistance * i), new Point(smallStepHeight, stepDistance * i));
                }
            }
        }

        #endregion


    }
}
