using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileExplorer.Model
{
    public class Category
    {
        public string Title { get; set; }
        public IList<FeatureItem> Features { get; set; }
    }
}
