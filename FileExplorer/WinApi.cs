using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FileExplorer
{

    /// <summary>
    /// 挂钩处理过程的类型
    /// </summary>
    public enum HookType
    {
        /// <summary>
        /// 安装一个挂钩处理过程, 以监视由对话框、消息框、菜单条、或滚动条中的输入事件引发的消息
        /// </summary>
        WH_MSGFILTER = -1,
        /// <summary>
        /// 安装一个挂钩处理过程,对寄送至系统消息队列的输入消息进行纪录
        /// </summary>
        WH_JOURNALRECORD = 0,
        /// <summary>
        /// 安装一个挂钩处理过程,对此前由WH_JOURNALRECORD 挂钩处理过程纪录的消息进行寄送
        /// </summary>
        WH_JOURNALPLAYBACK = 1,
        /// <summary>
        /// 安装一个挂钩处理过程对击键消息进行监视
        /// </summary>
        WH_KEYBORARD = 2,
        /// <summary>
        /// 安装一个挂钩处理过程对寄送至消息队列的消息进行监视
        /// </summary>
        WH_GETMESSAGE = 3,
        /// <summary>
        /// 安装一个挂钩处理过程,在系统将消息发送至目标窗口处理过程之前,对该消息进行监视
        /// </summary>
        WH_CALLWNDPROC = 4,
        /// <summary>
        /// 安装一个挂钩处理过程,接受对CBT应用程序有用的消息
        /// </summary>
        WH_CBT = 5,
        /// <summary>
        /// <para>安装一个挂钩处理过程,以监视由对话框、消息框、菜单条、或滚动条中</para>
        /// <para>的输入事件引发的消息.这个挂钩处理过程对系统中所有应用程序的这类</para>
        /// <para>消息都进行监视</para>
        /// </summary>
        WH_SYSMSGFILTER = 6,
        /// <summary>
        /// 安装一个挂钩处理过程,对鼠标消息进行监视
        /// </summary>
        WH_MOUSE = 7,
        /// <summary>
        /// 安装一个挂钩处理过程以便对其他挂钩处理过程进行调试
        /// </summary>
        WH_DEBUG = 9,
        /// <summary>
        /// 安装一个挂钩处理过程以接受对外壳应用程序有用的通知
        /// </summary>
        WH_SHELL = 10,
        /// <summary>
        /// <para>安装一个挂钩处理过程,该挂钩处理过程当应用程序的前台线程即将</para>
        /// <para>进入空闲状态时被调用,它有助于在空闲时间内执行低优先级的任务</para>
        /// </summary>
        WH_FOREGROUNDIDLE = 11,
        /// <summary>
        /// 安装一个挂钩处理过程,它对已被目标窗口处理过程处理过了的消息进行监视
        /// </summary>
        WH_CALLWNDPROCRET = 12,
        /// <summary>
        /// 此挂钩只能在Windows NT中被安装,用来对底层的键盘输入事件进行监视
        /// </summary>
        WH_KEYBORARD_LL = 13,
        /// <summary>
        /// 此挂钩只能在Windows NT中被安装,用来对底层的鼠标输入事件进行监视
        /// </summary>
        WH_MOUSE_LL = 14,
    }

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyBoardHookStruct
    {
        /// <summary>
        /// 
        /// </summary>
        public int vkCode;
        /// <summary>
        /// 
        /// </summary>
        public int scanCode;
        /// <summary>
        /// 
        /// </summary>
        public int flags;
        /// <summary>
        /// 
        /// </summary>
        public int time;
        /// <summary>
        /// 
        /// </summary>
        public int dwExtraInfo;
    }

    public static class WinApi
    {
        [DllImport("User32.dll")]
        public static extern bool RegisterHotKey(
              [In] IntPtr hWnd,
              [In] int id,
              [In] uint fsModifiers,
              [In] uint vk);

        [DllImport("User32.dll")]
        public static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);


        /// <summary>
        /// 获取一个应用程序或动态链接库的模块句柄
        /// </summary>
        /// <param name="name">指定模块名，这通常是与模块的文件名相同的一个名字。例如，NOTEPAD.EXE程序的模块文件名就叫作NOTEPAD</param>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        #region hook

        /// <summary>
        ///     <para>该函数将一个应用程序定义的挂钩处理过程安装到挂钩链中去,您可以</para>
        ///     <para>通过安装挂钩处理过程来对系统的某些类型事件进行监控,这些事件与</para>
        ///     <para>某个特定的线程或系统中的所有事件相关.</para>
        ///     <para>返回值:若此函数执行成功,则返回值就是该挂钩处理过程的句柄;</para>
        ///     <para>若此函数执行失败,则返回值为NULL(0).</para>
        /// </summary>
        /// <param name="idHook">指示准备被安装的挂钩处理过程之类型，(详细：)</param>
        /// <param name="lpfn"> 
        ///     <para>指向相应的挂钩处理过程.若参数dwThreadId为0或者指示了一个其他进程</para>
        ///     <para>创建的线程之标识符,则参数lpfn必须指向一个动态链接中的挂钩处理过</para>
        ///     <para>程.否则,参数lpfn可以指向一个与当前进程相关的代码中定义的挂钩处理过程</para>
        /// </param>
        /// <param name="hInstance">
        ///     <para>指示了一个动态链接的句柄,该动态连接库包含了参数lpfn 所指向的挂钩</para>
        ///     <para>处理过程.若参数dwThreadId指示的线程由当前进程创建,并且相应的挂钩处</para>
        ///     <para>理过程定义于当前进程相关的代码中,则参数hMod必须被设置为NULL(0).</para>
        /// </param>
        /// <param name="threadId">
        ///     <para>指示了一个线程标识符,挂钩处理过程与线程相关.</para>
        ///     <para>若此参数值为0,则该挂钩处理过程与所有现存的线程相关.</para>
        /// </param>
        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(HookType idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        /// <summary>
        /// 卸载钩子
        /// <para>返回值：如果函数成功，返回值为非零值。</para>
        /// <para>如果函数失败，返回值为零。</para>
        /// </summary>
        /// <param name="idHook">要删除的钩子的句柄。这个参数是上一个函数SetWindowsHookEx的返回值.</param>
        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(int idHook);

        /// <summary>
        /// <para>调用下一个钩子</para>
        /// <para>会返回下一个钩子执行后的返回值; 0 表示失败</para>
        /// </summary>
        /// <param name="idHook">当前钩子的句柄</param>
        /// <param name="nCode">钩子代码; 就是给下一个钩子要交待的</param>
        /// <param name="wParam">要传递的参数; 由钩子类型决定是什么参数</param>
        /// <param name="lParam">要传递的参数; 由钩子类型决定是什么参数</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        #endregion

    }
}
