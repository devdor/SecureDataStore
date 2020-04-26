using Serilog.Core;

namespace SecureDataStore.ViewServices {
    class NewDatabaseArgs : AbstractViewArgs {
        public NewDatabaseArgs(Logger logger, string viewHeader)
            : base(logger, viewHeader) {
        }
    }
}
