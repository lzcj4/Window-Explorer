using System.Windows;
using System.Windows.Media;

namespace FileExplorer.Helper
{
    public class UIAttachedProp : DependencyObject
    {
        #region Icon Width  and Height 

        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.RegisterAttached("IconWidth", typeof(int), typeof(UIAttachedProp), new PropertyMetadata(16));

        public static int GetIconWidth(DependencyObject obj)
        {
            return (int)obj.GetValue(IconWidthProperty);
        }

        public static void SetIconWidth(DependencyObject obj, int value)
        {
            obj.SetValue(IconWidthProperty, value);
        }

        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.RegisterAttached("IconHeight", typeof(int), typeof(UIAttachedProp), new PropertyMetadata(16));

        public static int GetIconHeight(DependencyObject obj)
        {
            return (int)obj.GetValue(IconHeightProperty);
        }

        public static void SetIconHeight(DependencyObject obj, int value)
        {
            obj.SetValue(IconHeightProperty, value);
        }

        public static double GetRadius(DependencyObject obj)
        {
            return (float)obj.GetValue(RadiusProperty);
        }

        public static void SetRadius(DependencyObject obj, double value)
        {
            obj.SetValue(RadiusProperty, value);
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.RegisterAttached("Radius", typeof(double), typeof(UIAttachedProp), new PropertyMetadata(8.0));

        public static double GetRadiusX(DependencyObject obj)
        {
            return (float)obj.GetValue(RadiusXProperty);
        }

        public static void SetRadiusX(DependencyObject obj, double value)
        {
            obj.SetValue(RadiusXProperty, value);
        }

        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.RegisterAttached("RadiusX", typeof(double), typeof(UIAttachedProp), new PropertyMetadata(8.0));


        public static float GetRadiusY(DependencyObject obj)
        {
            return (float)obj.GetValue(RadiusYProperty);
        }

        public static void SetRadiusY(DependencyObject obj, double value)
        {
            obj.SetValue(RadiusYProperty, value);
        }

        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.RegisterAttached("RadiusY", typeof(double), typeof(UIAttachedProp), new PropertyMetadata(8.0));


        #endregion


        #region Text hint 

        public static string GetTextHint(DependencyObject obj)
        {
            return (string)obj.GetValue(TextHintProperty);
        }

        public static void SetTextHint(DependencyObject obj, string value)
        {
            obj.SetValue(TextHintProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextHint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextHintProperty =
            DependencyProperty.RegisterAttached("TextHint", typeof(string), typeof(UIAttachedProp), new PropertyMetadata(""));

        #endregion

        #region DefaultIcon / CheckedIcon / UncheckedIncon

        /// <summary>
        /// 默认图标
        /// </summary>
        public static readonly DependencyProperty DefaultIconProperty = DependencyProperty.RegisterAttached(
                                    "DefaultIcon", typeof(ImageSource), typeof(UIAttachedProp),
                                    new FrameworkPropertyMetadata(null));
        public static void SetDefaultIcon(UIElement element, ImageSource value)
        {
            element.SetValue(DefaultIconProperty, value);
        }
        public static ImageSource GetDefaultIcon(UIElement element)
        {
            return (ImageSource)element.GetValue(DefaultIconProperty);
        }

        /// <summary>
        /// 选中图标
        /// </summary>
        public static readonly DependencyProperty CheckedIconProperty = DependencyProperty.RegisterAttached(
                                      "CheckedIcon", typeof(ImageSource), typeof(UIAttachedProp),
                                      new FrameworkPropertyMetadata(null));
        public static void SetCheckedIcon(UIElement element, ImageSource value)
        {
            element.SetValue(CheckedIconProperty, value);
        }
        public static ImageSource GetCheckedIcon(UIElement element)
        {
            return (ImageSource)element.GetValue(CheckedIconProperty);
        }


        /// <summary>
        /// 反选图标
        /// </summary>
        public static readonly DependencyProperty UncheckedIconProperty = DependencyProperty.RegisterAttached(
                                    "UncheckedIcon", typeof(ImageSource), typeof(UIAttachedProp),
                                    new FrameworkPropertyMetadata(null));
        public static void SetUncheckedIcon(UIElement element, ImageSource value)
        {
            element.SetValue(UncheckedIconProperty, value);
        }
        public static ImageSource GetUncheckedIcon(UIElement element)
        {
            return (ImageSource)element.GetValue(UncheckedIconProperty);
        }


        /// <summary>
        /// MOUSE OVER ICON
        /// </summary>


        public static ImageSource GetMouseOverIcon(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(MouseOverIconProperty);
        }

        public static void SetMouseOverIcon(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(MouseOverIconProperty, value);
        }

        public static readonly DependencyProperty MouseOverIconProperty =
            DependencyProperty.RegisterAttached("MouseOverIcon", typeof(ImageSource), typeof(UIAttachedProp), new PropertyMetadata(null));


        /// <summary>
        /// MOUSE PRESSED ICON
        /// </summary>
        public static ImageSource GetPressedIcon(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(PressedIconProperty);
        }

        public static void SetPressedIcon(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(PressedIconProperty, value);
        }

        public static readonly DependencyProperty PressedIconProperty =
            DependencyProperty.RegisterAttached("PressedIcon", typeof(ImageSource), typeof(UIAttachedProp), new PropertyMetadata(null));


        #endregion
    }
}
