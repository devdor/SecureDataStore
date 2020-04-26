namespace SecureDataStore.ViewModels {
    public interface IConfirmPasswordViewModel : IPasswordViewModel {
        string ConfirmPassword {
            get;
            set;
        }
    }
}
