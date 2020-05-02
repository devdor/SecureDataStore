using Prism.Commands;
using Serilog.Core;
using System;
using System.Diagnostics;

namespace SecureDataStore.ViewModels {
    public class PwdCreateViewModel : AbstractDialogViewModel, IPasswordViewModel {
        #region Fields and Properties
        string _pwd;
        public string Pwd {
            get => _pwd;
            set => SetProperty(ref _pwd, value);
        }

        int _pwdLength = 16;
        public int PwdLength {
            get => _pwdLength;
            set {
                SetProperty(ref _pwdLength, value);
                this.CreatePassword();
            }
        }

        bool _nonAlphaNumCharsIsChecked;
        public bool NonAlphaNumCharsIsChecked {
            get => _nonAlphaNumCharsIsChecked;
            set {
                SetProperty(ref _nonAlphaNumCharsIsChecked, value);
                this.CreatePassword();
            }
        }
        #endregion

        #region Commands
        public DelegateCommand CopyToClipboardCommand {
            get;
            set;
        }
        public DelegateCommand CreatePwdCommand {
            get;
            set;
        }
        #endregion

        public PwdCreateViewModel(Logger logger, string header)
            : base(logger, header) {
                        
            this.OkCommand = new DelegateCommand<object>(this.RaiseOk);
            this.CancelCommand = new DelegateCommand<object>(this.RaiseCancel);

            this.CopyToClipboardCommand = new DelegateCommand(this.RaiseCopyToClipboard);
            this.CreatePwdCommand = new DelegateCommand(this.RaiseCreatePwd);

            this.CreatePassword();
        }

        void RaiseOk(object param) {

            this.CloseWindow(
                param, !String.IsNullOrEmpty(this.Pwd));
        }

        void RaiseCreatePwd() {
            try {

                this.CreatePassword();
            }
            catch (Exception ex) {

                this.ShowError(ex);
            }
        }

        void CreatePassword() {

            this.Pwd = Util.GeneratePassword(
                this.PwdLength, this.NonAlphaNumCharsIsChecked ? this.PwdLength / 5 : 0);
        }
        void RaiseCopyToClipboard() {

            try {

                Process clipboardExecutable = new Process();
                clipboardExecutable.StartInfo = new ProcessStartInfo
                {
                    RedirectStandardInput = true,
                    FileName = @"clip",
                };
                clipboardExecutable.Start();
                clipboardExecutable.StandardInput.Write(this.Pwd);
                clipboardExecutable.StandardInput.Close();
            }
            catch (Exception ex) {

                this.ShowError(ex);
            }
        }
    }
}
