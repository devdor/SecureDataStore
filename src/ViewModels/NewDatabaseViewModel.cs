using Microsoft.Win32;
using Prism.Commands;
using Serilog.Core;
using System;

namespace SecureDataStore.ViewModels {
    public class NewDatabaseViewModel : AbstractDialogViewModel, IConfirmPasswordViewModel {
        #region Fields and Properties
        string _password;
        public string Pwd {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        string _confirmPassword;
        public string ConfirmPassword {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        string _safeFileName;
        public string SafeFileName {
            get => _safeFileName;
            set => SetProperty(ref _safeFileName, value);
        }

        string _fileName;
        public string FileName {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        bool _initSampleValues = true;
        public bool InitSampleValues {
            get => _initSampleValues;
            set => SetProperty(ref _initSampleValues, value);
        }
        #endregion

        #region Commands
        public DelegateCommand SelectDbCommand {
            get;
            set;
        }
        #endregion

        public NewDatabaseViewModel(Logger logger, string header)
            : base(logger, header) {

            this.OkCommand = new DelegateCommand<object>(this.RaiseOk);
            this.CancelCommand = new DelegateCommand<object>(this.RaiseCancel);

            this.SelectDbCommand = new DelegateCommand(this.RaiseSelectDatabase);
        }

        void RaiseOk(object param) {
            this.CloseWindow(param, true);
        }

        void RaiseSelectDatabase() {

            try {

                var fDlg = new SaveFileDialog() {
                    Filter = ConstValue.DLG_FILE_FILTER
                };

                if (fDlg.ShowDialog().GetValueOrDefault()) {

                    this.SafeFileName = fDlg.SafeFileName;
                    this.FileName = fDlg.FileName;
                }
            }
            catch (Exception ex) {

                this.ShowError(ex);
            }
        }
    }
}
