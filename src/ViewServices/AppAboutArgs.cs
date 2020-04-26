using Serilog.Core;

namespace SecureDataStore.ViewServices {
    class AppAboutArgs : AbstractViewArgs {
        #region Fields and Properties
        public string AppName {
            get;
            set;
        }

        public string VersionInfo {
            get;
            set;
        }
        #endregion
        public AppAboutArgs(Logger logger, string viewHeader)
            : base(logger, viewHeader) {
        }
    }
}
