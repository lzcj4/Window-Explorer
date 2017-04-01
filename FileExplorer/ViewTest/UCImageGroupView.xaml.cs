using FileExplorer.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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

        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(UCImageGroupView), new PropertyMetadata(30.0));

        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(UCImageGroupView), new PropertyMetadata(30.0));

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


        #region Cascade / Tile layout

        private void RenderImage()
        {
            this.RenderImage(this.ItemSource);
        }

        private void RenderImage(IList<ImageItem> list)
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

        private void ChangeSizeForCascade(double widthRate, double heightRate)
        {
            foreach (UCImageView item in canvas.Children)
            {
                item.Width *= widthRate;
                item.Height *= heightRate;

                Canvas.SetLeft(item, Canvas.GetLeft(item) * widthRate);
                Canvas.SetTop(item, Canvas.GetTop(item) * heightRate);
            }
        }

        private void ChangeSizeForTile(double widthRate, double heightRate)
        {
            foreach (UCImageView item in canvas.Children)
            {
                item.Width *= widthRate;
                item.Height *= heightRate;

                Canvas.SetLeft(item, Canvas.GetLeft(item) * widthRate);
            }
        }


        private void AddCascadeItem(ImageItem item, bool isAdd = true)
        {
            if (item.IsNull() || this.AllItems.Any(i => i.ViewModel == item))
            {
                return;
            }

            this.SelectedViewToBack();
            int itemCount = this.ItemSource.Count;
            double currentOffsetX = this.OffsetX * itemCount, currentOffsetY = this.OffsetY * itemCount;
            double imgWidth = this.ActualWidth - this.OffsetX * (CascadeMaxCount + 1);
            double imgHeight = this.ActualHeight - this.OffsetY * (CascadeMaxCount + 1);

            UCImageView ucImage = new UCImageView();
            this.AddEvents(ucImage);
            ucImage.DataContext = item;

            this.AddToUI(ucImage, new Rect(currentOffsetX, currentOffsetY, imgWidth, imgHeight));
            if (isAdd)
            {
                this.ItemSource.Add(item);
            }
        }

        private void AddTileItem(ImageItem item, bool isAdd = true)
        {
            if (item.IsNull() || this.AllItems.Any(i => i.ViewModel == item))
            {
                return;
            }

            this.SelectedViewToBack();
            int itemCount = this.ItemSource.Count;
            double currentOffsetX = this.ActualWidth / TileMaxCount + this.OffsetX * (itemCount - TileMaxCount);
            double currentOffsetY = this.OffsetY * (itemCount - TileMaxCount);
            double imgWidth = this.ActualWidth / TileMaxCount;
            double imgHeight = this.ActualHeight - this.OffsetY * 2;

            UCImageView ucImage = new UCImageView();
            this.AddEvents(ucImage);
            ucImage.DataContext = item;

            this.AddToUI(ucImage, new Rect(currentOffsetX, currentOffsetY, imgWidth, imgHeight));
            if (isAdd)
            {
                this.ItemSource.Add(item);
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

            this.SelectedViewToBack();

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
        public void AddItem(ImageItem item, bool isAdd = true)
        {
            switch (this.LayoutMode)
            {
                case LayoutMode.Tile:
                    AddTileItem(item, isAdd);
                    break;
                case LayoutMode.Cascade:
                default:
                    AddCascadeItem(item, isAdd);
                    break;
            }
        }

        private void SelectedViewToBack()
        {
            if (!selectedItem.IsNull())
            {
                Panel.SetZIndex(selectedItem, zindexNormal);
            }
        }

        #endregion

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
            this.SelectedViewToBack();

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

            selectedItem = ucView;
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

        UCImageView selectedItem;
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

                #region Resize Adorner

                this.RemoveResizeAdorner();

                #endregion

                return;
            }

            if (!selectedItem.IsNull() && !IsCtrlPressed)
            {
                selectedItem.ViewModel.IsSelected = false;
                Panel.SetZIndex(selectedItem, zindexNormal);
            }

            this.SetItemSelected(ucView.ViewModel);
            Panel.SetZIndex(ucView, zindexTop);

            Grid grid = hitItem.VisualHit as Grid;
            if (!grid.IsNull() && grid.Name == "gridTitle")
            {
                currentItem = ucView;
                clickPoint = e.GetPosition(canvas);
            }
            selectedItem = ucView;

            #region Resize Adorner 

            if (isResizeSelected)
            {
                isResizeSelected = false;
                this.RemoveResizeAdorner();
            }

            // If any element except canvas is clicked, 
            // assign the selected element and add the adorner
            if (e.Source != canvas)
            {
                isResizeDown = true;
                resizeStartPoint = e.GetPosition(canvas);

                resizeElement = e.Source as UIElement;

                resizeOriginalLeft = Canvas.GetLeft(resizeElement);
                resizeOriginalTop = Canvas.GetTop(resizeElement);

                adornerLayer = AdornerLayer.GetAdornerLayer(resizeElement);
                adornerLayer.Add(new ResizingAdorner(resizeElement));
                isResizeSelected = true;
                // e.Handled = true;
            }

            #endregion
        }

        #region Resize Adorner 

        AdornerLayer adornerLayer;

        bool isResizeDown;
        bool isResizeDragging;
        bool isResizeSelected = false;
        UIElement resizeElement = null;

        Point resizeStartPoint;
        private double resizeOriginalLeft;
        private double resizeOriginalTop;

        private void RemoveResizeAdorner()
        {
            if (resizeElement != null)
            {
                // Remove the adorner from the selected element
                var ad = adornerLayer.GetAdorners(resizeElement);
                if (!ad.IsNullOrEmpty())
                {
                    adornerLayer.Remove(ad[0]);
                    resizeElement = null;
                }

            }
        }

        #endregion

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            currentItem = null;
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ///可能鼠标在窗体外被放开
            if (currentItem.IsNull() || Mouse.LeftButton != MouseButtonState.Pressed)
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

            #region Resize Adorner 

            if (isResizeDown)
            {
                if ((isResizeDragging == false) &&
                    ((Math.Abs(e.GetPosition(canvas).X - resizeStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(canvas).Y - resizeStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                    isResizeDragging = true;

                if (isResizeDragging)
                {
                    Point position = Mouse.GetPosition(canvas);
                    Canvas.SetTop(resizeElement, position.Y - (resizeStartPoint.Y - resizeOriginalTop));
                    Canvas.SetLeft(resizeElement, position.X - (resizeStartPoint.X - resizeOriginalLeft));
                }
            }

            #endregion
        }

        private void UCImageGroupView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double widthRate = e.PreviousSize.Width == 0 ? 1 : e.NewSize.Width / e.PreviousSize.Width;
            double heightRate = e.PreviousSize.Height == 0 ? 1 : e.NewSize.Height / e.PreviousSize.Height;

            OffsetX *= widthRate;
            OffsetY *= heightRate;

            switch (this.LayoutMode)
            {
                case LayoutMode.Tile:
                    ChangeSizeForTile(widthRate, heightRate);
                    break;
                case LayoutMode.Cascade:
                default:
                    ChangeSizeForCascade(widthRate, heightRate);
                    break;
            }
        }

        #endregion
    }
}
