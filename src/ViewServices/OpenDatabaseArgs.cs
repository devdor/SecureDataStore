using Serilog.Core;

namespace SecureDataStore.ViewServices {
    class OpenDatabaseArgs : AbstractViewArgs {
        #region Fields and Properties
        public string DbFileName {
            get;
            set;
        }
        #endregion
        public OpenDatabaseArgs(Logger logger, string viewHeader)
            : base(logger, viewHeader) {
        }
    }
}
