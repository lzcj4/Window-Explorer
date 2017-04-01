using FileExplorer.Helper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FileExplorer.ViewTest
{
    public enum LayoutMode
    {
        /// <summary>
        /// 层叠
        /// </summary>
        Cascade = 0,
        /// <summary>
        /// 平辅
        /// </summary>
        Tile = 1
    }

    /// <summary>
    /// Interaction logic for UCImageGroupView.xaml
    /// </summary>
    public partial class UCImageGroupView : UserControl
    {
        #region DP

        public ObservableCollection<ImageItem> ItemSource
        {
            get { return (ObservableCollection<ImageItem>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(ObservableCollection<ImageItem>), typeof(UCImageGroupView),
                new PropertyMetadata(null, new PropertyChangedCallback(ItemSourceChanged)));

        private static void ItemSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs arg)
        {
            UCImageGroupView view = (obj as UCImageGroupView);
            view.RenderImage();
            view.ItemSource.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (ImageItem item in e.NewItems)
                    {
                        view.AddItem(item, false);
                    }
                }
            };
        }

        public int CascadeMaxCount
        {
            get { return (int)GetValue(CascadeMaxCountProperty); }
            set { SetValue(CascadeMaxCountProperty, value); }
        }
        public static readonly DependencyProperty CascadeMaxCountProperty =
            DependencyProperty.Register("CascadeMaxCount", typeof(int), typeof(UCImageGroupView), new PropertyMetadata(3,
                new PropertyChangedCallback((obj, args) =>
                {
                    (obj as UCImageGroupView).RenderImage();
                })));

        public int TileMaxCount
        {
            get { return (int)GetValue(TileMaxCountProperty); }
            set { SetValue(TileMaxCountProperty, value); }
        }
        public static readonly DependencyProperty TileMaxCountProperty =
            DependencyProperty.Register("TileMaxCount", typeof(int), typeof(UCImageGroupView), new PropertyMetadata(3,
                new PropertyChangedCallback((obj, args) =>
                {
                    (obj as UCImageGroupView).RenderImage();
                })));

        public int OffsetX
        {
            get { return (int)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(int), typeof(UCImageGroupView), new PropertyMetadata(30));

        public int OffsetY
        {
            get { return (int)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(int), typeof(UCImageGroupView), new PropertyMetadata(30));

        public LayoutMode LayoutMode
        {
            get { return (LayoutMode)GetValue(ViewModeProperty); }
            set { SetValue(ViewModeProperty, value); }
        }

        public static readonly DependencyProperty ViewModeProperty =
            DependencyProperty.Register("LayoutMode", typeof(LayoutMode), typeof(UCImageGroupView), new PropertyMetadata(LayoutMode.Cascade, (obj, args) =>
                 {
                     (obj as UCImageGroupView).RenderImage();
                 }));

        #endregion

        double imgPadding = 10;

        public UCImageGroupView()
        {
            InitializeComponent();
            this.canvas.PreviewMouseLeftButtonDown += Canvas_PreviewMouseLeftButtonDown;
            this.canvas.PreviewMouseMove += Canvas_PreviewMouseMove;
            this.canvas.PreviewMouseLeftButtonUp += Canvas_PreviewMouseLeftButtonUp;
            this.SizeChanged += UCImageGroupView_SizeChanged;
        }

        private IList<UCImageView> AllItems
        {
            get { return this.canvas.Children.Cast<UCImageView>().ToList(); }
        }

        private void RenderImage()
        {
            this.RenderImage(this.ItemSource);
        }

        private void RenderImage(ObservableCollection<ImageItem> list)
        {
            switch (this.LayoutMode)
            {
                case LayoutMode.Tile:
                    SetTileView(list);
                    break;
                case LayoutMode.Cascade:
                default:
                    SetCascadeView(list);
                    break;
            }
        }

        private void SetCascadeView(IList<ImageItem> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }

            canvas.Children.Clear();

            double currentOffsetX = this.OffsetX, currentOffsetY = this.OffsetY;
            double imgWidth = this.ActualWidth - this.OffsetX * (CascadeMaxCount + 1);
            double imgHeight = this.ActualHeight - this.OffsetY * (CascadeMaxCount + 1);
            int listCount = list.Count;

            for (int i = 0; i < this.CascadeMaxCount && i < listCount; i++)
            {
                ImageItem imageItem = list[i];
                UCImageView ucImage = new UCImageView();
                this.AddEvents(ucImage);
                ucImage.DataContext = imageItem;
                this.AddToUI(ucImage, new Rect(currentOffsetX, currentOffsetY, imgWidth, imgHeight));

                currentOffsetX += this.OffsetX;
                currentOffsetY += this.OffsetY;
            }
        }

        private void SetTileView(IList<ImageItem> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }
            canvas.Children.Clear();

            double imgWidth = (this.ActualWidth - this.OffsetX * 2 - (TileMaxCount - 1) * imgPadding) / TileMaxCount;
            double imgHeight = this.ActualHeight - this.OffsetY * 2;
            int listCount = list.Count;
            double currentOffsetX = this.OffsetX, currentOffsetY = this.OffsetY;

            for (int i = 0; i < this.TileMaxCount && i < listCount; i++)
            {
                ImageItem imageItem = list[i];
                UCImageView ucImage = new UCImageView();
                this.AddEvents(ucImage);
                ucImage.DataContext = imageItem;

                this.AddToUI(ucImage, new Rect(currentOffsetX, currentOffsetY, imgWidth, imgHeight));
                currentOffsetX += (imgWidth + imgPadding);
            }
        }

        /// <summary>
        /// 最小化被还原
        /// </summary>
        /// <param name="item"></param>
        public void Resume(ImageItem item)
        {
            if (item.IsNull() || !item.IsMin)
            {
                return;
            }

            UCImageView ucView = minList.FirstOrDefault(i => i.ViewModel == item);
            if (ucView.IsNull() || ucView.Tag.IsNull() || !(ucView.Tag is Rect) ||
                this.AllItems.Contains(ucView))
            {
                return;
            }

            this.AddToUI(ucView, (Rect)ucView.Tag);
            ucView.ViewModel.SizeMode = ImageSize.Normal;
            this.AddEvents(ucView);
            minList.Remove(ucView);
            maxList.Remove(ucView);
        }

        /// <summary>
        /// 双击加入新项目
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(ImageItem item, bool needAddToList = true)
        {
            if (item.IsNull() || this.AllItems.Any(i => i.ViewModel == item))
            {
                return;
            }
            int itemCount = this.ItemSource.Count + 1;
            double currentOffsetX = this.OffsetX * itemCount, currentOffsetY = this.OffsetY * itemCount;
            double imgWidth = this.ActualWidth - this.OffsetX * (CascadeMaxCount + 1);
            double imgHeight = this.ActualHeight - this.OffsetY * (CascadeMaxCount + 1);

            UCImageView ucImage = new UCImageView();
            this.AddEvents(ucImage);
            ucImage.DataContext = item;

            this.AddToUI(ucImage, new Rect(currentOffsetX, currentOffsetY, imgWidth, imgHeight));
            if (needAddToList)
            {
                this.ItemSource.Add(item);
            }
        }

        #region UCImageView events

        private void AddEvents(UCImageView ucView)
        {
            if (null == ucView)
            {
                return;
            }

            ucView.OnMinClick += UcView_OnMinClick;
            ucView.OnMaxClick += UcView_OnMaxClick;
            ucView.OnCloseClick += UcView_OnCloseClick;
        }

        private void RemoveEvents(UCImageView ucView)
        {
            if (null == ucView)
            {
                return;
            }

            ucView.OnMinClick -= UcView_OnMinClick;
            ucView.OnMaxClick -= UcView_OnMaxClick;
            ucView.OnCloseClick -= UcView_OnCloseClick;
        }

        private Rect GetImageRect(UCImageView ucView)
        {
            double left = Canvas.GetLeft(ucView);
            double top = Canvas.GetTop(ucView);
            Rect rect = new Rect(left, top, ucView.ActualWidth, ucView.ActualHeight);
            return rect;
        }

        IList<UCImageView> minList = new List<UCImageView>();
        private void UcView_OnMinClick(object sender, System.EventArgs e)
        {
            UCImageView ucView = sender as UCImageView;
            if (!ucView.ViewModel.IsMax)
            {
                ucView.Tag = this.GetImageRect(ucView);
            }
            ucView.ViewModel.SizeMode = ImageSize.Min;
            minList.Add(ucView);
            this.RemoveFromUI(ucView);
        }

        IList<UCImageView> maxList = new List<UCImageView>();

        private void UcView_OnMaxClick(object sender, System.EventArgs e)
        {
            if (!lastItem.IsNull())
            {
                Panel.SetZIndex(lastItem, zindexNormal);
            }

            UCImageView ucView = sender as UCImageView;
            Rect rect = new Rect();
            if (!ucView.ViewModel.IsMax)
            {
                ucView.Tag = GetImageRect(ucView);

                rect.X = imgPadding;
                rect.Y = imgPadding;
                rect.Width = this.ActualWidth - imgPadding * 2;
                rect.Height = this.ActualHeight - imgPadding * 2;
                ucView.ViewModel.SizeMode = ImageSize.Max;
                maxList.Add(ucView);
            }
            else
            {
                rect = (Rect)ucView.Tag;
                ucView.ViewModel.SizeMode = ImageSize.Normal;
                maxList.Remove(ucView);
            }

            lastItem = ucView;
            this.SetViewSize(ucView, rect, zindexTop);
        }

        private void SetViewSize(UCImageView ucView, Rect rect, int zindex = 0)
        {
            ucView.Width = rect.Width;
            ucView.Height = rect.Height;
            Canvas.SetLeft(ucView, rect.X);
            Canvas.SetTop(ucView, rect.Y);
            Panel.SetZIndex(ucView, zindex);
        }

        private void AddToUI(UCImageView ucView, Rect rect, int zindex = 0)
        {
            SetViewSize(ucView, rect, zindex);
            canvas.Children.Add(ucView);
        }

        private void UcView_OnCloseClick(object sender, System.EventArgs e)
        {
            UCImageView ucView = sender as UCImageView;
            minList.Remove(ucView);
            maxList.Remove(ucView);
            this.ItemSource.Remove(ucView.ViewModel);
            this.RemoveFromUI(ucView);
        }

        private void RemoveFromUI(UCImageView ucView)
        {
            this.canvas.Children.Remove(ucView);
            this.RemoveEvents(ucView);
        }

        #endregion

        #region Mouse Move

        private void SetAllUnselected(IList<ImageItem> list)
        {
            list.All(item => { item.IsSelected = false; return true; });
        }

        private void SetItemSelected(ImageItem item)
        {
            if (item.IsNull())
            {
                return;
            }

            if (this.IsCtrlPressed)
            {
                item.IsSelected = true;
                selectedList.Add(item);
                return;
            }

            SetAllUnselected(this.selectedList);
            this.selectedList.Clear();

            item.IsSelected = true;
            selectedList.Add(item);
        }

        private bool IsCtrlPressed
        {
            get
            {
                return Keyboard.IsKeyDown(Key.LeftCtrl) ||
                       Keyboard.IsKeyDown(Key.RightCtrl);
            }
        }

        UCImageView lastItem;
        UCImageView currentItem;
        Point clickPoint;
        const int zindexTop = 1;
        const int zindexNormal = 0;

        IList<ImageItem> selectedList = new List<ImageItem>();

        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clickPoint = e.GetPosition(canvas);
            HitTestResult hitItem = VisualTreeHelper.HitTest(canvas, clickPoint);
            if (hitItem.IsNull())
            {
                return;
            }

            UCImageView ucView = hitItem.VisualHit.TryFindParent<UCImageView>();
            if (ucView.IsNull())
            {
                if (!IsCtrlPressed)
                {
                    this.SetAllUnselected(this.ItemSource);
                }
                return;
            }

            if (!lastItem.IsNull() && !IsCtrlPressed)
            {
                lastItem.ViewModel.IsSelected = false;
                Panel.SetZIndex(lastItem, zindexNormal);
            }

            this.SetItemSelected(ucView.ViewModel);
            Panel.SetZIndex(ucView, zindexTop);

            Grid grid = hitItem.VisualHit as Grid;
            if (!grid.IsNull() && grid.Name == "gridTitle")
            {
                currentItem = ucView;
                clickPoint = e.GetPosition(canvas);
            }
            lastItem = ucView;
        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            currentItem = null;
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (currentItem.IsNull())
            {
                return;
            }

            Point newPoint = e.GetPosition(canvas);

            double left = Canvas.GetLeft(currentItem);
            double top = Canvas.GetTop(currentItem);

            double moveX = newPoint.X - clickPoint.X;
            double moveY = newPoint.Y - clickPoint.Y;
            clickPoint = newPoint;

            Canvas.SetLeft(currentItem, left + moveX);
            Canvas.SetTop(currentItem, top + moveY);
        }

        private void UCImageGroupView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double widthRate = e.NewSize.Width / e.PreviousSize.Width;
            double heightRate = e.NewSize.Height / e.PreviousSize.Height;
            switch (this.LayoutMode)
            {
                case LayoutMode.Tile:
                    TileSizeChanged(widthRate, heightRate);
                    break;
                case LayoutMode.Cascade:
                    CascadeSizeChanged(widthRate, heightRate);
                    break;
                default:
                    break;
            }
        }

        private void CascadeSizeChanged(double widthRate, double heightRate)
        {
            foreach (UCImageView item in canvas.Children)
            {
                item.Width *= widthRate;
                item.Height *= heightRate;

                Canvas.SetLeft(item, Canvas.GetLeft(item) * widthRate);
                Canvas.SetTop(item, Canvas.GetTop(item) * heightRate);
            }
        }

        private void TileSizeChanged(double widthRate, double heightRate)
        {
            foreach (UCImageView item in canvas.Children)
            {
                item.Width *= widthRate;
                item.Height *= heightRate;

                Canvas.SetLeft(item, Canvas.GetLeft(item) * widthRate);
            }
        }

        #endregion
    }
}
