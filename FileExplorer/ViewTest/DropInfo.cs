﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.ViewTest
{
    public class DropInfo
    {
        public object Data { get; private set; }
        public DragInfo DragInfo { get; private set; }
        public Type DropTargetAdorner { get; set; }
        public DragDropEffects Effects { get; set; }
        public int InsertIndex { get; private set; }
        public IEnumerable TargetCollection { get; private set; }
        public object TargetItem { get; private set; }
        public UIElement VisualTarget { get; private set; }
        public UIElement VisualTargetItem { get; private set; }
        public Orientation VisualTargetOrientation { get; private set; }

        public DragEventArgs DragEventArgs { get; private set; }

        public DropInfo(object sender, DragEventArgs e, DragInfo dragInfo, string dataFormat)
        {
            Data = (e.Data.GetDataPresent(dataFormat)) ? e.Data.GetData(dataFormat) : e.Data;
            this.DragEventArgs = e;
            DragInfo = dragInfo;

            VisualTarget = sender as UIElement;

            if (sender is ItemsControl)
            {
                ItemsControl itemsControl = (ItemsControl)sender;
                UIElement item = itemsControl.GetItemContainerAt(e.GetPosition(itemsControl));

                VisualTargetOrientation = itemsControl.GetItemsPanelOrientation();

                if (item != null)
                {
                    ItemsControl itemParent = ItemsControl.ItemsControlFromItemContainer(item);

                    InsertIndex = itemParent.ItemContainerGenerator.IndexFromContainer(item);
                    TargetCollection = itemParent.ItemsSource ?? itemParent.Items;
                    TargetItem = itemParent.ItemContainerGenerator.ItemFromContainer(item);
                    VisualTargetItem = item;

                    if (VisualTargetOrientation == Orientation.Vertical)
                    {
                        if (e.GetPosition(item).Y > item.RenderSize.Height / 2) InsertIndex++;
                    }
                    else
                    {
                        if (e.GetPosition(item).X > item.RenderSize.Width / 2) InsertIndex++;
                    }
                }
                else
                {
                    TargetCollection = itemsControl.ItemsSource ?? itemsControl.Items;
                    InsertIndex = itemsControl.Items.Count;
                }
            }
            else
            {
                Point point = e.GetPosition(VisualTarget);
                var hitElement = VisualTarget.InputHitTest(point) as UIElement;
                UCImageView ucView = hitElement.GetVisualAncestor<UCImageView>();
                if (hitElement is UCImageView || ucView != null)
                {
                    TargetItem = ucView;
                }
            }
        }
    }
}
