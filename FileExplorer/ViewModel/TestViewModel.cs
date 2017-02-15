using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileExplorer.ViewModel
{
    public class TestViewModel : ViewModelBase
    {
        private string password;

        public string Password
        {
            get { return password; }
            set { this.SetProperty(ref password, value, "Password"); }
        }

    }
}
