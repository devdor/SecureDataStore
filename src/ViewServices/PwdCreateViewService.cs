using SecureDataStore.ViewModels;
using SecureDataStore.Views;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace SecureDataStore.ViewServices {
    class PwdCreateViewService {
        #region Delegates
        readonly Action<PwdCreateResultArgs> _callback;
        #endregion

        public PwdCreateViewService(Action<PwdCreateResultArgs> callback) {
            this._callback = callback;
        }

        public void ShowView(PwdCreateArgs args) {
            if (args == null)
                throw new ArgumentNullException("PwdCreateArgs");

            var win = new PwdCreateView() {
                DataContext = new PwdCreateViewModel(args.Log, args.ViewHeader)
            };

            win.Owner = Application.Current.MainWindow;
            win.Closing += Win_Closing;
            win.ShowDialog();
        }

        private void Win_Closing(object sender, CancelEventArgs e) {

            if (!(sender is PwdCreateView))
                return;

            var args = new PwdCreateResultArgs();
            var wnd = sender as PwdCreateView;
            if (wnd.DataContext != null
                && wnd.DataContext is PwdCreateViewModel) {

                var vm = wnd.DataContext as PwdCreateViewModel;
                args.IsConfirmed = vm.IsConfirmed;
                args.Password = vm.Pwd;
            }

            this._callback?.Invoke(args);
        }
    }
}
