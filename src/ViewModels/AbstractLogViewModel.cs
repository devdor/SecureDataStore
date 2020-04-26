using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.Core;
using System;
using System.Windows;

namespace SecureDataStore.ViewModels {
    public abstract class AbstractLogViewModel : AbstractHeaderedViewModel {
        #region Fields and Properties
        public Logger Log {
            get;
            private set;
        }
        #endregion

        public AbstractLogViewModel(Logger logger, string header)
            : base(header) {

            this.Log = logger;
        }

        public void LogError(Exception ex, bool showErrorMessageBox = true) {

            this.Log?.Error(ex.Message);

            if (showErrorMessageBox)
                this.ShowError(ex);
        }

        protected void ShowMessage(string msg, bool isError = false) {
            if (String.IsNullOrWhiteSpace(msg))
                return;

            MessageBox.Show(msg,
                Util.AppDisplayName, MessageBoxButton.OK, isError ? MessageBoxImage.Error : MessageBoxImage.Information);
        }

        public void ShowError(Exception ex) {
            if (ex == null)
                return;

            this.ShowMessage(ex.Message, true);
        }

        public bool ValidateJSON(string s) {
            try {
                JToken.Parse(s);
                return true;
            }
            catch (JsonReaderException ex) {

                this.ShowError(ex);
                return false;
            }
        }
    }
}
