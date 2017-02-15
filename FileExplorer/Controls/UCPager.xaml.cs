using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileExplorer.Controls
{

    public class PageEvnetArgs : EventArgs
    {
        public int StartIndex { get; private set; }
        public int Len { get; private set; }

        public PageEvnetArgs(int index, int len)
        {
            if (index < 0 || len < 0)
            {
                throw new InvalidOperationException("页码值必须大于0");
            }

            this.StartIndex = index;
            this.Len = len;
        }
    }

    /// <summary>
    /// Interaction logic for UCPager.xaml
    /// </summary>
    public partial class UCPager : UserControl
    {

        public event EventHandler<PageEvnetArgs> PageChanged;

        private const string STR_PREVIOUS_BUTTON = "上一页";
        private const string STR_NEXT_BUTTON = "下一页";
        private const string STR_NEXT_PAGES = "*";

        #region DP

        /// <summary>
        /// 每页显示多少条
        /// </summary>
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(int), typeof(UCPager), new PropertyMetadata(10, (d, e) =>
             {
                 int newPageSize = (int)e.NewValue;
                 UCPager uc = d as UCPager;
                 if (null != uc && uc.PageSize > 0 && newPageSize > 0)
                 {
                     uc.TotalPages = uc.ItemLen / newPageSize + 1;
                     uc.ReloadPageButtons();
                 }
             }));



        /// <summary>
        /// 总共多少条记录
        /// </summary>
        public int ItemLen
        {
            get { return (int)GetValue(ItemLenProperty); }
            set { SetValue(ItemLenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemLen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemLenProperty =
            DependencyProperty.Register("ItemLen", typeof(int), typeof(UCPager), new PropertyMetadata(0, (d, e) =>
            {
                int newLen = (int)e.NewValue;
                UCPager uc = d as UCPager;
                if (null != uc && uc.PageSize > 0 && newLen > 0)
                {
                    uc.TotalPages = newLen / uc.PageSize + 1;
                    uc.ReloadPageButtons();
                }
            }));



        /// <summary>
        /// 多少个页按钮，默认5个：(不含1个翻页键）
        /// </summary>
        public int ButtonLen
        {
            get { return (int)GetValue(ButtonLenProperty); }
            set { SetValue(ButtonLenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonLenProperty =
            DependencyProperty.Register("ButtonLen", typeof(int), typeof(UCPager), new PropertyMetadata(5));


        #endregion


        #region Properties

        private Button PreviousBtn { get; set; }

        private Button NextBtn { get; set; }

        private int currentIndex = 0;
        private int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                if (currentIndex != value)
                {
                    this.currentIndex = value;
                    ToggleButton btn = PageButtons.FirstOrDefault(item => ((int)item.Tag) == value);
                    if (null != btn)
                    {
                        this.UncheckLastBtn();
                        btn.IsChecked = true;
                        this.LastPageBtn = btn;
                    }
                    this.PreviousBtn.IsEnabled = this.CurrentIndex != 0;
                    this.NextBtn.IsEnabled = this.CurrentIndex != this.TotalPages - 1;
                }
            }
        }

        private int TotalPages { get; set; }

        private IList<ToggleButton> pageButtons = new List<ToggleButton>();

        public IList<ToggleButton> PageButtons
        {
            get { return pageButtons; }
            set { pageButtons = value; }
        }
        private ToggleButton LastPageBtn { get; set; }
        private Thickness buttonMargin = new Thickness(5, 0, 0, 0);

        #endregion

        public UCPager()
        {
            InitializeComponent();
            this.InitialPages();
            this.LoadNavigateButtons();
        }

        private void InitialPages()
        {
            if (this.ItemLen < 0 || this.PageSize < 0)
            {
                return;
            }
            this.TotalPages = this.ItemLen / this.PageSize;
        }

        private void LoadNavigateButtons()
        {
            if (null == PreviousBtn)
            {
                PreviousBtn = new Button();
                PreviousBtn.Content = UCPager.STR_PREVIOUS_BUTTON;
                PreviousBtn.Click += PreviousBtn_Click;
            }

            if (null == NextBtn)
            {
                NextBtn = new Button();
                NextBtn.Content = UCPager.STR_NEXT_BUTTON;
                NextBtn.Margin = buttonMargin;
                NextBtn.Click += NextBtn_Click;
            }
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.CurrentIndex < this.TotalPages - 1)
            {
                this.isPrevious = false;
                ToggleButton btn = PageButtons.FirstOrDefault(item => ((int)item.Tag) == this.CurrentIndex);
                if (null != btn && (PageButtons.IndexOf(btn) == this.GetLastPageButtonIndex()))
                {
                    this.LoadNextPageButtons(++this.CurrentIndex);
                }
                else
                {
                    this.CurrentIndex++;
                }
            }


        }

        private bool isPrevious = false;
        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.CurrentIndex > 0)
            {
                ToggleButton btn = PageButtons.FirstOrDefault(item => ((int)item.Tag) == this.CurrentIndex);
                if (null != btn && (PageButtons.IndexOf(btn) == GetLastPageButtonIndex()))
                {
                    int previewPage = this.CurrentIndex - this.ButtonLen;
                    if (previewPage >= 0)
                    {
                        this.isPrevious = true;
                        this.LoadNextPageButtons(previewPage);
                    }
                }
                else
                {
                    this.CurrentIndex--;
                }
            }
        }

        private void LoadPageButtons(bool isNext = true)
        {
            int pageIndex = this.CurrentIndex;

            ToggleButton btn = null;
            for (int i = 0; i < this.ButtonLen; i++)
            {
                if (pageIndex < this.TotalPages)
                {
                    btn = new ToggleButton();
                    btn.Tag = pageIndex;
                    btn.Margin = buttonMargin;

                    btn.Click += (sender, e) =>
                    {
                        PageButtonClicked(((int)(sender as ButtonBase).Tag));
                    };

                    btn.Content = string.Format("{0,2}", pageIndex + 1);
                    this.PageButtons.Add(btn);
                }

                pageIndex++;
            }

            if (pageIndex < this.TotalPages - 1)
            {
                btn = new ToggleButton();
                btn.Tag = pageIndex;
                btn.Margin = buttonMargin;
                btn.Click += (sender, e) =>
                {
                    this.isPrevious = false;
                    LoadNextPageButtons((int)(sender as ButtonBase).Tag);
                };
                btn.Content = btn.Content = string.Format("{0,2}", UCPager.STR_NEXT_PAGES);
                this.PageButtons.Add(btn);
            }

            this.ShowButtons();
        }

        private void LoadNextPageButtons(int index)
        {
            if (index != this.TotalPages - 1)
            {
                this.CurrentIndex = index;
                this.ReloadPageButtons();
            }
        }

        private void PageButtonClicked(int pageIndex)
        {
            this.UncheckLastBtn();
            this.CurrentIndex = pageIndex;
            int len = this.PageSize;
            if (pageIndex == this.TotalPages - 1)
            {
                len = this.ItemLen % this.PageSize;
                this.RaisePageChanged(pageIndex, len);
            }
        }

        private void RemovePageButtons()
        {
            foreach (var btn in this.PageButtons)
            {
                // btn.Click -= PageButton_Click;

            }
            this.PageButtons.Clear();
        }

        private void ReloadPageButtons(bool isNext = true)
        {
            this.RemovePageButtons();
            this.LoadPageButtons(isNext);
        }

        private void UncheckLastBtn()
        {
            if (this.LastPageBtn != null)
            {
                this.LastPageBtn.IsChecked = false;
            }
        }

        private int GetLastPageButtonIndex()
        {
            int index = this.PageButtons.Count > 1 ? this.PageButtons.Count - 2 : 0;
            return index;
        }


        private void ShowButtons()
        {
            this.panelPager.Children.Clear();
            this.panelPager.Children.Add(this.PreviousBtn);

            if (this.PageButtons.Count > 0)
            {
                foreach (var btn in this.PageButtons)
                {
                    this.panelPager.Children.Add(btn);
                }
                int index = this.PageButtons.Count > 1 && this.isPrevious ? this.PageButtons.Count - 2 : 0;
                this.PageButtons[index].IsChecked = true;
                this.LastPageBtn = this.PageButtons[index];
            }

            this.panelPager.Children.Add(this.NextBtn);
            this.PreviousBtn.IsEnabled = this.CurrentIndex != 0;
            this.NextBtn.IsEnabled = this.CurrentIndex != this.TotalPages - 1;
        }

        private void RaisePageChanged(int index, int len)
        {
            if (index < 0 || len < 0)
            {
                throw new InvalidOperationException();
            }

            if (null != this.PageChanged)
            {
                this.PageChanged(this, new PageEvnetArgs(index, len));
            }
        }
    }
}
