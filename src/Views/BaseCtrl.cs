using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace SecureDataStore.Views {
    public class BaseCtrl : UserControl, INotifyPropertyChanged {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
