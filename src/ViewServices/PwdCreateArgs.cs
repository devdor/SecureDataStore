using Serilog.Core;

namespace SecureDataStore.ViewServices {
    class PwdCreateArgs : AbstractViewArgs {
        public PwdCreateArgs(Logger logger, string viewHeader)
            : base(logger, viewHeader) {
        }
    }
}
