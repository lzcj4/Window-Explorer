using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Forms = System.Windows.Forms;

namespace FileExplorer
{
    public class HotkeyEventArgs : EventArgs
    {
        public ModifierKeys CtrlKeys { get; private set; }
        public Forms.Keys Key { get; private set; }
        public HotkeyEventArgs(ModifierKeys ctrlKeys, Forms.Keys key)
        {
            this.CtrlKeys = ctrlKeys;
            this.Key = key;
        }
    }

    /// <summary>
    /// 键盘窗口Hook,全局Register
    /// </summary>
    public class HotkeyHelper : IDisposable
    {
        public event EventHandler<HotkeyEventArgs> OnKeysPressed;

        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 9000;

        private static int global_hotkey_id = HOTKEY_ID;
        private int curret_hotkey_id;
        private HwndSource hwnd;
        private IDictionary<int, Tuple<ModifierKeys, Forms.Keys>> registedKeyDic = new Dictionary<int, Tuple<ModifierKeys, Forms.Keys>>();

        #region Infos

        /// RegisterHotkey: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646309(v=vs.85).aspx
        /// Virtual_key: https://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx 
        /// Examples:
        ///   http://stackoverflow.com/questions/11377977/global-hotkeys-in-wpf-working-from-every-window
        ///   http://stackoverflow.com/questions/48935/how-can-i-register-a-global-hot-key-to-say-ctrlshiftletter-using-wpf-and-ne
        ///   http://joelabrahamsson.com/detecting-mouse-and-keyboard-input-with-net/

        #endregion

        KeyHook keyHook;
        public HotkeyHelper(IntPtr wndPtr)
        {
            if (wndPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("无效窗口");
            }
            hwnd = HwndSource.FromHwnd(wndPtr);
            curret_hotkey_id = global_hotkey_id++;
            StartWinApiHook();
        }

        #region WIN_API

        private static bool RegisterKeys(IntPtr hwnd, int id, ModifierKeys ctrlKeys, Forms.Keys key)
        {
            bool result = WinApi.RegisterHotKey(hwnd, id, (uint)ctrlKeys, (uint)key);
            Debug.WriteLine(string.Format("/+++++ RegisterKeys pressed id: {0} ,keys: {1} + {2} , result: {3} ++++++/", id, ctrlKeys, key, result));
            return result;
        }

        private static bool UnegisterKeys(IntPtr hwnd, int id)
        {
            bool result = WinApi.UnregisterHotKey(hwnd, id);
            Debug.WriteLine(string.Format("/------ UnegisterKeys id: {0}, result: {1} ------/", id, result));
            return result;
        }

        #endregion

        #region EventManager hook keys

        private void StartEventHook()
        {
            EventManager.RegisterClassHandler(typeof(Window), Window.KeyDownEvent, new KeyEventHandler(KeyDownEvent));
            EventManager.RegisterClassHandler(typeof(Window), Window.KeyUpEvent, new KeyEventHandler(KeyUpEvent));
        }

        static IList<Key> currenPressedKeys = new List<Key>();
        private void KeyDownEvent(object sender, KeyEventArgs e)
        {
            Key key = e.Key;

            if (!currenPressedKeys.Contains(key))
            {
                if (key != Key.System)
                {
                    currenPressedKeys.Add(key);
                }
                if (e.SystemKey != Key.None)
                {
                    currenPressedKeys.Add(e.SystemKey);
                }

                string keyStr = string.Empty;
                foreach (var item in currenPressedKeys)
                {
                    keyStr += string.Format("{0} + ", item.ToString());
                }
                Debug.WriteLine(string.Format("/****** {0} ******/", keyStr));
            }
        }

        private void KeyUpEvent(object sender, KeyEventArgs e)
        {
            if (currenPressedKeys.Count == 0)
            {
                return;
            }

            ModifierKeys ctrlKeys = ModifierKeys.None;
            Forms.Keys keys = Forms.Keys.None;
            foreach (var item in currenPressedKeys)
            {
                Forms.Keys itemKey = (Forms.Keys)KeyInterop.VirtualKeyFromKey(item);
                if (itemKey == Forms.Keys.Control ||
                    itemKey == Forms.Keys.LControlKey ||
                    itemKey == Forms.Keys.RControlKey)
                {
                    ctrlKeys |= ModifierKeys.Control;
                }
                else if (itemKey == Forms.Keys.Alt ||
                    itemKey == Forms.Keys.LMenu ||
                    itemKey == Forms.Keys.RMenu)
                {
                    ctrlKeys |= ModifierKeys.Alt;
                }
                else if (itemKey == Forms.Keys.Shift ||
                    itemKey == Forms.Keys.LShiftKey ||
                    itemKey == Forms.Keys.RShiftKey)
                {
                    ctrlKeys |= ModifierKeys.Shift;
                }
                else if (itemKey == Forms.Keys.LWin ||
                    itemKey == Forms.Keys.RWin)
                {
                    ctrlKeys |= ModifierKeys.Windows;
                }
                else
                {
                    keys |= itemKey;
                }
            }

            //if (ctrlKeys != ModifierKeys.None && keys != Forms.Keys.None)
            {
                RaiseKeysPressed(ctrlKeys, keys);
            }

            if (e.SystemKey != Key.None)
            {
                currenPressedKeys.Remove(e.SystemKey);
            }
            if (e.Key != Key.None)
            {
                currenPressedKeys.Remove(e.Key);
            }
        }

