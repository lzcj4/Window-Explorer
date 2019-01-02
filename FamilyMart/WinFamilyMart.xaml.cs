using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace WpfApplication3
{
    /// <summary>
    /// Interaction logic for WinFamilyMart.xaml
    /// </summary>
    public partial class WinFamilyMart : Window
    {
        public WinFamilyMart()
        {
            InitializeComponent();
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.PrintPreview, PrintSinglePage_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, Print_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New_Executed));
        }


        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            UCPage uc = new UCPage();
            TabItem tab = new TabItem() { Content = uc, Header = $"Sheet{this.tabGood.Items.Count + 1}" };
            this.tabGood.Items.Add(tab);
            this.tabGood.SelectedItem = tab;
        }


        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (true == printDialog.ShowDialog())
            {
                printDialog.PrintDocument(new ShapeDocumentPaginator(GetSheets()), "全家套餐");
            }
        }

        //打印当前页
        private void PrintSinglePage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem tab = this.tabGood.SelectedItem as TabItem;
            if (tab == null) return;
            UCPage page = tab.Content as UCPage;
            if (page == null) return;
            PrintDialog printDialog = new PrintDialog();

            if (true == printDialog.ShowDialog())
            {
                printDialog.PrintVisual(page, "全家套餐");
            }
        }

        //private void PrintPreview_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    System.Windows.Forms.PrintPreviewDialog printDialog = new System.Windows.Forms.PrintPreviewDialog() { Document;

        //    if (true == printDialog.ShowDialog())
        //    {
        //        printDialog.PrintDocument(new ShapeDocumentPaginator(GetSheets()), "全家套餐");
        //    }
        //}

        private IList<UCPage> GetSheets()
        {
            IList<UCPage> items = this.tabGood.Items
                                          .Cast<TabItem>()
                                          .Select(item => { return (item.Content as UCPage); })
                                          .ToList();
            return items;
        }

        public class ShapeDocumentPaginator : DocumentPaginator
        {
            class ShapeDocumentPaginatorSource : IDocumentPaginatorSource
            {
                private ShapeDocumentPaginator doc;
                public ShapeDocumentPaginatorSource(ShapeDocumentPaginator doc)
                {
                    this.doc = doc;
                }
                public DocumentPaginator DocumentPaginator { get { return this.doc; } }
            }

            private IList<UCPage> items;

            public ShapeDocumentPaginator(IList<UCPage> items)
            {
                this.items = items;
            }

            public override bool IsPageCountValid { get { return this.PageCount > 0; } }
            public override int PageCount { get { return this.items.Count; } }
            public override Size PageSize
            {
                get { return new Size(this.items[0].ActualWidth, this.items[0].ActualHeight); }
                set { throw new NotImplementedException(); }
            }
            public override IDocumentPaginatorSource Source { get { return new ShapeDocumentPaginatorSource(this); } }

            public override DocumentPage GetPage(int pageNumber)
            {
                return new DocumentPage(this.items[pageNumber], this.PageSize, Rect.Empty, Rect.Empty);
            }

        }

    }
}
