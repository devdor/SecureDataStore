namespace SecureDataStore {
    public class AppSettings {
        #region Fields and Properties
        public string DefaultDbName {
            get;
            set;
        }
        #endregion
        public AppSettings() {
        }

        public static AppSettings Load() {

            return new AppSettings();
        }
    }
}
