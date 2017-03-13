using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FileExplorer.ViewModel
{
    public interface IMEFTest
    {
        void Test();
    }

    [Export(typeof(IMEFTest))]
    public class MEFTestA : IMEFTest
    {
        public void Test()
        {
            Debug.WriteLine("aaaaaaaaaaa");
        }
    }

    [Export(typeof(IMEFTest))]
    public class MEFTestB : IMEFTest
    {
        public void Test()
        {
            Debug.WriteLine("BBBBBBBBBBBB");
        }
    }
}
