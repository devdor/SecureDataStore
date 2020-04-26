using SecureDataStore.Dto;
using System;

namespace SecureDataStore.ViewModels {
    public class LvSecureItemViewModel : AbstractLvItem {
        #region Fields and Properties
        bool _isFavItem;
        public bool IsFavItem {
            get => _isFavItem;
            set => SetProperty(ref _isFavItem, value);
        }

        int _state;
        public int State {
            get => _state;
            set => SetProperty(ref _state, value);
        }
        
        SecItemType _itemType;
        public SecItemType ItemType {
            get => _itemType;
            set => SetProperty(ref _itemType, value);
        }
        #endregion
        public LvSecureItemViewModel(string header, int id)
            : base(header, id) {
        }
    }
}
