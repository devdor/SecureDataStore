using Newtonsoft.Json;
using Prism.Commands;
using SecureDataStore.Dto;
using SecureDataStore.ViewServices;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SecureDataStore.ViewModels {
    public class AppMainViewModel : AbstractLogViewModel {
        #region Fields and Properties
        Database _db;
        public IList<MenuItemViewModel> MenuItemList {
            get;
            private set;
        }

        string _appStatusMsg;
        public string AppStatusMsg {
            get => _appStatusMsg;
            set => SetProperty(ref _appStatusMsg, value);
        }

        ObservableCollection<NavItemViewModel> _listSysNav;
        public ObservableCollection<NavItemViewModel> ListSysNav {
            get => _listSysNav;
            set => SetProperty(ref _listSysNav, value);
        }

        CollectionViewSource _listSysNavView;
        public CollectionViewSource ListSysNavView {
            get => _listSysNavView;
            set => SetProperty(ref _listSysNavView, value);
        }

        NavItemViewModel _selectedNavItem;
        public NavItemViewModel SelectedNavItem {
            get => _selectedNavItem;
            set {

                this.SelectedLvSecItem = null;
                SetProperty(ref _selectedNavItem, value);
                this.FilterSecureItems();
            }
        }

        ObservableCollection<LvSecureItemViewModel> _listLvSecItem;
        public ObservableCollection<LvSecureItemViewModel> ListLvSecItem {
            get => _listLvSecItem;
            set => SetProperty(ref _listLvSecItem, value);
        }

        CollectionViewSource _lvSecItemCollectionView;
        public CollectionViewSource LvSecItemCollectionView {
            get => _lvSecItemCollectionView;
            set => SetProperty(ref _lvSecItemCollectionView, value);
        }

        LvSecureItemViewModel _selectedLvSecItem;
        public LvSecureItemViewModel SelectedLvSecItem {
            get => _selectedLvSecItem;
            set {
                if (value != null) {

                    this.CtrlNameIsVisible = false;
                    this.CtrlUsernameIsVisible = false;
                    this.CtrlPasswordIsVisible = false;
                    this.CtrlWebsiteIsVisible = false;
                    this.CtrlMultilineIsVisible = false;
                    this.CtrlFileIsVisible = false;
                    
                    switch (value.ItemType) {
                        case SecItemType.Document:
                            this.CtrlFileIsVisible = true;
                            break;
                        case SecItemType.Login:
                            this.CtrlNameIsVisible = true;
                            this.CtrlUsernameIsVisible = true;
                            this.CtrlPasswordIsVisible = true;
                            this.CtrlWebsiteIsVisible = true;
                            this.CtrlMultilineIsVisible = true;
                            break;
                        case SecItemType.Password:
                            this.CtrlNameIsVisible = true;                            
                            this.CtrlPasswordIsVisible = true;
                            this.CtrlWebsiteIsVisible = true;
                            break;
                        case SecItemType.SecureNote:
                            this.CtrlNameIsVisible = true;
                            this.CtrlMultilineIsVisible = true;
                            break;
                    }
                }
                SetProperty(ref _selectedLvSecItem, value);
            }
        }

        bool _btnSecItemEditVisible = true;
        public bool BtnSecItemEditVisible {
            get => _btnSecItemEditVisible;
            set => SetProperty(ref _btnSecItemEditVisible, value);
        }

        bool _btnSecItemSaveVisible = false;
        public bool BtnSecItemSaveVisible {
            get => _btnSecItemSaveVisible;
            set => SetProperty(ref _btnSecItemSaveVisible, value);
        }

        bool _btnSecItemCancelEditVisible = false;
        public bool BtnSecItemCancelEditVisible {
            get => _btnSecItemCancelEditVisible;
            set => SetProperty(ref _btnSecItemCancelEditVisible, value);
        }

        CtrlMode _ctrlEditMode = CtrlMode.Display;
        public CtrlMode CtrlEditMode {
            get => _ctrlEditMode;
            set {
                this.BtnSecItemCancelEditVisible = value == CtrlMode.Edit ? true : false;
                this.BtnSecItemSaveVisible = value == CtrlMode.Edit ? true : false;
                this.BtnSecItemEditVisible = !this.BtnSecItemSaveVisible;
                SetProperty(ref _ctrlEditMode, value);
            }
        }

        bool _ctrlNameIsVisible = false;
        public bool CtrlNameIsVisible {
            get => _ctrlNameIsVisible;
            set => SetProperty(ref _ctrlNameIsVisible, value);
        }        

        bool _ctrlUsernameIsVisible = false;
        public bool CtrlUsernameIsVisible {
            get => _ctrlUsernameIsVisible;
            set => SetProperty(ref _ctrlUsernameIsVisible, value);
        }        

        bool _ctrlPasswordIsVisible = false;
        public bool CtrlPasswordIsVisible {
            get => _ctrlPasswordIsVisible;
            set => SetProperty(ref _ctrlPasswordIsVisible, value);
        }        

        bool _ctrlWebsiteIsVisible = false;
        public bool CtrlWebsiteIsVisible {
            get => _ctrlWebsiteIsVisible;
            set => SetProperty(ref _ctrlWebsiteIsVisible, value);
        }
        
        bool _ctrlMultilineIsVisible = false;
        public bool CtrlMultilineIsVisible {
            get => _ctrlMultilineIsVisible;
            set => SetProperty(ref _ctrlMultilineIsVisible, value);
        }
        
        bool _ctrlFileIsVisible = false;
        public bool CtrlFileIsVisible {
            get => _ctrlFileIsVisible;
            set => SetProperty(ref _ctrlFileIsVisible, value);
        }
        #endregion

        #region Commands
        public DelegateCommand AppExitCommand {
            get;
            set;
        }
        public DelegateCommand OpenDbCommand {
            get;
            set;
        }
        public DelegateCommand NewDbCommand {
            get;
            set;
        }
        public DelegateCommand PwdGeneratorCommand {
            get;
            set;
        }
        public DelegateCommand AddCategoryItemCommand {
            get;
            set;
        }
        public DelegateCommand NavMouseRightBtnUpCommand {
            get;
            set;
        }
        public DelegateCommand SecItemEditCommand {
            get;
            set;
        }
        public DelegateCommand SecItemEditSaveCommand {
            get;
            set;
        }
        public DelegateCommand SecItemCancelEditCommand {
            get;
            set;
        }
        public DelegateCommand SecItemMoveToTrashCommand {
            get;
            set;
        }
        public DelegateCommand SecItemMouseRightBtnUpCommand {
            get;
            set;
        }
        #endregion

        public AppMainViewModel(Logger logger, string viewHeader)
            : base(logger, viewHeader) {

            this.AppExitCommand = new DelegateCommand(this.RaiseAppExit);
            this.OpenDbCommand = new DelegateCommand(this.RaiseOpenDb);
            this.NewDbCommand = new DelegateCommand(this.RaiseNewDb);
            this.AddCategoryItemCommand = new DelegateCommand(this.RaiseAddCategoryItem);
            this.SecItemCancelEditCommand = new DelegateCommand(this.RaiseSecItemCancelEdit);
            this.SecItemEditCommand = new DelegateCommand(this.RaiseSecItemEdit);
            this.SecItemEditSaveCommand = new DelegateCommand(this.RaiseSecItemSave);
            this.SecItemMoveToTrashCommand = new DelegateCommand(this.RaiseSecItemMoveToTrash);
            this.PwdGeneratorCommand = new DelegateCommand(this.RaisePwdGenerator);
            this.NavMouseRightBtnUpCommand = new DelegateCommand(this.RaiseNavMouseRightBtnUp);
            this.SecItemMouseRightBtnUpCommand = new DelegateCommand(this.RaiseSecItemMouseRightBtnUp);

            string GetResString(string img) {
                return $"../Res/{img}";
            }

            this.ListSysNav = new ObservableCollection<NavItemViewModel>() {
                new NavItemViewModel("All") {
                    NavType = NavItemType.NULL,
                    Group = Util.ReadStringRes("StrGrpCommon"),
                    ImgSource = GetResString("baseline_all_inclusive_white_18dp.png")
                },
                new NavItemViewModel("Favorites") {
                    NavType = NavItemType.ShowFavorites,
                    Group = Util.ReadStringRes("StrGrpCommon"),
                    ImgSource = GetResString("baseline_star_outline_white_18dp.png")
                },
                new NavItemViewModel(Util.ReadStringRes("StrSecItemLogin")) {
                    NavType = NavItemType.Category,
                    SecItemType = SecItemType.Login,
                    Group = Util.ReadStringRes("StrGrpCategories"),
                    ImgSource = GetResString("baseline_cloud_queue_white_18dp.png")
                },
                new NavItemViewModel(Util.ReadStringRes("StrSecItemDoc")){
                    NavType = NavItemType.Category,
                    SecItemType = SecItemType.Document,
                    Group = Util.ReadStringRes("StrGrpCategories"),
                    ImgSource = GetResString("baseline_mail_outline_white_18dp.png")
                },
                new NavItemViewModel(Util.ReadStringRes("StrSecItemNote")) {
                    NavType = NavItemType.Category,
                    SecItemType = SecItemType.SecureNote,
                    Group = Util.ReadStringRes("StrGrpCategories"),
                    ImgSource = GetResString("outline_insert_drive_file_white_18dp.png")
                },
                new NavItemViewModel(Util.ReadStringRes("StrSecItemPwd")) {
                    NavType = NavItemType.Category,
                    SecItemType = SecItemType.Password,
                    Group = Util.ReadStringRes("StrGrpCategories"),
                    ImgSource = GetResString("outline_vpn_key_white_18dp.png")
                },
                new NavItemViewModel(Util.ReadStringRes("StrTrash")) {
                    NavType = NavItemType.ShowTrash,
                    ImgSource = GetResString("outline_delete_white_18dp.png") }
            };

            var view = new CollectionViewSource();
            view.GroupDescriptions.Add(
                new PropertyGroupDescription("Group"));
            view.Source = this.ListSysNav;
            this.ListSysNavView = view;

            this.MenuItemList = new List<MenuItemViewModel>() {
                new MenuItemViewModel(Util.ReadStringRes("StrFile")) {
                    MenuItemList = new List<MenuItemViewModel>(){
                        new MenuItemViewModel(Util.ReadStringRes("StrNew")) {
                          MenuItemList = new List<MenuItemViewModel>() {
                              new MenuItemViewModel(Util.ReadStringRes("StrDatabase", true)) {
                                  Command = this.NewDbCommand,
                                  InputGestureText = Util.ReadStringRes("StrGestureCtrl_Shift_N")
                              }
                          }
                        },

                        new MenuItemViewModel(Util.ReadStringRes("StrOpen")) {
                            MenuItemList = new List<MenuItemViewModel>() {
                                new MenuItemViewModel(Util.ReadStringRes("StrDatabase", true)) {
                                    Command = this.OpenDbCommand,
                                    InputGestureText = Util.ReadStringRes("StrGestureCtrl_O")
                                }
                            }
                        },
                        new MenuItemViewModel(Util.ReadStringRes("StrPwdGenerator", true)) {
                            Command = this.PwdGeneratorCommand
                        },
                        new MenuItemViewModel(Util.ReadStringRes("StrExit")) {
                            Command = this.AppExitCommand,
                            InputGestureText = Util.ReadStringRes("StrGestureAlt_F4")
                        }
                    }
                },
                new MenuItemViewModel(Util.ReadStringRes("StrHelp")) {
                    MenuItemList = new List<MenuItemViewModel>(){
                        new MenuItemViewModel(Util.ReadStringRes("StrAbout", true)) {
                            Command = new DelegateCommand(this.RaiseAppAbout)
                        }
                    }
                }
            };

            this.AppStatusMsg = Util.ReadStringRes("StrReady", ConstValue.STR_PUNCTUATION);
        }

        #region Contextmenu SecItem
        void RaiseSecItemMouseRightBtnUp() {
            try {

                if (this.SelectedLvSecItem == null)
                    return;

                MenuItem menItemMoveToTrash = new MenuItem() {
                    Header = Util.ReadStringRes("StrMoveToTrash"),
                    Tag = "MOVE_TO_TRASH"
                };
                menItemMoveToTrash.Click += ContextMenSecItenCClick;

                ContextMenu ctxMenu = new ContextMenu();
                if (this.SelectedLvSecItem.State != (int)DataItemState.Trash) {
                    ctxMenu.Items.Add(menItemMoveToTrash);
                }

                if (ctxMenu.Items.Count > 0) {
                    ctxMenu.IsOpen = true;
                }
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void ContextMenSecItenCClick(object sender, RoutedEventArgs e) {
            try {

                if (this._db == null)
                    return;

                if (!(sender is MenuItem))
                    return;

                var menItem = sender as MenuItem;
                if (menItem.Tag != null) {

                    switch (menItem.Tag.ToString()) {

                        case "MOVE_TO_TRASH":

                            this.RaiseSecItemMoveToTrash();
                            this.LoadDatabase();
                            break;
                    }
                }
                
                this.FilterSecureItems();
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }
        #endregion

        #region Contextmenu NavItem
        void RaiseNavMouseRightBtnUp() {
            try {

                if (this.SelectedNavItem == null)
                    return;

                MenuItem menItenClearTrash = new MenuItem() {
                    Header = Util.ReadStringRes("StrEmptyTrash"),
                    Tag = "EMPTY_TRASH"
                };
                menItenClearTrash.Click += ContextMenNavItenCClick;

                ContextMenu ctxMenu = new ContextMenu();
                switch (this.SelectedNavItem.NavType) {
                    case NavItemType.ShowTrash:
                        ctxMenu.Items.Add(menItenClearTrash);
                        break;
                }

                if (ctxMenu.Items.Count > 0) {
                    ctxMenu.IsOpen = true;
                }
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void ContextMenNavItenCClick(object sender, RoutedEventArgs e) {
            try {

                if (this._db == null)
                    return;

                if (!(sender is MenuItem))
                    return;

                var menItem = sender as MenuItem;
                if (menItem.Tag != null) {

                    switch (menItem.Tag.ToString()) {

                        case "EMPTY_TRASH":

                            this._db.EmptyTrash();
                            this.LoadDatabase();
                            break;
                    }
                }

                this.FilterSecureItems();
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }
        #endregion

        #region Common Eventhandler
        void RaiseSecItemEdit() {
            this.CtrlEditMode = CtrlMode.Edit;
        }

        void RaiseSecItemCancelEdit() {
            this.CtrlEditMode = CtrlMode.Display;
        }

        void RaiseSecItemSave() {
            this.CtrlEditMode = CtrlMode.Display;
        }

        void RaiseAppExit() {
            Application.Current.Shutdown();
        }

        void RaiseSecItemMoveToTrash() {

            try {

                if (this.SelectedLvSecItem == null)
                    return;

                if (this._db == null)
                    return;

                this._db.SecItemMoveToTrash(this.SelectedLvSecItem.Id);
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void RaisePwdGenerator() {
            try {

                new PwdCreateViewService(dlg => {
                }).ShowView(
                    new PwdCreateArgs(
                        this.Log, Util.ReadStringRes("StrPwdGenerator")));
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void RaiseAddCategoryItem() {
            try {

                if (this._db == null)
                    return;
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void RaiseNewDb() {
            try {

                new NewDatabaseViewService(r => {
                    if (r.IsConfirmed
                    && r.DbFileInfo != null) {

                        this._db = new Database(
                            this.Log, r.DbFileInfo, r.Password);

                        this._db.Init(r.InitSampleValues);
                        this.LoadDatabase();
                    }
                }).ShowView(
                    new NewDatabaseArgs(
                        this.Log, Util.ReadStringRes("StrNew", "StrDatabase")));
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void RaiseOpenDb() {
            try {

                var appSettings = this.ReadCurrentAppSettings();
                new OpenDatabaseViewService(r => {
                    if (r.IsConfirmed
                    && !String.IsNullOrEmpty(r.DbFileName)) {

                        this._db = new Database(
                            this.Log, new FileInfo(r.DbFileName), r.Password);

                        this.LoadDatabase();

                        if (!Directory.Exists(ConstValue.USER_SETTINGS_PATH))
                            Directory.CreateDirectory(ConstValue.USER_SETTINGS_PATH);

                        if (appSettings == null)
                            appSettings = new AppSettings();

                        appSettings.DefaultDbName = r.DbFileName;
                        File.WriteAllText(ConstValue.APP_SETTINGS_NAME,
                            JsonConvert.SerializeObject(appSettings,
                            Formatting.Indented, new JsonSerializerSettings {
                                TypeNameHandling = TypeNameHandling.All
                            }));

                    }
                }).ShowView(
                    new OpenDatabaseArgs(
                        this.Log,
                        Util.ReadStringRes("StrOpen", "StrDatabase")) {
                        DbFileName = appSettings?.DefaultDbName
                    });
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void RaiseAppAbout() {
            try {

                var assembly = Assembly.GetExecutingAssembly();

                new AppAboutViewService().ShowView(
                    new AppAboutArgs(
                        this.Log,
                        Util.ReadStringRes("StrAbout", Util.AppDisplayName)) {
                        AppName = assembly.GetName().Name,
                        VersionInfo = $"Version {assembly.GetName().Version.ToString()}"
                    });
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void LoadDatabase() {

            this.LoadDatabaseInternal(NavItemType.NULL);
        }

        void LoadDatabaseInternal(NavItemType navType) {

            try {

                if (this._db == null)
                    return;

                var secItemList = this._db.SecItemReadList(navType);
                if (!DictionaryUtils.IsNullOrEmpty(secItemList)) {

                    this.ListLvSecItem = new ObservableCollection<LvSecureItemViewModel>();
                    foreach (var secItem in secItemList.OrderBy(obj => obj.Name)) {

                        this.ListLvSecItem.Add(
                            new LvSecureItemViewModel(secItem.Name, secItem.Id) {
                                Created = secItem.Created,
                                Updated = secItem.Updated,
                                IsFavItem = secItem.IsFavItem,
                                State = secItem.State,
                                ItemType = (SecItemType)secItem.ItemType
                            });
                    }
                }

                if (this.SelectedNavItem == null) {
                    this.SelectedNavItem = this.ListSysNav.FirstOrDefault(obj => obj.NavType == NavItemType.NULL);
                }
                
                this.AppStatusMsg = $"{this._db.DbName} loaded";
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void FilterSecureItems() {
            try {

                var view = new CollectionViewSource();
                view.Source = this.ListLvSecItem;
                view.Filter += OnSecItemFilter;
                this.LvSecItemCollectionView = view;
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void OnSecItemFilter(object sender, FilterEventArgs e) {

            if (!(e.Item is LvSecureItemViewModel))
                return;

            var secureItem = e.Item as LvSecureItemViewModel;
            e.Accepted = true;

            if (this.SelectedNavItem != null) {

                var navItem = this.SelectedNavItem;
                switch (navItem.NavType) {

                    case NavItemType.NULL:
                        e.Accepted = secureItem.State == 0;
                        break;
                    case NavItemType.Category:
                        e.Accepted = secureItem.ItemType == navItem.SecItemType && secureItem.State == 0;
                        break;
                    case NavItemType.ShowFavorites:
                        e.Accepted = secureItem.IsFavItem && secureItem.State == 0;
                        break;
                    case NavItemType.ShowTrash:
                        if (secureItem.State != 1) {
                            e.Accepted = false;
                        }
                        break;
                }
            }

        }

        AppSettings ReadCurrentAppSettings() {

            var tmpData = File.ReadAllText(ConstValue.APP_SETTINGS_NAME);
            if (!String.IsNullOrEmpty(tmpData)
                && this.ValidateJSON(tmpData)) {

                return JsonConvert.DeserializeObject<AppSettings>(tmpData,
                    new JsonSerializerSettings {
                        TypeNameHandling = TypeNameHandling.All
                    });
            }

            return null;
        }
        #endregion
    }
}
