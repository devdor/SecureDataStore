using System;
using SecureDataStore.Dto;

namespace SecureDataStore.ViewModels {
    public class NavItemViewModel : AbstractHeaderedViewModel {
        #region Fields and Properties
        NavItemType _navType;
        public NavItemType NavType {
            get => _navType;
            set => SetProperty(ref _navType, value);
        }
        
        int? _numItems;
        public int? NumItems {
            get => _numItems;
            set => SetProperty(ref _numItems, value);
        }
        
        string _imgSource;
        public string ImgSource {
            get => _imgSource;
            set => SetProperty(ref _imgSource, value);
        }
        
        string _group;
        public string Group {
            get => _group;
            set => SetProperty(ref _group, value);
        }
        
        SecItemType _secItemType;
        public SecItemType SecItemType {
            get => _secItemType;
            set => SetProperty(ref _secItemType, value);
        }
        #endregion
        public NavItemViewModel(string header)
            : base(header) {
        }
    }
}