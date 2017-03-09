using FileExplorer.Model;
using System.Linq;

namespace FileExplorer.ViewModel
{
    public class FeatureModelViewModel : DataViewModelBase<CategoryViewModel, FeatureItem>
    {
        public FeatureModelViewModel(CategoryViewModel parent):base(parent)
        {
        }
        
        public override void Remove()
        {
            this.Parent.Remove(this);
        }

        public override void Edit()
        {
            EditWindow win = new EditWindow();
            win.Initial(this.Title, this.Items);
            if (win.ShowDialog().Value)
            {
                this.Items.Clear();
                win.Items.All(item => { this.Items.Add(item); return true; });
                this.Title = win.FeatureTitle;
            }
        }

    }
}
