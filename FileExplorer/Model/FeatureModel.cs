using System.Collections.Generic;
using FileExplorer.Helper;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileExplorer.Model
{
    /// <summary>
    /// 特征项，含选择数据项
    /// </summary>
    public class FeatureGroup : ICloneable
    {
        public string Title { get; set; }
        public string ID { get; set; }

        private ObservableCollection<FeatureItem> items = new ObservableCollection<FeatureItem>();
        public IList<FeatureItem> Items { get { return items; } }

        public FeatureItem SelectedItem { get; set; }

        public FeatureGroup() { }
        public FeatureGroup(string title)
        {
            if (title.IsNullOrEmpty())
            {
                throw new InvalidOperationException();
            }
            this.Title = title;
        }

        public FeatureGroup(string title, string id) : this(title)
        {
            if (id.IsNullOrEmpty())
            {
                throw new InvalidOperationException();
            }
            this.ID = id;
        }

        public object Clone()
        {
            FeatureGroup result = new FeatureGroup();
            result.Title = this.Title;
            result.ID = this.ID;
            if (!this.SelectedItem.IsNull())
            {
                result.SelectedItem = this.SelectedItem;
            }

            this.Items.Clone().All(item => { result.items.Add(item); return true; });
            return result;
        }
    }
}
