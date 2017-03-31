using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MongoMQTest
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        TestViewModel viewModel;
        public TestWindow()
        {
            InitializeComponent();
            viewModel = new TestViewModel();
            this.DataContext = viewModel;
           // viewModel.Switch();
        }
    }

    public class TestViewModel : ViewModelBase
    {
        private static readonly BitmapSource DefaultImage = new BitmapImage(new Uri(@"E:\aa.png"));
        private static readonly BitmapSource NewImage = new BitmapImage(new Uri(@"E:\bb.png"));

        private ImageSource image = DefaultImage;
        public ImageSource Image
        {
            get { return image; }
            set { SetProperty(ref image, value, "Image"); }
        }

        private BitmapSource img;
        private int count = 0;
        public void Switch()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(2 * 1000);
                    this.RunOnUIThreadAsync(() =>
                    {
                        if (count++ % 2 == 0)
                        {
                            img = new BitmapImage(new Uri(@"E:\aa.jpg")); 
                        }
                        else
                        {
                            img = new BitmapImage(new Uri(@"E:\aa.jpg"));
                        }
                        this.SwitchLoop();
                    });
                }     
            });
        }

        //WriteableBitmap target;
        private void SwitchLoop()
        {
            //NewImage = new BitmapImage(new Uri(@"E:\bb.png"));
            //byte[] bytes = null;

            //JpegBitmapEncoder jpegEncoder = new JpegBitmapEncoder();
            //jpegEncoder.Frames.Add(BitmapFrame.Create(NewImage));

            //using (var stream = new MemoryStream())
            //{
            //    jpegEncoder.Save(stream);
            //    bytes = stream.ToArray();
            //}

            // Calculate stride of source
            int stride = img.PixelWidth * (img.Format.BitsPerPixel + 7) / 8;

            // Create data array to hold source pixel data
            byte[] data = new byte[stride * img.PixelHeight];

            // Copy source image pixels to the data array
            img.CopyPixels(data, stride, 0);


            // Create WriteableBitmap to copy the pixel data to.      
            WriteableBitmap target = new WriteableBitmap(
                                  img.PixelWidth,
                                  img.PixelHeight,
                                  img.DpiX, img.DpiY,
                                  img.Format, null);

            // Write the pixel data to the WriteableBitmap.
            target.WritePixels(
              new Int32Rect(0, 0, img.PixelWidth, img.PixelHeight),
              data, stride, 0);

            // Set the WriteableBitmap as the source for the <Image> element 
            // in XAML so you can see the result of the copy
            Image = target;
        }
    }
}
