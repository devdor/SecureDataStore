using System;

namespace SecureDataStore.Dto {
    public class SecValueItem : AbstractDataItem {
        #region Fields and Properties
        public int RefSecItemId {
            get;
            set;
        }

        public int Pos {
            get;
            set;
        }

        public int ValueType {
            get;
            set;
        }

        public string Value {
            get;
            set;
        }
        #endregion

        public static SecValueItem Create(int refSecItem, int pos, SecValueItemType valueType, string value) {

            return new SecValueItem() {
                State = (int)DataItemState.Default,
                Created = DateTime.UtcNow,
                RefSecItemId = refSecItem,
                Pos = pos,
                ValueType = (int)valueType,
                Value = value
            };
        }
    }
}
