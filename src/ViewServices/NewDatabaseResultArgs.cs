using System.IO;

namespace SecureDataStore.ViewServices {
    public class NewDatabaseResultArgs : AbstractViewResultArgs {
        #region Fields and Properties
        public string Password {
            get;
            set;
        }

        public FileInfo DbFileInfo {
            get;
            set;
        }

        public bool InitSampleValues {
            get;
            set;
        }
        #endregion

        public NewDatabaseResultArgs() {
        }
    }
}
