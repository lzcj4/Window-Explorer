using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Forms = System.Windows.Forms;

namespace FileExplorer
{
    public class TestModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }


        private IList<TestModel> items = new List<TestModel>();

        public IList<TestModel> Items
        {
            get { return items; }
            set { items = value; }
        }

    }


    /// <summary>
    /// Interaction logic for TestDataGrid.xaml
    /// </summary>
    public partial class TestDataGrid : Window
    {
        public TestDataGrid()
        {
            InitializeComponent();

            LoadData();
            LoadHotkeysData();
        }

        private void LoadData()
        {
            IList<TestModel> list = new List<TestModel>();
            string cp = string.Empty;
            for (int k = 0; k < 20; k++)
            {
                cp += "A";
            }

            for (int i = 0; i < 100; i++)
            {
                TestModel tp = new TestModel() { ID = i, Name = string.Format("Parent :{0}", i), Comment = cp };
                string cc = string.Empty;
                for (int k = 0; k < 20; k++)
                {
                    cc += "B";
                }

                for (int j = i * 100; j < i * 100 + 100; j++)
                {

                    TestModel tc = new TestModel() { ID = j, Name = string.Format("Child :{0}", j), Comment = cc };
                    tp.Items.Add(tc);
                }
                list.Add(tp);

            }

            dataGrid.ItemsSource = list;
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = null;
            FrameworkElement currentItem = e.OriginalSource as FrameworkElement;
            while (true)
            {
                if (currentItem == null)
                {
                    return;
                }
                currentItem = VisualTreeHelper.GetParent(currentItem) as FrameworkElement;
                if (currentItem is DataGridRow)
                {
                    row = currentItem as DataGridRow;
                    break;
                }
            }
            row.DetailsVisibility = Visibility.Visible;
        }

        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
        }

        private void LoadHotkeysData()
        {
            IList<KeyItem> list = new List<KeyItem>();
            list.Add(new KeyItem() { Title = "aaa", Keys = "Ctrl+A" });
            list.Add(new KeyItem() { Title = "bbb", Keys = "Ctrl+B" });
            list.Add(new KeyItem() { Title = "ccc", Keys = "Ctrl+C" });
            list.Add(new KeyItem() { Title = "ddd", Keys = "Ctrl+D" });
            lvKeys.ItemsSource = list;
        }
    }


    class KeyItem : ViewModel.ViewModelBase
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value, "Title"); }
        }

        private string keys;
        public string Keys
        {
            get { return keys; }
            set { SetProperty(ref keys, value, "Keys"); }
        }

    }

}