        private void RaiseKeysPressed(ModifierKeys ctrlKeys, Forms.Keys key)
        {
            this.OnKeysPressed?.Invoke(null, new HotkeyEventArgs(ctrlKeys, key));
        }

        #endregion

        #region WIN API hook keys

        private void StartWinApiHook()
        {
            keyHook = new KeyHook();
            keyHook.Install_Hook();
            keyHook.KeyDownEvent += NativeKeyDownEvent;
            keyHook.KeyUpEvent += NativeKeyUpEvent;
        }

        private void StopWinApiHook()
        {
            if (null != keyHook)
            {
                keyHook.Uninstall_Hook();
                keyHook.KeyDownEvent -= NativeKeyDownEvent;
                keyHook.KeyUpEvent -= NativeKeyUpEvent;
            }
        }

        IList<Forms.Keys> nativeCurrenPressedKeys = new List<Forms.Keys>();
        private void NativeKeyDownEvent(object sender, Forms.KeyEventArgs e)
        {
            Debug.WriteLine(string.Format("/+++++ keyHook down keys: {0}++++++/", e.KeyData.ToString()));
            Forms.Keys key = e.KeyData;

            if (!nativeCurrenPressedKeys.Contains(key))
            {
                nativeCurrenPressedKeys.Add(key);
                string keyStr = string.Empty;
                foreach (var item in nativeCurrenPressedKeys)
                {
                    keyStr += string.Format("{0} + ", item.ToString());
                }
                Debug.WriteLine(string.Format("/****** {0} ******/", keyStr));
            }
        }

        private void NativeKeyUpEvent(object sender, Forms.KeyEventArgs e)
        {
            Debug.WriteLine(string.Format("/--- keyHook up keys: {0}--/", e.KeyData.ToString()));
            if (nativeCurrenPressedKeys.Count == 0)
            {
                return;
            }

            ModifierKeys ctrlKeys = ModifierKeys.None;
            Forms.Keys keys = Forms.Keys.None;
            foreach (var item in nativeCurrenPressedKeys)
            {
                if (item == Forms.Keys.Control ||
                    item == Forms.Keys.LControlKey ||
                    item == Forms.Keys.RControlKey)
                {
                    ctrlKeys |= ModifierKeys.Control;
                }
                else if (item == Forms.Keys.Alt ||
                    item == Forms.Keys.LMenu ||
                    item == Forms.Keys.RMenu)
                {
                    ctrlKeys |= ModifierKeys.Alt;
                }
                else if (item == Forms.Keys.Shift ||
                    item == Forms.Keys.LShiftKey ||
                    item == Forms.Keys.RShiftKey)
                {
                    ctrlKeys |= ModifierKeys.Shift;
                }
                else if (item == Forms.Keys.LWin ||
                    item == Forms.Keys.RWin)
                {
                    ctrlKeys |= ModifierKeys.Windows;
                }
                else
                {
                    keys |= item;
                }
            }

            //if (ctrlKeys != ModifierKeys.None && keys != Forms.Keys.None)
            {
                RaiseKeysPressed(ctrlKeys, keys);
            }

            if (e.KeyData != Forms.Keys.None)
            {
                nativeCurrenPressedKeys.Remove(e.KeyData);
            }
        }

        #endregion

        public void RegisterKeys(ModifierKeys ctrKeys, Forms.Keys key)
        {
            hwnd.AddHook(WinHook);
            HotkeyHelper.RegisterKeys(hwnd.Handle, curret_hotkey_id, ctrKeys, key);
            registedKeyDic.Add(curret_hotkey_id, new Tuple<ModifierKeys, Forms.Keys>(ctrKeys, key));
        }

        public void UnregisterKeys()
        {
            hwnd.RemoveHook(WinHook);
            for (int i = curret_hotkey_id; i >= HOTKEY_ID; i--)
            {
                HotkeyHelper.UnegisterKeys(hwnd.Handle, curret_hotkey_id);
            }
            registedKeyDic.Clear();
        }

        public IntPtr WinHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == HotkeyHelper.WM_HOTKEY)
            {
                //if (curret_hotkey_id == wParam.ToInt32())
                if (registedKeyDic.ContainsKey(wParam.ToInt32()))
                {
                    int lp = lParam.ToInt32();
                    ModifierKeys ctrlKeys = (ModifierKeys)(lp & 0xFFFF);
                    Forms.Keys key = (Forms.Keys)((lp >> 16) & 0xFFFF);

                    RaiseKeysPressed(ctrlKeys, key);
                    handled = true;

                    Debug.WriteLine(string.Format("/****** Key pressed: {0} + {1} ******/", ctrlKeys, key));
                }
            }
            return hwnd;
        }

        public void Dispose()
        {
            this.UnregisterKeys();
            this.StopWinApiHook();
        }
    }
}
