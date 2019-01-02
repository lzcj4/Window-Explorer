using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for UCGood.xaml
    /// </summary>
    public partial class UCGood : UserControl
    {
        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(string), typeof(UCGood), new PropertyMetadata("./Resources/Images/1.jpg"));

        public string Price
        {
            get { return (string)GetValue(PriceProperty); }
            set { SetValue(PriceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Price.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PriceProperty =
            DependencyProperty.Register("Price", typeof(string), typeof(UCGood), new PropertyMetadata(""));


        public UCGood()
        {
            InitializeComponent();
        }

        private void Image_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            const string SupportedImageFilter = "图片文件|*.jpg;*.bmp;*.png;";
            var openDlg = new OpenFileDialog() { Multiselect = false, Filter = SupportedImageFilter };
            if (openDlg.ShowDialog() == true)
            {
                this.img.Source = new BitmapImage(new System.Uri(openDlg.FileName));
            }
        }
    }
}
