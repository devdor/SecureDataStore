namespace SecureDataStore.ViewServices {
    public class OpenDatabaseResultArgs : AbstractViewResultArgs {
        #region Fields and Properties
        public string Password {
            get;
            set;
        }

        public string DbFileName {
            get;
            set;
        }
        #endregion

        public OpenDatabaseResultArgs() {
        }
    }
}
