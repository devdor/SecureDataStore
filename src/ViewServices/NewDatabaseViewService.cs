using SecureDataStore.ViewModels;
using SecureDataStore.Views;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace SecureDataStore.ViewServices {
    class NewDatabaseViewService {
        #region Delegates
        readonly Action<NewDatabaseResultArgs> _callback;
        #endregion

        public NewDatabaseViewService(Action<NewDatabaseResultArgs> callback) {
            this._callback = callback;
        }

        public void ShowView(NewDatabaseArgs args) {
            if (args == null)
                throw new ArgumentNullException("NewDatabaseArgs");

            var win = new NewDatabaseView() {

                DataContext = new NewDatabaseViewModel(args.Log, args.ViewHeader)
            };

            win.Owner = Application.Current.MainWindow;
            win.Closing += Win_Closing;
            win.ShowDialog();
        }

        private void Win_Closing(object sender, CancelEventArgs e) {

            if (!(sender is NewDatabaseView))
                return;

            var args = new NewDatabaseResultArgs();
            var wnd = sender as NewDatabaseView;
            if (wnd.DataContext != null
                && wnd.DataContext is NewDatabaseViewModel) {

                var vm = wnd.DataContext as NewDatabaseViewModel;

                if (!String.IsNullOrEmpty(vm.Pwd)
                    && !vm.Pwd.Equals(vm.ConfirmPassword)) {
                    e.Cancel = true;
                    return;
                }

                args.IsConfirmed = vm.IsConfirmed;
                args.Password = vm.Pwd;
                args.InitSampleValues = vm.InitSampleValues;

                if (!String.IsNullOrEmpty(vm.FileName))
                    args.DbFileInfo = new FileInfo(vm.FileName);
            }

            this._callback?.Invoke(args);
        }
    }
}
