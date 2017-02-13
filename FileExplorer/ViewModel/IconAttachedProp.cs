using System;
using System.Windows;

namespace FileExplorer.ViewModel
{
    public class IconAttachedProp : DependencyObject
    {
        public static readonly DependencyProperty DefaultIconProperty = DependencyProperty.RegisterAttached(
                                    "DefaultIcon", typeof(Uri), typeof(IconAttachedProp),
                                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static void SetDefaultIcon(UIElement element, Uri value)
        {
            element.SetValue(DefaultIconProperty, value);
        }
        public static Uri GetDefaultIcon(UIElement element)
        {
            return (Uri)element.GetValue(DefaultIconProperty);
        }

        public static readonly DependencyProperty CheckedIconProperty = DependencyProperty.RegisterAttached(
                                      "CheckedIcon", typeof(Uri), typeof(IconAttachedProp),
                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static void SetCheckedIcon(UIElement element, Uri value)
        {
            element.SetValue(CheckedIconProperty, value);
        }
        public static Uri GetCheckedIcon(UIElement element)
        {
            return (Uri)element.GetValue(CheckedIconProperty);
        }


        public static readonly DependencyProperty UncheckedIconProperty = DependencyProperty.RegisterAttached(
                                    "UncheckedIcon", typeof(Uri), typeof(IconAttachedProp),
                                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static void SetUncheckedIcon(UIElement element, Uri value)
        {
            element.SetValue(UncheckedIconProperty, value);
        }
        public static Uri GetUncheckedIcon(UIElement element)
        {
            return (Uri)element.GetValue(UncheckedIconProperty);
        }
    }
}
