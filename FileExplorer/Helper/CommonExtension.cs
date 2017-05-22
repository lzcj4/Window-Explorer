using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace FileExplorer.Helper
{
    public static class CommonExtension
    {
        public static bool IsNull(this object obj)
        {
            bool result = obj == null;
            return result;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            bool result = String.IsNullOrEmpty(str);
            return result;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            bool result = list == null || !list.Any();
            return result;
        }


        /// <summary>
        /// same as string.StartsWith, IgnoreCase
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns></returns>
        public static bool IsStartsWith(this string strA, string strB)
        {
            if (string.IsNullOrEmpty(strA) || string.IsNullOrEmpty(strB)) return false;
            return strA.StartsWith(strB, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// same as string.contains, IgnoreCase
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="testValue"></param>
        /// <returns></returns>
        public static bool IsContains(this string sourceValue, string testValue)
        {
            if (string.IsNullOrEmpty(sourceValue) || string.IsNullOrEmpty(testValue)) return false;
            return sourceValue.ToUpperInvariant().Contains(testValue.ToUpperInvariant());
        }

        public static string StrFormat(this string str, params string[] args)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }
            return string.Format(str, args);
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> list)
        {
            ObservableCollection<T> result = new ObservableCollection<T>();
            if (list.IsNullOrEmpty())
            {
                return result;
            }

            foreach (var item in list)
            {
                result.Add(item);
            }

            return result;
        }

        public static IList<T> Clone<T>(this IEnumerable<T> list) where T : class, ICloneable
        {
            IList<T> result = new List<T>();
            if (list.IsNullOrEmpty())
            {
                return result;
            }

            foreach (var item in list)
            {
                result.Add(item.Clone() as T);
            }

            return result;
        }

        public static T TryFindParent<T>(this DependencyObject current) where T : DependencyObject
        {
            if (current.IsNull())
            {
                return null;
            }
            DependencyObject parent = VisualTreeHelper.GetParent(current);

            if (parent.IsNull())
                return null;

            if (parent is T)
                return parent as T;
            else
                return TryFindParent<T>(parent);
        }

        public static T TryFindParent<T>(this DependencyObject current, string name) where T : FrameworkElement
        {
            if (current.IsNull())
            {
                return null;
            }
            DependencyObject parent = VisualTreeHelper.GetParent(current);

            if (parent.IsNull())
                return null;

            if ((parent is T) && (parent as T).Name == name)
                return parent as T;
            else
                return TryFindParent<T>(parent, name);
        }

        public static T TryFindChild<T>(this DependencyObject current) where T : DependencyObject
        {
            if (current.IsNull())
            {
                return null;
            }
            int count = VisualTreeHelper.GetChildrenCount(current);

            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(current, i);
                if (child.IsNull())
                    return null;

                if (child is T)
                    return child as T;
                else
                    return TryFindChild<T>(child);
            }
            return null;
        }

        public static T TryFindChild<T>(this DependencyObject current, string name) where T : FrameworkElement
        {
            if (current.IsNull())
            {
                return null;
            }
            int count = VisualTreeHelper.GetChildrenCount(current);

            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(current, i);
                if (child.IsNull())
                    return null;

                if ((child is T) && (child as T).Name == name)
                    return child as T;
                else
                    return TryFindChild<T>(child);
            }
            return null;
        }
    }
}
