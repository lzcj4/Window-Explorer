using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.Controls
{

    public class TestModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }

        public TestModel(int id, string name)
        {
            this.ID = id;
            this.Name = name;
        }
    }

    /// <summary>
    /// Interaction logic for UCListView.xaml
    /// </summary>
    public partial class UCListView : UserControl
    {
        #region DP
        public static int GetRows(DependencyObject obj)
        {
            return (int)obj.GetValue(RowsProperty);
        }

        public static void SetRows(DependencyObject obj, int value)
        {
            obj.SetValue(RowsProperty, value);
        }
        
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.RegisterAttached("Rows", typeof(int), typeof(UCListView), new PropertyMetadata(5));


        public static int GetColumns(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnsProperty);
        }

        public static void SetColumns(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnsProperty, value);
        }
        
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached("Columns", typeof(int), typeof(UCListView), new PropertyMetadata(3));



        public static int GetImageWidth(DependencyObject obj)
        {
            return (int)obj.GetValue(ImageWidthProperty);
        }

        public static void SetImageWidth(DependencyObject obj, int value)
        {
            obj.SetValue(ImageWidthProperty, value);
        }
        
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.RegisterAttached("ImageWidth", typeof(int), typeof(UCListView), new PropertyMetadata(80));

        
        public static int GetImageHeight(DependencyObject obj)
        {
            return (int)obj.GetValue(ImageHeightProperty);
        }

        public static void SetImageHeight(DependencyObject obj, int value)
        {
            obj.SetValue(ImageHeightProperty, value);
        }
        
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.RegisterAttached("ImageHeight", typeof(int), typeof(UCListView), new PropertyMetadata(50));


        #endregion


        public UCListView()
        {
            InitializeComponent();
            LoadData();
        }


        private IList<TestModel> internalList;
        private void LoadData()
        {
            int len = 223;
            internalList = new List<TestModel>();
            for (int i = 0; i < len; i++)
            {
                internalList.Add(new TestModel(i + 1, "AAA"));
            }
            ucPager.ItemLen = len;

        }

        bool isListView = false;
        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            this.isListView = !this.isListView;
            if (this.isListView)
            {
                SetListViewStyle("ListViewListStyle");
            }
            else
            {
                SetListViewStyle("ImageViewListStyle");
            }
        }

        private void SetListViewStyle(string name)
        {
            Style style = null;
            if (this.Resources.Contains(name))
            {
                style = this.Resources[name] as Style;
            }

            if (null != style)
            {
                this.lvContent.Style = style;
            }
        }

        private void UCPager_PageChanged(object sender, PageEvnetArgs e)
        {
            int index = e.Index;
            int len = e.Len;
            if (index >= 0 && len > 0)
            {
                var tempItems = this.internalList.Skip(index * ucPager.PageSize).Take(len);
                this.lvContent.ItemsSource = tempItems;
            }
        }

        private void btnBig_Click(object sender, RoutedEventArgs e)
        {
            UCListView.SetRows(lvContent, 5);
            UCListView.SetColumns(lvContent, 3);

            int imgWidth = (int)(lvContent.ActualWidth / 5);
            int imgHeight = (int)(lvContent.ActualWidth / 3);

            UCListView.SetImageWidth(lvContent, imgWidth);
            UCListView.SetImageHeight(lvContent, imgHeight);
        }

        private void btnSmall_Click(object sender, RoutedEventArgs e)
        {
            UCListView.SetRows(lvContent, 3);
            UCListView.SetColumns(lvContent, 5);
            int imgWidth = (int)(lvContent.ActualWidth / 3);
            int imgHeight = (int)(lvContent.ActualWidth / 5);

            UCListView.SetImageWidth(lvContent, imgWidth);
            UCListView.SetImageHeight(lvContent, imgHeight);
        }
    }
}
