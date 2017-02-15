using System;
using System.Windows;

namespace FileExplorer.ViewModel
{
    public class UIAttachedProp : DependencyObject
    {
        #region Icon Width  and Height 

        public static int GetIconWidth(DependencyObject obj)
        {
            return (int)obj.GetValue(IconWidthProperty);
        }

        public static void SetIconWidth(DependencyObject obj, int value)
        {
            obj.SetValue(IconWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.RegisterAttached("IconWidth", typeof(int), typeof(UIAttachedProp), new PropertyMetadata(16));


        public static int GetIconHeight(DependencyObject obj)
        {
            return (int)obj.GetValue(IconHeightProperty);
        }

        public static void SetIconHeight(DependencyObject obj, int value)
        {
            obj.SetValue(IconHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.RegisterAttached("IconHeight", typeof(int), typeof(UIAttachedProp), new PropertyMetadata(16));



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
                                    "DefaultIcon", typeof(Uri), typeof(UIAttachedProp),
                                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static void SetDefaultIcon(UIElement element, Uri value)
        {
            element.SetValue(DefaultIconProperty, value);
        }
        public static Uri GetDefaultIcon(UIElement element)
        {
            return (Uri)element.GetValue(DefaultIconProperty);
        }

        /// <summary>
        /// 选中图标
        /// </summary>
        public static readonly DependencyProperty CheckedIconProperty = DependencyProperty.RegisterAttached(
                                      "CheckedIcon", typeof(Uri), typeof(UIAttachedProp),
                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static void SetCheckedIcon(UIElement element, Uri value)
        {
            element.SetValue(CheckedIconProperty, value);
        }
        public static Uri GetCheckedIcon(UIElement element)
        {
            return (Uri)element.GetValue(CheckedIconProperty);
        }


        /// <summary>
        /// 反选图标
        /// </summary>
        public static readonly DependencyProperty UncheckedIconProperty = DependencyProperty.RegisterAttached(
                                    "UncheckedIcon", typeof(Uri), typeof(UIAttachedProp),
                                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static void SetUncheckedIcon(UIElement element, Uri value)
        {
            element.SetValue(UncheckedIconProperty, value);
        }
        public static Uri GetUncheckedIcon(UIElement element)
        {
            return (Uri)element.GetValue(UncheckedIconProperty);
        }

        #endregion
    }
}
