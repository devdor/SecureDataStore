using Prism.Commands;
using System.Collections.Generic;

namespace SecureDataStore.ViewModels {
    public class MenuItemViewModel : AbstractHeaderedViewModel {
        #region Fields and Properties
        public IList<MenuItemViewModel> MenuItemList {
            get;
            set;
        }

        public string InputGestureText {
            get;
            set;
        }
        #endregion

        #region Commands
        public DelegateCommand Command {
            get;
            set;
        }
        #endregion

        public MenuItemViewModel(string header)
            : base(header) {
        }
    }
}
