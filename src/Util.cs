using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace SecureDataStore {
    public class Util {
        public static string AppDisplayName {
            get {

                return Util.ReadStringRes("StrAppDisplayName");
            }
        }

        public static string ConvertCompareOp(CompareOp op) {

            switch (op) {

                case CompareOp.EqualTo:
                    return "=";
                case CompareOp.GreaterThan:
                    return ">";
                case CompareOp.GreaterThanEqualTo:
                    return ">=";
                case CompareOp.LessThan:
                    return "<";
                case CompareOp.LessThanEqualTo:
                    return "<=";
                case CompareOp.NotEqualTo:
                    return "!=";
            }

            throw new ArgumentException("Unknown CompareOp");
        }

        public static string GetSqliteConnectionString(FileInfo fileInfo) {

            if (fileInfo == null)
                throw new ArgumentNullException("FileInfo");

            return @$"Data Source={fileInfo.FullName}";
        }

        public static T ReadResource<T>(string key) {

            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException("ResourceKey");

            if (!DictionaryUtils.IsNullOrEmpty(Application.Current.Resources.MergedDictionaries)) {

                foreach (ResourceDictionary resDict in Application.Current.Resources.MergedDictionaries) {

                    object resValue = resDict[key];
                    if (resValue != null) {

                        return (T)Convert.ChangeType(resValue, typeof(T));
                    }
                }
            }

            return default(T);
        }

        public static string ReadStringRes(string key, bool appendPunctuation = false) {

            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException("Key");

            return !appendPunctuation ? ReadResource<string>(key)
                : ReadStringRes(new object[] {
                    ReadResource<string>(key),
                    ConstValue.STR_PUNCTUATION });
        }

        public static string ReadStringRes(params object[] args) {

            if (args == null)
                return null;

            List<string> tmpList = null;
            foreach (var obj in args.Where(obj => obj != null)) {

                var strValue = ReadResource<string>(obj.ToString());

                if (tmpList == null)
                    tmpList = new List<string>();

                tmpList.Add(!String.IsNullOrEmpty(strValue) ? strValue : obj.ToString());
            }


            return tmpList != null ? String.Join(' ', tmpList) : null;
        }
    }
}
