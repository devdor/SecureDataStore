using SQLite;
using System;

namespace SecureDataStore.Dto {
    public class AbstractDataItem {
        #region Fields and Properties
        [PrimaryKey, AutoIncrement]
        public int Id {
            get;
            set;
        }

        public DateTime Created {
            get;
            set;
        }

        public DateTime? Updated {
            get;
            set;
        }

        public int State {
            get;
            set;
        }
        #endregion
    }
}
