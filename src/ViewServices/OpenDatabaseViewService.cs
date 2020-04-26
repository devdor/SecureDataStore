using SecureDataStore.ViewModels;
using SecureDataStore.Views;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace SecureDataStore.ViewServices {
    class OpenDatabaseViewService {
        #region Delegates
        readonly Action<OpenDatabaseResultArgs> _callback;
        #endregion

        public OpenDatabaseViewService(Action<OpenDatabaseResultArgs> callback) {
            this._callback = callback;
        }

        public void ShowView(OpenDatabaseArgs args) {
            if (args == null)
                throw new ArgumentNullException("OpenDatabaseArgs");

            var vm = new OpenDatabaseViewModel(args.Log, args.ViewHeader);
            if (!String.IsNullOrEmpty(args.DbFileName)) {

                var fileInfo = new FileInfo(args.DbFileName);
                vm.SafeFileName = fileInfo.Name;
                vm.FileName = fileInfo.FullName;
            }

            var win = new OpenDatabaseView() {
                DataContext = vm
            };

            win.Owner = Application.Current.MainWindow;
            win.Closing += Win_Closing;
            win.ShowDialog();
        }

        private void Win_Closing(object sender, CancelEventArgs e) {

            if (!(sender is OpenDatabaseView))
                return;

            var args = new OpenDatabaseResultArgs();
            var wnd = sender as OpenDatabaseView;
            if (wnd.DataContext != null
                && wnd.DataContext is OpenDatabaseViewModel) {

                var vm = wnd.DataContext as OpenDatabaseViewModel;
                args.IsConfirmed = vm.IsConfirmed;
                args.Password = vm.Password;
                args.DbFileName = vm.FileName;
            }

            this._callback?.Invoke(args);
        }
    }
}
