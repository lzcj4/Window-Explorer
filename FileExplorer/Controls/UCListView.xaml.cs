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
        public UCListView()
        {
            InitializeComponent();
            LoadData();
        }


        private IList<TestModel> internalList;
        private void LoadData()
        {
            int len = 100;
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
    }
}
