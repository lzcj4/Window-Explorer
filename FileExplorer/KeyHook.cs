using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace FileExplorer
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="nCode"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    public delegate int HookProc(int nCode, int wParam, IntPtr lParam);

    /// <summary>
    /// 键盘钩子
    /// </summary>
    public class KeyHook
    {
        #region 常量

        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;
        public const int WH_KEYBOARD_LL = 13;

        #endregion

        #region 变量

        private static int hHook = 0;
        private HookProc KeyBoardHookProcedure;
        #endregion

        #region 事件

        /// <summary>
        /// 当按下键盘按键时发生
        /// </summary>
        public event KeyEventHandler KeyDownEvent;

        /// <summary>
        /// 当抬起键盘按键时发生
        /// </summary>
        public event KeyEventHandler KeyUpEvent;

        #endregion

        #region 激发事件的参数

        /// <summary>
        /// 激发KeyDownEvent事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">包含事件数据的 System.Windows.Forms.KeyEventArgs</param>
        private void OnKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (this.KeyDownEvent != null)
            {
                this.KeyDownEvent(sender, e);
            }
        }

        /// <summary>
        /// 激发KeyUpEvent事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">包含事件数据的 System.Windows.Forms.KeyEventArgs</param>
        private void OnKeyUpEvent(object sender, KeyEventArgs e)
        {
            if (this.KeyUpEvent != null)
            {
                this.KeyUpEvent(sender, e);
            }
        }
        #endregion

        #region 方法

        /// <summary>
        /// 安装键盘钩子
        /// </summary>
        public void Install_Hook()
        {
            if (hHook == 0)
            {
                KeyBoardHookProcedure = new HookProc(KeyBoardHookProc);
                hHook = WinApi.SetWindowsHookEx(
                    HookType.WH_KEYBORARD_LL,
                    KeyBoardHookProcedure,
                    WinApi.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName),
                    0);

                //如果设置钩子失败
                if (hHook == 0)
                    Uninstall_Hook();
            }

        }

        /// <summary>
        /// 卸载键盘钩子
        /// </summary>
        public void Uninstall_Hook()
        {
            if (hHook != 0)
            {
                int result = WinApi.UnhookWindowsHookEx(hHook);
                hHook = 0;
                if (result == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    //  throw new Win32Exception("KeyHook.Uninstall_Hook()->" + WinApi.GetLastErrorString(errorCode));
                }
            }
        }

        //private int KeyBoardHookProc(int nCode, int wParam, IntPtr lParam)
        //{
        //    if (nCode >= 0)
        //    {
        //        KeyBoardHookStruct kbh = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));
        //        Keys key = (Keys)Enum.Parse(typeof(Keys), kbh.vkCode.ToString());
        //        if (kbh.flags == 0)
        //        {
        //            //这里写按下后做什么
        //            KeyEventArgs e = new KeyEventArgs(key);
        //            this.OnKeyDownEvent(this, e);
        //        }
        //        else if (kbh.flags == 128)
        //        {
        //            //放开后做什么
        //            KeyEventArgs e = new KeyEventArgs(key);
        //            this.OnKeyUpEvent(this, e);
        //        }
        //        return 1;
        //    }
        //    return NativeMethods.CallNextHookEx(hHook, nCode, wParam, lParam);
        //}


        private int KeyBoardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //如果该消息被丢弃（nCode<0）或者没有事件绑定处理程序则不会触发事件
            if (nCode >= 0)
            {
                KeyBoardHookStruct kbh = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));
                Keys keyData = (Keys)kbh.vkCode;
                //WM_KEYDOWN和WM_SYSKEYDOWN消息，将会引发OnKeyDownEvent事件
                if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)
                {
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    this.OnKeyDownEvent(this, e);
                }

                //WM_KEYUP和WM_SYSKEYUP消息，将引发OnKeyUpEvent事件 
                if (wParam == WM_KEYUP || wParam == WM_SYSKEYUP)
                {
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    this.OnKeyUpEvent(this, e);
                }
            }
            return WinApi.CallNextHookEx(hHook, nCode, wParam, lParam);
        }




        #endregion
    }
}
