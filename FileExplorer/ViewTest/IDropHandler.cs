using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace FileExplorer.ViewTest
{
    /// <summary>
    /// 处理Drop事件
    /// </summary>
    public interface IDropHandler
    {
        /// <summary>
        /// 处理Drop Over消息。判断当前是否能够接受拖拽的数据源，同时根据需求修改鼠标拖拽效果
        /// </summary>
        /// <param name="dropInfo"></param>
        void OnDragOver(DropInfo dropInfo);

        /// <summary>
        /// 处理Drop事件
        /// </summary>
        /// <param name="dropInfo"></param>
        void OnDrop(DropInfo dropInfo);
    }
}
