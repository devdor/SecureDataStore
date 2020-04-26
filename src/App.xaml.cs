using SecureDataStore.ViewModels;
using SecureDataStore.Views;
using Serilog;
using System;
using System.Windows;

namespace SecureDataStore {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("log.txt",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true)
                .CreateLogger();

            try {

                var mainWnd = new AppMainWindow() {
                    DataContext = new AppMainViewModel(log, Util.AppDisplayName)
                };
                mainWnd.Show();
            }
            catch (Exception ex) {

                log.Error(ex.Message);
            }

            Log.CloseAndFlush();
        }
    }
}
