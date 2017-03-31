using System;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.ViewTest
{
    /// <summary>
    /// Interaction logic for UCImageView.xaml
    /// </summary>
    public partial class UCImageView : UserControl
    {
        public event EventHandler OnMinClick;
        public event EventHandler OnMaxClick;
        public event EventHandler OnCloseClick;

        public ImageItem ViewModel { get { return this.DataContext as ImageItem; } }

        public UCImageView()
        {
            InitializeComponent();
            
            this.MouseDoubleClick += UCImageView_MouseDoubleClick;
        }      

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            if (null != OnMinClick)
            {
                OnMinClick(this, EventArgs.Empty);
            }
        }

        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            if (null != OnMaxClick)
            {
                OnMaxClick(this, EventArgs.Empty);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (null != OnCloseClick)
            {
                OnCloseClick(this, EventArgs.Empty);
            }
        }

        private void UCImageView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (null != OnMaxClick)
            {
                OnMaxClick(this, EventArgs.Empty);
            }
        }

    }
}
