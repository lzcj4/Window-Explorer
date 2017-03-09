using FileExplorer.Helper;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileExplorer.Controls
{
    public class UCConfBase : UserControl
    {
        #region DP

        public string FeatureTitle
        {
            get { return (string)GetValue(ItemTitleProperty); }
            set { SetValue(ItemTitleProperty, value); }
        }

        public static readonly DependencyProperty ItemTitleProperty =
            DependencyProperty.Register("FeatureTitle", typeof(string), typeof(UCConfBase), new PropertyMetadata(null));

        #endregion

        public event EventHandler OnEdit;
        public event EventHandler OnDelete;

        protected void RaiseOnEdit()
        {
            if (!this.OnEdit.IsNull())
            {
                this.OnEdit(this, EventArgs.Empty);
            }
        }

        protected void RaiseOnDelete()
        {
            if (!this.OnDelete.IsNull())
            {
                this.OnDelete(this, EventArgs.Empty);
            }
        }

        protected void imgEdit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.RaiseOnEdit();
        }

        protected void imgDel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.RaiseOnDelete();
        }
    }
}
