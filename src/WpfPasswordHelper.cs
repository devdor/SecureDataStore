using SecureDataStore.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SecureDataStore {
    public static class WpfPasswordHelper {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
            typeof(string), typeof(WpfPasswordHelper),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(WpfPasswordHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty =
           DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
           typeof(WpfPasswordHelper));


        public static void SetAttach(DependencyObject dp, bool value) {
            dp.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(DependencyObject dp) {
            return (bool)dp.GetValue(AttachProperty);
        }

        public static string GetPassword(DependencyObject dp) {
            return (string)dp.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dp, string value) {
            dp.SetValue(PasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp) {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(DependencyObject dp, bool value) {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e) {
            PasswordBox passwordBox = sender as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;

            if (!(bool)GetIsUpdating(passwordBox)) {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void Attach(DependencyObject sender,
            DependencyPropertyChangedEventArgs e) {
            PasswordBox passwordBox = sender as PasswordBox;

            if (passwordBox == null)
                return;

            if ((bool)e.OldValue) {
                passwordBox.PasswordChanged -= PasswordChanged;
            }

            if ((bool)e.NewValue) {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e) {
            PasswordBox passwordBox = sender as PasswordBox;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);

            if (passwordBox.DataContext != null) {

                if (passwordBox.Tag != null
                    && passwordBox.Tag.ToString().Equals("CONFIRM")) {
                    if (passwordBox.DataContext is IConfirmPasswordViewModel) {

                        ((IConfirmPasswordViewModel)passwordBox.DataContext).ConfirmPassword = passwordBox.Password;
                    }
                }
                else {
                    if (passwordBox.DataContext is IPasswordViewModel) {

                        ((IPasswordViewModel)passwordBox.DataContext).Pwd = passwordBox.Password;
                    }
                }
            }

            SetIsUpdating(passwordBox, false);
        }
    }
}
