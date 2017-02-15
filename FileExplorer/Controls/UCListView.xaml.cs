using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private void LoadData()
        {
            IList<TestModel> list = new List<TestModel>();

            for (int i = 0; i < 100; i++)
            {
                list.Add(new TestModel(i, "AAA"));
            }

            this.lvContent.ItemsSource = list;
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

            if(null != style)
            {
                this.lvContent.Style = style;
            }
        }

    }
}
