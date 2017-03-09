using FileExplorer.Helper;
using FileExplorer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileExplorer.Controls
{
    /// <summary>
    /// Interaction logic for UCConfEdit.xaml
    /// </summary>
    public partial class UCConfEdit : UserControl
    {
        private IList<FeatureItem> items = new List<FeatureItem>();
        public IList<FeatureItem> Items { get { return items; } }

        public UCConfEdit()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                //this.txtUser.Focus();
                this.txtUser.ScrollToLine(0);
                this.txtUser.PreviewKeyUp += txtUser_PreviewKeyUp;
                this.txtSys.PreviewKeyUp += txtSys_PreviewKeyUp;
            };
            this.Unloaded += (sender, e) =>
            {
                this.txtUser.PreviewKeyUp -= txtUser_PreviewKeyUp;
                this.txtSys.PreviewKeyUp -= txtSys_PreviewKeyUp;
            };
        }

        private void txtSys_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                int currentLine = txtSys.GetLineIndexFromCharacterIndex(txtSys.CaretIndex);
                if (currentLine == this.txtSys.LineCount - 1)
                {
                    this.txtUser.Focus();
                    this.txtUser.CaretIndex = 0;
                    e.Handled = true;
                }
            }
        }

        private void txtUser_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                int currentLine = txtUser.GetLineIndexFromCharacterIndex(txtUser.CaretIndex);
                if (currentLine == 0)
                {
                    this.txtSys.Focus();
                    if (this.txtSys.Text.Length > 1)
                    {
                        this.txtSys.CaretIndex = this.txtSys.Text.Length + 1;
                    }
                    e.Handled = true;
                }
            }
        }

        char[] splitChars = new char[] { '\r', '\n' };

        public void SetItems(IEnumerable<FeatureItem> list)
        {
            if (list.IsNullOrEmpty())
            {
                return;
            }

            StringBuilder sysSb = new StringBuilder();
            StringBuilder userSb = new StringBuilder();
            int count = list.Count();
            foreach (var item in list)
            {
                if (item.IsSys)
                {
                    sysSb.AppendLine(item.Title);
                }
                else
                {
                    userSb.AppendLine(item.Title);
                }
            }

            if (sysSb.Length > 0)
            {
                string text = sysSb.ToString().TrimEnd(splitChars);
                txtSys.Text = text;
                txtSys.Visibility = Visibility.Visible;
            }

            if (userSb.Length > 0)
            {
                string text = userSb.ToString().TrimEnd(splitChars);
                txtUser.Text = text;
            }
        }

        public IList<FeatureItem> GetItems()
        {
            this.Items.Clear();
            this.GetItems(txtSys.Text, true);
            this.GetItems(txtUser.Text, true);
            return this.Items;
        }

        private void GetItems(string txt, bool isSys)
        {
            if (txt.IsNullOrEmpty())
            {
                return;
            }

            string[] lines = txt.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            if (lines.IsNullOrEmpty())
            {
                return;
            }

            foreach (var item in lines)
            {
                string tempValue = item.Trim();
                this.Items.Add(new FeatureItem(tempValue, tempValue, isSys));
            }
        }
    }
}
