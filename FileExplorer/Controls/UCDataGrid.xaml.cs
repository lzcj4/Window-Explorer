using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileExplorer.Controls
{

    public class TestItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
    }

    /// <summary>
    /// Interaction logic for UCDataGrid.xaml
    /// </summary>
    public partial class UCDataGrid : UserControl
    {
        public UCDataGrid()
        {
            InitializeComponent();

            IList<TestItem> list = new List<TestItem> { new TestItem() { ID = 1, Title = "AA" },
                new TestItem() { ID = 2, Title = "BB" },
                new TestItem() { ID = 3, Title = "AA" },
                new TestItem() { ID = 4, Title = "AA" },
                new TestItem() { ID = 5, Title = "AA" },
                new TestItem() { ID = 6, Title = "AA" },
                new TestItem() { ID = 7, Title = "AA" },
            };
            dataGrid.ItemsSource = list;
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(dataGrid);
            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                if (null == row) continue;

                Rect bound = row.TransformToAncestor(dataGrid).TransformBounds(new Rect(0, 0, row.ActualWidth, row.ActualHeight));

                if (bound.Contains(p))
                {
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    return;
                }

            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (null == btn) { return; }

            TestItem ti = btn.DataContext as TestItem;
            if (null == ti) { return; }

            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromItem(ti) as DataGridRow;
            if (null == row) return;

            row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        }

    }
}
