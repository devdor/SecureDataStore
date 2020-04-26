using System;

namespace SecureDataStore.ViewModels {
    public abstract class AbstractLvItem : AbstractHeaderedViewModel {
        #region Fields and Properties
        int _id;
        public int Id {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        object _tag;
        public object Tag {
            get => _tag;
            set => SetProperty(ref _tag, value);
        }        
        #endregion
        public AbstractLvItem(string header, int id)
            : base(header) {

            this.Id = id;
        }
    }
}
