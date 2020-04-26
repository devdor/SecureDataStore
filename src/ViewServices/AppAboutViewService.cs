using SecureDataStore.ViewModels;
using SecureDataStore.Views;
using System;
using System.Windows;

namespace SecureDataStore.ViewServices {
    class AppAboutViewService {
        public void ShowView(AppAboutArgs args) {
            if (args == null)
                throw new ArgumentNullException("AppAboutArgs");

            var win = new AppAboutView() {

                DataContext = new AppAboutViewModel(args.Log, args.ViewHeader) {
                    VersionInfo = args.VersionInfo,
                    AppName = args.AppName
                }
            };

            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }
    }
}
