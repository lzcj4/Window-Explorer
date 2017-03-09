using System;

namespace FileExplorer.Model
{
    /// <summary>
    /// 特征实际数据项
    /// </summary>
    public class FeatureItem : ICloneable
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public bool IsSys { get; set; }

        public FeatureItem() { }
        public FeatureItem(string title, string value, bool isSys = false)
        {
            this.Title = title;
            this.Value = value;
        }

        public object Clone()
        {
            FeatureItem result = new FeatureItem();
            result.Title = this.Title;
            result.Value = this.Value;
            result.IsSys = this.IsSys;
            return result;
        }
    }
}
