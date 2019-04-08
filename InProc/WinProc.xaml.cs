using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InProc
{
    public interface ITest
    {
        string Name { get; set; }
    }

    public class Test : ITest
    {
        string ITest.Name { get; set; }
    }

    /// <summary>
    /// Interaction logic for WinProc.xaml
    /// </summary>
    public partial class WinProc : Window
    {
        ITest t;
        public WinProc()
        {
            InitializeComponent();
            this.Loaded += Window_Loaded;
            this.SizeChanged += Window_SizeChanged;
            t = new Test() { };
            t.Name = "aa";
        }

        IntPtr otherWindow = IntPtr.Zero, thisWindow = IntPtr.Zero;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //This window's handle
            thisWindow = new WindowInteropHelper(this).Handle;

            //Open another program for embedding
            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo("notepad.exe");
            proc.Start();

            //Wait until otherWindow has been opened
            // await Task.Delay(500);
            //Thread.Sleep(500);

            //this.FindWindow("ConsoleWindowClass", true);
            while (otherWindow == IntPtr.Zero)
            {
                otherWindow = proc.MainWindowHandle;
            }

            //int winLog = WinHelper.GetWindowLong(otherWindow, WinHelper.GWL_STYLE);
            //this.GetForegroundWindow(otherWindow, true);

            //This bit removes the border of otherWindow and sets thisWindow as parent
            //I actually don't know what flags should be set, but simply setting the WS_VISIBLE flag seems to make window work, however borderless.
            WinHelper.SetWindowLong(otherWindow, WinHelper.GWL_STYLE, WinHelper.winStyle.WS_VISIBLE);
            WinHelper.SetParent(otherWindow, thisWindow);
            ArrangeWindows();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ArrangeWindows();
        }

        private void ArrangeWindows()
        {
            //Moves the otherWindow on top of childPlaceHolder
            Point topLeft = border1.TransformToAncestor(this).Transform(new Point(0, 0));
            Point bottomRight = border1.TransformToAncestor(this).Transform(new Point(border1.ActualWidth, border1.ActualHeight));
            WinHelper.MoveWindow(otherWindow, (int)topLeft.X, (int)topLeft.Y,
                                (int)bottomRight.X - (int)topLeft.X,
                                (int)bottomRight.Y - (int)topLeft.Y, true);
        }

        public int FindWindow(string windowName, bool wait)
        {
            int hWnd = WinHelper.FindWindow(windowName, null);
            while (wait && hWnd == 0)
            {
                System.Threading.Thread.Sleep(500);
                hWnd = WinHelper.FindWindow(windowName, null);
            }

            return hWnd;
        }

        public bool GetForegroundWindow(IntPtr winHanlde, bool wait)
        {
            bool result = false;
            while (wait)
            {
                if (winHanlde == WinHelper.GetForegroundWindow())
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            WinHelper.ShowWindow(otherWindow, WinHelper.SW_SHOW);
        }

        private void btnHidden_Click(object sender, RoutedEventArgs e)
        {
            WinHelper.ShowWindow(otherWindow, WinHelper.SW_HIDE);
        }

        private void TabControl_Selected(object sender, RoutedEventArgs e)
        {
            if ((sender as TabControl).SelectedIndex == 0)
            {
                WinHelper.ShowWindow(otherWindow, WinHelper.SW_SHOW);
            }
            else
            {
                WinHelper.ShowWindow(otherWindow, WinHelper.SW_HIDE);
            }
        }

        static class WinHelper
        {
            //Sets a window to be a child window of another window
            [DllImport("user32.dll")]
            public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

            [DllImport("user32.dll")]
            public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

            //Sets window attributes
            [DllImport("user32.dll")]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, winStyle dwNewLong);

            //Sets window attributes
            [DllImport("user32.dll")]
            public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int ShowWindow(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            public static int GWL_STYLE = -16;

            //https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-showwindow
            public static int SW_SHOW = 5;
            public static int SW_SHOWDEFAULT = 10;
            public static int SW_HIDE = 0;

            [Flags]
            public enum winStyle : int
            {
                WS_VISIBLE = 0x10000000,
                WS_CHILD = 0x40000000, //child window
                WS_BORDER = 0x00800000, //window with border
                WS_DLGFRAME = 0x00400000, //window with double border but no title
                WS_CAPTION = WS_BORDER | WS_DLGFRAME //window with a title bar
            }
        }
    }
}
