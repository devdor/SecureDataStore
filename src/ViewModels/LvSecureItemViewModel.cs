using SecureDataStore.Dto;
using System;

namespace SecureDataStore.ViewModels {
    public class LvSecureItemViewModel : AbstractLvItem {
        #region Fields and Properties
        DateTime _created;
        public DateTime Created {
            get => _created;
            set => SetProperty(ref _created, value);
        }

        DateTime? _updated;
        public DateTime? Updated {
            get => _updated;
            set => SetProperty(ref _updated, value);
        }

        bool _isFavItem;
        public bool IsFavItem {
            get => _isFavItem;
            set => SetProperty(ref _isFavItem, value);
        }

        DataItemState _state;
        public DataItemState State {
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

        public static LvSecureItemViewModel Create(SecItem secItem) {

            return new LvSecureItemViewModel(secItem.Name, secItem.Id) {
                Created = secItem.Created.ToLocalTime(),
                Updated = secItem.Updated?.ToLocalTime(),
                IsFavItem = secItem.IsFavItem,
                State = (DataItemState)secItem.State,
                ItemType = (SecItemType)secItem.ItemType
            };
        }

        public void Update(SecItem secItem) {

            this.Created = secItem.Created;
            this.Header = secItem.Name;
            this.IsFavItem = secItem.IsFavItem;
            this.ItemType = (SecItemType)secItem.ItemType;
            this.State = (DataItemState)secItem.State;
            this.Updated = secItem.Updated;
        }

        public override string ToString() {

            if (String.IsNullOrEmpty(this.Header)
                && this.Created > DateTime.MinValue) {

                return $"{this.ItemType} {Util.ReadStringRes("StrCreated")} {Created}";
            }

            return base.ToString();
        }
    }
}
