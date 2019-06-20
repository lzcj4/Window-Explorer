using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApplication3
{
    public class DrawCanvas : Canvas
    {
        #region Props

        public double DrawWidth
        {
            get { return this.ActualWidth > 0 ? this.ActualWidth : this.Width; }
        }

        public double DrawHeight
        {
            get { return this.ActualHeight > 0 ? this.ActualHeight : this.Height; }
        }

        private Brush bakcgroundBrush = DrawCanvas.Str2Brush("#3B3F49");
        public Brush BakcgroundBrush
        {
            get { return bakcgroundBrush; }
            set
            {
                this.bakcgroundBrush = value;
                this.InvalidateVisual();
            }
        }

        private Brush numBrush = DrawCanvas.Str2Brush("#828385");
        public Brush NumBrush
        {
            get { return numBrush; }
            set
            {
                this.numBrush = value;
                this.InvalidateVisual();
            }
        }

        private Brush lineBrush = DrawCanvas.Str2Brush("#636466");
        public Brush LineBrush
        {
            get { return lineBrush; }
            set
            {
                this.lineBrush = value;
                this.InvalidateVisual();
            }
        }

        private static Brush Str2Brush(string color)
        {
            return (Brush)new BrushConverter().ConvertFromString(color);
        }

        #endregion

        public DrawCanvas()
        {

        }

        #region  Overrid Methods

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            DrawBg(dc);
            DrawXNum(dc);
            DrawYNum(dc);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0)
            {
                minXCount *= 2;
            }
            else
            {
                minXCount = (int)Math.Ceiling(minXCount / 2.0);
            }
            this.InvalidateVisual();
        }

        #endregion

        #region Draw Actions

        private void DrawBg(DrawingContext dc)
        {
            dc.DrawRectangle(this.BakcgroundBrush, new Pen(Str2Brush("#787980"), 0.3),
                             new Rect(0, 0, this.DrawWidth, this.DrawHeight));
        }

        int minXCount = 4;
        private void DrawXNum(DrawingContext dc)
        {
            Point startPt = new Point(0, 10);
            for (int i = 0; i < 100; i += 5)
            {
                startPt.X += this.DrawWidth / minXCount;
                DrawText(dc, Int2TimeStr(i), startPt);
            }
        }

        const int yCount = 2;
        private void DrawYNum(DrawingContext dc)
        {
            Point startPt = new Point(10, this.DrawHeight);
            int maxY = 50;
            int step = maxY / yCount;

            for (int i = 0; i <= yCount; i++)
            {
                //上边和下边留空:+2
                startPt.Y = this.DrawHeight - (i + 1) * (this.DrawHeight / (yCount + 2));
                if (startPt.Y > 0)
                {
                    DrawText(dc, (step * i).ToString(), startPt);
                }
            }
        }

        private void DrawText(DrawingContext dc, string text, Point pos)
        {
            const int fontSize = 18;
            FontStyle fontStyle = FontStyles.Normal;
            FontWeight fontWeight = FontWeights.Medium;
            CultureInfo cultInfo = CultureInfo.GetCultureInfo("en-us");
            FontFamily fontFami = new FontFamily("宋体");
            FormattedText formattedText = new FormattedText(text, cultInfo, FlowDirection.LeftToRight,
                                                            new Typeface(fontFami, fontStyle, fontWeight, FontStretches.Normal),
                                                            fontSize, this.NumBrush);
            double txtWidth = formattedText.Width;
            double txtHeight = formattedText.Height;

            if (pos.X + txtWidth < 0 || pos.X + txtWidth > this.DrawWidth ||
                pos.Y + txtHeight < 0 || pos.Y + txtHeight > this.DrawHeight)
            {
                Debug.WriteLine($"当前字符:{text},位置:{pos},超过范围:{this.DrawWidth},{this.DrawHeight}");
                return;
            }
            dc.DrawText(formattedText, pos);
        }

        #endregion

        #region

        private string Int2TimeStr(int i)
        {
            string s = $"{(i / (60 * 60)).ToString().PadLeft(2, '0')}:{((i % (60 * 60)) / 60).ToString().PadLeft(2, '0')}:{(i % 60).ToString().PadLeft(2, '0')}";
            return s;
        }

        #endregion
    }
}
