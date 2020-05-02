using System.IO;

namespace SecureDataStore.ViewServices {
    public class PwdCreateResultArgs : AbstractViewResultArgs {
        #region Fields and Properties
        public string Password {
            get;
            set;
        }
        #endregion

        public PwdCreateResultArgs() {
        }
    }
}
