using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileExplorer.Controls
{
    /// <summary>
    /// 自定义密码控件，直接提示和显示密码
    /// </summary>
    public partial class UCPassword : UserControl
    {
        public UCPassword()
        {
            InitializeComponent();
        }

        #region DP

        /// <summary>
        /// 当前密码
        /// </summary>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(UCPassword),
                new PropertyMetadata(null, new PropertyChangedCallback(PasswordChangedCallback)));

        public static void PasswordChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UCPassword uc = d as UCPassword;
            if (uc != null && uc.IsShowPassword)
            {
                uc.txtPwd.Password = e.NewValue.ToString();
            }
        }


        /// <summary>
        /// 无密码时提示
        /// </summary>
        public string PasswordHint
        {
            get { return (string)GetValue(PasswordHintProperty); }
            set { SetValue(PasswordHintProperty, value); }
        }

        public static readonly DependencyProperty PasswordHintProperty =
            DependencyProperty.Register("PasswordHint", typeof(string), typeof(UCPassword), new PropertyMetadata("设定密码"));


        /// <summary>
        /// 是否显示密码
        /// </summary>
        public bool IsShowPassword
        {
            get { return (bool)GetValue(IsShowPasswordProperty); }
            set { SetValue(IsShowPasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowPassword.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowPasswordProperty =
            DependencyProperty.Register("IsShowPassword", typeof(bool), typeof(UCPassword), new PropertyMetadata(false));


        /// <summary>
        /// 密码可见图标
        /// </summary>
        public Uri ImageEyeOpened
        {
            get { return (Uri)GetValue(ImageEyeOpenedProperty); }
            set { SetValue(ImageEyeOpenedProperty, value); }
        }

        public static readonly DependencyProperty ImageEyeOpenedProperty =
            DependencyProperty.Register("ImageEyeOpened", typeof(Uri), typeof(UCPassword), 
                new PropertyMetadata(new Uri("../Assets/eye_opened.png", UriKind.RelativeOrAbsolute)));


        /// <summary>
        /// 密码不可见图标
        /// </summary>
        public Uri ImageEyeClosed
        {
            get { return (Uri)GetValue(ImageEyeClosedProperty); }
            set { SetValue(ImageEyeClosedProperty, value); }
        }

        public static readonly DependencyProperty ImageEyeClosedProperty =
            DependencyProperty.Register("ImageEyeClosed", typeof(Uri), typeof(UCPassword), 
                new PropertyMetadata(new Uri("../Assets/eye_closed.png", UriKind.RelativeOrAbsolute)));


        #endregion

        private void Image_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.IsShowPassword = !this.IsShowPassword;

            //焦点在最后字符处
            if (this.IsShowPassword)
            {
                this.txtShowPwd.CaretIndex = this.txtShowPwd.Text.Length;
                this.txtShowPwd.Focus();
            }
            else
            {
                this.txtPwd.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic)
                      .Invoke(txtPwd, new object[] { this.txtPwd.Password.Length, 0 });
                this.txtPwd.Focus();
            }
        }

        private void txtPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.Password = this.txtPwd.Password;
        }
    }
}
