namespace SecureDataStore.ViewModels {
    public abstract class AbstractLvItem : AbstractHeaderedViewModel {
        #region Fields and Properties
        int _id;
        public int Id {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        #endregion
        public AbstractLvItem(string header, int id)
            : base(header) {

            this.Id = id;
        }
    }
}
