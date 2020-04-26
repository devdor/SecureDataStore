using System;

namespace SecureDataStore.ViewModels {
    public abstract class AbstractHeaderedViewModel : AbstractBaseViewModel {
        #region Fields and Properties
        string _header;
        public string Header {
            get => _header;
            set => this.SetProperty(ref _header, value);
        }
        #endregion

        public AbstractHeaderedViewModel(string header)
            : base() {

            this.Header = header;
        }

        public override string ToString() {

            return !String.IsNullOrEmpty(this.Header) ? this.Header : base.ToString();
        }
    }
}
