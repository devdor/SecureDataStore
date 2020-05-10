using SecureDataStore.Dto;
using Serilog.Core;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace SecureDataStore {
    public class Database {
        #region Fields and Properties
        Logger _logger;

        SQLiteConnectionString _conString;

        public string DbName {

            get {
                if (this._conString != null) {

                    return new FileInfo(this._conString.DatabasePath).Name;
                }
                return null;
            }
        }
        #endregion
        public Database(Logger logger, FileInfo dbFileInfo, string password) {

            if (dbFileInfo == null)
                throw new ArgumentNullException("DbFileInfo");

            this._logger = logger ?? throw new ArgumentNullException("Logger");

            /*
            this._conString = new SQLiteConnectionString(dbFileInfo.FullName, false,
                key: password,
                preKeyAction: db => db.Execute("PRAGMA cipher_default_use_hmac = OFF;"),
                postKeyAction: db => db.Execute("PRAGMA kdf_iter = 128000;"));*/

            this._conString = new SQLiteConnectionString(dbFileInfo.FullName, false, key: password);
        }

        public void Init(bool initSampleValues) {

            if (this._conString == null)
                throw new MissingFieldException("SQLiteConnectionString");

            using (var db = new SQLiteConnection(this._conString)) {

                db.CreateTable<SecItem>();
                db.CreateTable<SecValueItem>();

                if (initSampleValues) {

                    Random rnd = new Random();
                    foreach (var enumVal in Enum.GetValues(typeof(SecItemType))) {

                        for (int i = 0; i < 3; i++) {

                            var secItem = SecItem.Create((SecItemType)enumVal, $"{enumVal} DemoItem{i}");
                            secItem.IsFavItem = rnd.Next(100) < 50 ? true : false;
                            secItem.State = rnd.Next(100) < 20 ? 1 : 0;

                            var result = db.Insert(secItem);
                            if (result == 1) {

                                if (result == 1) {

                                    db.Insert(SecValueItem.Create(secItem.Id, 0, SecValueItemType.Username, $"ValueItem Username"));
                                    db.Insert(SecValueItem.Create(secItem.Id, 1, SecValueItemType.Password, $"ValueItem Password"));
                                    db.Insert(SecValueItem.Create(secItem.Id, 2, SecValueItemType.Website, $"ValueItem Website"));
                                    db.Insert(SecValueItem.Create(secItem.Id, 3, SecValueItemType.Notice, $"ValueItem Notice"));
                                }
                            }
                        }
                    }
                }
            }
        }

        public int SecItemUpdateState(int id, DataItemState state) {

            if (this._conString == null)
                throw new MissingFieldException("SQLiteConnectionString");

            using (var db = new SQLiteConnection(this._conString)) {

                var querySecItem = db.Table<SecItem>().Where(
                    secItem => secItem.Id == id);

                foreach (var secItem in querySecItem) {

                    secItem.State = (int)state;
                    secItem.Updated = DateTime.UtcNow;

                    return db.Update(secItem);
                }
            }

            return 0;
        }

        public int SecItemUpdateFav(int id, bool isFav) {

            if (this._conString == null)
                throw new MissingFieldException("SQLiteConnectionString");

            using (var db = new SQLiteConnection(this._conString)) {

                var querySecItem = db.Table<SecItem>().Where(
                    secItem => secItem.Id == id);

                foreach (var secItem in querySecItem) {

                    secItem.IsFavItem = isFav;
                    secItem.Updated = DateTime.UtcNow;

                    return db.Update(secItem);
                }
            }

            return 0;
        }

        public IEnumerable<SecItem> SecItemReadList(NavItemType navItemType) {

            if (this._conString == null)
                throw new MissingFieldException("SQLiteConnectionString");

            using (var db = new SQLiteConnection(this._conString)) {

                var query = db.Table<SecItem>();
                if (query != null) {

                    return navItemType == NavItemType.NULL ?
                        query.ToList() : query.Where(catItem => catItem.ItemType == (int)navItemType).ToList();
                }

                return null;
            }
        }

        public IEnumerable<SecValueItem> ValueItemReadList(int secItemId) {

            if (this._conString == null)
                throw new MissingFieldException("SQLiteConnectionString");

            using (var db = new SQLiteConnection(this._conString)) {

                var query = db.Table<SecValueItem>().Where(catItem => catItem.RefSecItemId == secItemId);
                return query?.ToList();
            }
        }

        public int Create(AbstractDataItem item) {

            if (this._conString == null)
                throw new MissingFieldException("SQLiteConnectionString");

            if (item == null)
                throw new ArgumentNullException("DataItem");

            item.State = (int)DataItemState.Default;
            item.Created = DateTime.UtcNow;
            item.Updated = null;

            using (var db = new SQLiteConnection(this._conString)) {

                if (db.Insert(item) == 1) {

                    return item.Id;
                }
            }

            return 0;
        }

        public void EmptyTrash() {

            if (this._conString == null)
                throw new MissingFieldException("SQLiteConnectionString");

            using (var db = new SQLiteConnection(this._conString)) {

                var querySecItem = db.Table<SecItem>().Where(
                    secItem => secItem.State == (int)DataItemState.Trash);

                foreach (var secItem in querySecItem) {

                    var queryValueItem = db.Table<SecValueItem>().Where(valueItem =>
                    valueItem.RefSecItemId == secItem.Id);

                    foreach (var valueItem in queryValueItem) {

                        db.Delete<SecValueItem>(valueItem.Id);
                    }

                    db.Delete<SecItem>(secItem.Id);
                }
            }
        }
    }
}