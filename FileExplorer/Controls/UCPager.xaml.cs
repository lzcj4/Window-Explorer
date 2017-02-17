using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FileExplorer.Controls
{
    public class PageEvnetArgs : EventArgs
    {
        public int Index { get; private set; }
        public int Len { get; private set; }

        public PageEvnetArgs(int index, int len)
        {
            if (index < 0 || len < 0)
            {
                throw new InvalidOperationException("页码值必须大于0");
            }

            this.Index = index;
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
        private const string STR_NEXT_PAGES = "...";

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
                 if (null != uc && uc.PageSize > 0 &&
                     newPageSize > 0 && uc.ItemLen > 0)
                 {
                     uc.TotalPages = uc.ItemLen / newPageSize + 1;
                     if (uc.TotalPages > 0)
                     {
                         uc.ReloadPageButtons();
                         uc.CheckDefaultPageButton();
                     }
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
                if (null != uc && uc.PageSize > 0 &&
                    newLen > 0 && uc.ItemLen > 0)
                {
                    uc.TotalPages = newLen / uc.PageSize + 1;
                    if (uc.TotalPages > 0)
                    {
                        uc.ReloadPageButtons();
                        uc.CheckDefaultPageButton();
                    }
                }
            }));


        /// <summary>
        /// 多少个页按钮，默认5个：(不含1个翻页键）
        /// </summary>
        public int ButtonSize
        {
            get { return (int)GetValue(ButtonSizeProperty); }
            set { SetValue(ButtonSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonSizeProperty =
            DependencyProperty.Register("ButtonSize", typeof(int), typeof(UCPager), new PropertyMetadata(5));


        public int TotalPages
        {
            get { return (int)GetValue(TotalPagesProperty); }
            set { SetValue(TotalPagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalPages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register("TotalPages", typeof(int), typeof(UCPager), new PropertyMetadata(0));

        #endregion


        #region Properties

        private Button PreviousBtn { get; set; }

        private Button NextBtn { get; set; }

        private int currentIndex = -1;
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
                        this.lastClickedBtn = btn;
                    }
                    this.PreviousBtn.IsEnabled = this.CurrentIndex != 0;
                    this.NextBtn.IsEnabled = this.CurrentIndex != this.TotalPages - 1;

                    this.RaisePageChanged(value, this.PageSize);
                }
            }
        }

        private IList<ToggleButton> pageButtons = new List<ToggleButton>();
        public IList<ToggleButton> PageButtons
        {
            get { return pageButtons; }
            set { pageButtons = value; }
        }

        private ToggleButton lastClickedBtn { get; set; }
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
                if (null != btn && (PageButtons.IndexOf(btn) == 0))
                {
                    int previewPage = this.CurrentIndex - this.ButtonSize;
                    if (previewPage >= 0)
                    {
                        this.isPrevious = true;
                        this.LoadNextPageButtons(previewPage);
                        this.CurrentIndex--;
                    }
                }
                else
                {
                    this.CurrentIndex--;
                }
            }
        }

        Style pageButtonStyle = null;
        private Style GetPageButtonStyle()
        {
            pageButtonStyle = pageButtonStyle ?? this.FindResource("PageButtonControlTemplate") as Style;
            return pageButtonStyle;
        }

        private void LoadPageButtons(int startIndex)
        {
            ToggleButton btn = null;
            for (int i = 0; i < this.ButtonSize; i++)
            {
                if (startIndex < this.TotalPages)
                {
                    btn = new ToggleButton();
                    btn.Tag = startIndex;
                    btn.Style = this.GetPageButtonStyle();
                    btn.Margin = buttonMargin;

                    btn.Click += (sender, e) =>
                    {
                        PageButtonClicked(((int)(sender as ButtonBase).Tag));
                    };

                    btn.Content = string.Format("{0,2}", startIndex + 1);
                    this.PageButtons.Add(btn);
                }

                startIndex++;
            }

            if (startIndex < this.TotalPages - 1)
            {
                btn = new ToggleButton();
                btn.Tag = startIndex;
                btn.Margin = buttonMargin;
                btn.Style = this.GetPageButtonStyle();
                btn.Click += (sender, e) =>
                {
                    this.isPrevious = false;
                    this.CurrentIndex = (int)(sender as ButtonBase).Tag;
                    LoadNextPageButtons(this.CurrentIndex);
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
                this.ReloadPageButtons(index);
            }
        }

        private void PageButtonClicked(int pageIndex)
        {
            this.UncheckLastBtn();
            this.CurrentIndex = pageIndex;
        }

        private void RemovePageButtons()
        {
            foreach (var btn in this.PageButtons)
            {
                // btn.Click -= PageButton_Click;

            }
            this.PageButtons.Clear();
        }

        private void ReloadPageButtons(int startIndex = 0)
        {
            this.RemovePageButtons();
            this.LoadPageButtons(startIndex);
        }

        private void CheckDefaultPageButton()
        {
            if (null != this.PageButtons && this.PageButtons.Count > 0)
            {
                this.CurrentIndex = 0;
            }
        }

        private void UncheckLastBtn()
        {
            if (this.lastClickedBtn != null)
            {
                this.lastClickedBtn.IsChecked = false;
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
                this.lastClickedBtn = this.PageButtons[index];
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

            if (index == this.TotalPages - 1)
            {
                len = this.ItemLen % this.PageSize;
            }

            if (null != this.PageChanged)
            {
                this.PageChanged(this, new PageEvnetArgs(index, len));
            }
        }
    }
}
