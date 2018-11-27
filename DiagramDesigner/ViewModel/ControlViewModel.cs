using System.Windows.Media;

namespace DiagramDesigner.ViewModel
{
    public class ControlViewModel : ViewModelBase
    {
        private string text;

        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value, "Text"); }
        }

        private string controlId;

        public string ControlId
        {
            get { return controlId; }
            set { SetProperty(ref controlId, value, "ControlId"); }
        }

        private Brush fill = Brushes.Transparent;

        public Brush Fill
        {
            get { return fill; }
            set { SetProperty(ref fill, value, "Fill"); }
        }

        private Brush stroke = Brushes.Black;

        public Brush Stroke
        {
            get { return stroke; }
            set { SetProperty(ref stroke, value, "Stroke"); }
        }

    }
}
