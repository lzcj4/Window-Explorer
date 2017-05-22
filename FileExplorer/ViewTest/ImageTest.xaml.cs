using FileExplorer.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileExplorer.ViewTest
{
    [Export]
    /// <summary>
    /// Interaction logic for ImageTest.xaml
    /// </summary>
    public partial class ImageTest : Window
    {
        [Import]
        public Lazy<CategoryGroupViewModel> ViewModel { get; set; }
        public ImageTest()
        {
            InitializeComponent();
            //this.Loaded += ImageTest_Loaded;
        }

        private void ImageTest_Loaded(object sender, RoutedEventArgs e)
        {
            Bitmap bmp = new Bitmap(@"E:\1.bmp"); // Lock the bitmap's bits. 
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            // Get the address of the first line. 
            IntPtr ptr = bmpData.Scan0;     // Declare an array to hold the bytes of the bitmap. 
                                            // This code is specific to a bitmap with 24 bits per pixels. 
            int bytes = bmp.Width * bmp.Height * 3;
            byte[] rgbValues = new byte[bytes];     // Copy the RGB values into the array. 
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);     // Set every red value to 255. 
            for (int counter = 0; counter < rgbValues.Length; counter += 3)
                rgbValues[counter] = 255;
            // Copy the RGB values back to the bitmap 
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);     // Unlock the bits. 
            bmp.UnlockBits(bmpData);     // Draw the modified image. 

            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();



            imgB.Source = image;

        }
    }
}
