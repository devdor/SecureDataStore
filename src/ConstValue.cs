using System;
using System.IO;
using System.Reflection;

namespace SecureDataStore {
    public class ConstValue {
        public static readonly string ASSEMBLY_NAME = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string USER_SETTINGS_PATH = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ASSEMBLY_NAME);
        public static readonly string APP_SETTINGS_NAME = Path.Combine(
            ConstValue.USER_SETTINGS_PATH, "settings.json");
        public static readonly string STR_PUNCTUATION = "...";
        public static readonly string DLG_FILE_FILTER = "DB file(*.db)|*.db|All files(*.*)|*.*";
    }
}
