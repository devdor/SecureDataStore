using SecureDataStore.Dto;
using System;

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

        public static NavItemViewModel Create(SecItemType itemType) {

            switch (itemType) {
                case SecItemType.Document:
                    return new NavItemViewModel(Util.ReadStringRes("StrSecItemDoc")) {
                        NavType = NavItemType.Category,
                        SecItemType = itemType,
                        Group = Util.ReadStringRes("StrGrpCategories"),
                        ImgSource = Util.GetResPath("baseline_mail_outline_white_18dp.png")
                    };
                case SecItemType.Login:
                    return new NavItemViewModel(Util.ReadStringRes("StrSecItemLogin")) {
                        NavType = NavItemType.Category,
                        SecItemType = itemType,
                        Group = Util.ReadStringRes("StrGrpCategories"),
                        ImgSource = Util.GetResPath("baseline_cloud_queue_white_18dp.png")
                    };
                case SecItemType.Password:
                    return new NavItemViewModel(Util.ReadStringRes("StrSecItemPwd")) {
                        NavType = NavItemType.Category,
                        SecItemType = itemType,
                        Group = Util.ReadStringRes("StrGrpCategories"),
                        ImgSource = Util.GetResPath("outline_vpn_key_white_18dp.png")
                    };
                case SecItemType.SecureNote:
                    return new NavItemViewModel(Util.ReadStringRes("StrSecItemNote")) {
                        NavType = NavItemType.Category,
                        SecItemType = itemType,
                        Group = Util.ReadStringRes("StrGrpCategories"),
                        ImgSource = Util.GetResPath("outline_insert_drive_file_white_18dp.png")
                    };
                default:
                    throw new NotImplementedException(itemType.ToString());
            }
        }
    }
}