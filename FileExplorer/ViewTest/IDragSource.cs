using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileExplorer.ViewTest
{
    public interface IDragSource
    {
        /// <summary>
        /// 开始处理拖拽：
        /// 1. 判断元素是否可以拖拽
        /// 2. 如果可以，则根据DragInfo的信息解析数据，填充DragInfo.Data，设置DragDropEffects
        /// 
        /// </summary>
        /// <param name="dragInfo"></param>
        void StartDrag(DragInfo dragInfo);
    }
}
