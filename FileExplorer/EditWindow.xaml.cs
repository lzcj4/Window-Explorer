using FileExplorer.Controls;
using FileExplorer.Model;
using System.Collections.Generic;
using System.Windows;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public string FeatureTitle
        {
            get { return txtTitle.Text; }
        }

        public IList<FeatureItem> Items
        {
            get { return txtItems.Items; }
        }

        public EditWindow()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => { txtTitle.Focus(); };
        }

        public void Initial(string title,FeatureItem item)
        {
            this.Initial(title,new FeatureItem[] { item });
        }

        public void Initial(string title,IEnumerable<FeatureItem> list)
        {
            txtTitle.Text = title;
            txtItems.SetItems(list);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            txtItems.GetItems();
            this.Close();
        }
    }
}
