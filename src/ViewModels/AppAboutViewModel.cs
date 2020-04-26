using Prism.Commands;
using Serilog.Core;

namespace SecureDataStore.ViewModels {
    public class AppAboutViewModel : AbstractDialogViewModel {
        #region Fields and Properties
        string _versionInfo;
        public string VersionInfo {
            get => _versionInfo;
            set => SetProperty(ref _versionInfo, value);
        }

        string _appName;
        public string AppName {
            get => _appName;
            set => SetProperty(ref _appName, value);
        }
        #endregion        

        public AppAboutViewModel(Logger logger, string viewHeader)
            : base(logger, viewHeader) {

            this.OkCommand = new DelegateCommand<object>(this.RaiseOk);
        }

        void RaiseOk(object param) {
            this.CloseWindow(param, true);
        }
    }
}
