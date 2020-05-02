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

        public static string GeneratePassword(int Length, int NonAlphaNumericChars) {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            string allowedNonAlphaNum = "!@#$%&*()_-+=[{]};:<>|./?";
            Random rd = new Random();

            if (NonAlphaNumericChars > Length || Length <= 0 || NonAlphaNumericChars < 0)
                throw new ArgumentOutOfRangeException();

            char[] pass = new char[Length];
            int[] pos = new int[Length];
            int i = 0, j = 0, temp = 0;
            bool flag = false;

            while (i < Length - 1) {
                j = 0;
                flag = false;
                temp = rd.Next(0, Length);
                for (j = 0; j < Length; j++)
                    if (temp == pos[j]) {
                        flag = true;
                        j = Length;
                    }

                if (!flag) {
                    pos[i] = temp;
                    i++;
                }
            }

            for (i = 0; i < Length - NonAlphaNumericChars; i++)
                pass[i] = allowedChars[rd.Next(0, allowedChars.Length)];

            for (i = Length - NonAlphaNumericChars; i < Length; i++)
                pass[i] = allowedNonAlphaNum[rd.Next(0, allowedNonAlphaNum.Length)];

            char[] sorted = new char[Length];
            for (i = 0; i < Length; i++)
                sorted[i] = pass[pos[i]];

            return new String(sorted);
        }
    }
}
