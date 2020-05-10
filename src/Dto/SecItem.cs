using System;

namespace SecureDataStore.Dto {
    public class SecItem : AbstractDataItem {
        #region Fields and Properties
        public int ItemType {
            get;
            set;
        }

        public string Name {
            get;
            set;
        }

        public bool IsFavItem {
            get;
            set;
        }
        #endregion
        public static SecItem Create(SecItemType itemType, string name = null) {
            
            return new SecItem() {
                State = (int)DataItemState.Default,
                Created = DateTime.UtcNow,
                ItemType = (int)itemType,
                Name = name
            };
        }
    }
}
