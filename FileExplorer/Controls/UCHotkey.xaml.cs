using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace FileExplorer.Controls
{
    /// <summary>
    /// Interaction logic for UCHotkey.xaml
    /// </summary>
    public partial class UCHotkey : UserControl
    {
        #region DP

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(UCHotkey), new PropertyMetadata("设置"));

        public string Keys
        {
            get { return (string)GetValue(KeysProperty); }
            set { SetValue(KeysProperty, value); }
        }
        public static readonly DependencyProperty KeysProperty =
            DependencyProperty.Register("Keys", typeof(string), typeof(UCHotkey), new PropertyMetadata("Ctrl+Alt+A"));

        #endregion

        HotkeyHelper hotkeyHelper;
        public UCHotkey()
        {
            InitializeComponent();
        }


        private void img_delete_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StartKeysEdit();
            Keys = "无";
            e.Handled = true;
        }

        private void img_edit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StartKeysEdit();
            e.Handled = true;
        }

        private void StartKeyHook()
        {
            //var helper = new WindowInteropHelper(this);
            //hwnd = HwndSource.FromHwnd(helper.Handle);
            //hwnd = (HwndSource)HwndSource.FromVisual(this);
            HwndSource hwnd = HwndSource.FromVisual(this) as HwndSource;
            this.hotkeyHelper = new HotkeyHelper(hwnd.Handle);
            hotkeyHelper.OnKeysPressed += hotkeyHelper_OnKeysPressed;
        }

        private void StopKeyHook()
        {
            if (null != this.hotkeyHelper)
            {
                using (this.hotkeyHelper)
                {
                    hotkeyHelper.OnKeysPressed -= hotkeyHelper_OnKeysPressed;
                }
                this.hotkeyHelper = null;
            }
        }

        private void hotkeyHelper_OnKeysPressed(object sender, HotkeyEventArgs e)
        {
            if (this.isEditable)
            {
                if (e.CtrlKeys != ModifierKeys.None &&
                    e.Key != System.Windows.Forms.Keys.None)
                {
                    this.Keys = string.Format("{0} + {1}", e.CtrlKeys.ToString().Replace(",", "+"), e.Key.ToString());
                }
            }
        }
        private bool isEditable = false;
        private void StartKeysEdit()
        {
            this.isEditable = !this.isEditable;
            if (this.isEditable)
            {
                lblKeys.Visibility = Visibility.Collapsed;
                txtKeys.Visibility = Visibility.Visible;
                txtHint.Visibility = txtKeys.Visibility;
                txtKeys.Focus();
                txtKeys.CaretIndex = this.Keys.Length;

                this.StartKeyHook();
                //Below code will lead edit/delete btn need click twice
                //var b = Mouse.Capture(txtKeys);
                //var mc = Mouse.Captured;
            }
            else
            {
                lblKeys.Visibility = Visibility.Visible;
                txtKeys.Visibility = Visibility.Collapsed;
                txtHint.Visibility = txtKeys.Visibility;
                this.StopKeyHook();
            }
        }
    }
}
