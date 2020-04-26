using Prism.Commands;
using Serilog.Core;
using System.Windows;

namespace SecureDataStore.ViewModels {
    public abstract class AbstractDialogViewModel : AbstractLogViewModel {
        #region Fields and Properties
        public bool IsConfirmed {
            get;
            protected set;
        } = false;
        #endregion

        #region Commands
        public DelegateCommand<object> OkCommand {
            get;
            protected set;
        }

        public DelegateCommand<object> CancelCommand {
            get;
            protected set;
        }
        #endregion

        public AbstractDialogViewModel(Logger logger, string header)
            : base(logger, header) {
        }

        public void RaiseCancel(object param) {
            this.CloseWindow(param, false);
        }

        protected void CloseWindow(object obj, bool isConfirmed = false) {

            if (obj == null)
                return;

            this.IsConfirmed = isConfirmed;
            if (obj is Window) {

                ((Window)obj).Close();
            }
        }
    }
}
