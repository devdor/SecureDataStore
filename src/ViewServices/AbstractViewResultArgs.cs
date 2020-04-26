namespace SecureDataStore.ViewServices {
    public class AbstractViewResultArgs {
        #region Fields and Properties
        public bool IsConfirmed {
            get;
            set;
        }
        #endregion
        public AbstractViewResultArgs() {
        }

        public AbstractViewResultArgs(bool isConfirmed) {
            this.IsConfirmed = isConfirmed;
        }
    }
}
