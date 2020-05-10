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

                    this.SecItemCreated = value.Created;
                    this.SecItemHeader = value.Header;
                    this.SecItemIsFavItem = value.IsFavItem;
                    this.SecItemUpdated = value.Updated;
                }
                else {

                    this.SecItemCreated = null;
                    this.SecItemHeader = null;
                    this.SecItemIsFavItem = false;
                    this.SecItemUpdated = null;
                }

                SetProperty(ref _selectedLvSecItem, value);
            }
        }

        bool _btnSecItemEditIsVisible = true;
        public bool BtnSecItemEditIsVisible {
            get => _btnSecItemEditIsVisible;
            set => SetProperty(ref _btnSecItemEditIsVisible, value);
        }

        bool _btnSecItemSaveIsVisible = false;
        public bool BtnSecItemSaveIsVisible {
            get => _btnSecItemSaveIsVisible;
            set => SetProperty(ref _btnSecItemSaveIsVisible, value);
        }

        bool _btnSecItemCancelEditIsVisible = false;
        public bool BtnSecItemCancelEditIsVisible {
            get => _btnSecItemCancelEditIsVisible;
            set => SetProperty(ref _btnSecItemCancelEditIsVisible, value);
        }

        CtrlMode _ctrlEditMode = CtrlMode.Display;
        public CtrlMode CtrlEditMode {
            get => _ctrlEditMode;
            set {
                this.BtnSecItemCancelEditIsVisible = value == CtrlMode.Edit ? true : false;
                this.BtnSecItemSaveIsVisible = value == CtrlMode.Edit ? true : false;
                this.BtnSecItemEditIsVisible = !this.BtnSecItemSaveIsVisible;
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

        bool _ctrlSecItemCreateIsEnabled = false;
        public bool CtrlSecItemCreateIsEnabled {
            get => _ctrlSecItemCreateIsEnabled;
            set => SetProperty(ref _ctrlSecItemCreateIsEnabled, value);
        }

        bool _btnDbOpenIsVisible = true;
        public bool BtnDbOpenIsVisible {
            get => _btnDbOpenIsVisible;
            set => SetProperty(ref _btnDbOpenIsVisible, value);
        }

        bool _btnDbCloseIsVisible = false;
        public bool BtnDbCloseIsVisible {
            get => _btnDbCloseIsVisible;
            set => SetProperty(ref _btnDbCloseIsVisible, value);
        }

        bool _favIconDefaultIsVisible = true;
        public bool FavIconDefaultIsVisible {
            get => _favIconDefaultIsVisible;
            set => SetProperty(ref _favIconDefaultIsVisible, value);
        }

        bool _favIconActiveIsVisible = false;
        public bool FavIconActiveIsVisible {
            get => _favIconActiveIsVisible;
            set => SetProperty(ref _favIconActiveIsVisible, value);
        }

        #region SecureItem
        bool _secItemIsFavItem;
        public bool SecItemIsFavItem {
            get => _secItemIsFavItem;
            set {
                this.FavIconActiveIsVisible = value;
                this.FavIconDefaultIsVisible = !value;
                SetProperty(ref _secItemIsFavItem, value);
            }
        }
        
        DateTime? _secItemCreated;
        public DateTime? SecItemCreated {
            get => _secItemCreated;
            set => SetProperty(ref _secItemCreated, value);
        }
        
        DateTime? _secItemUpdated;
        public DateTime? SecItemUpdated {
            get => _secItemUpdated;
            set => SetProperty(ref _secItemUpdated, value);
        }
        
        string _secItemHeader;
        public string SecItemHeader {
            get => _secItemHeader;
            set => SetProperty(ref _secItemHeader, value);
        }
        #endregion
        #endregion

        #region Commands
        public DelegateCommand AppExitCommand {
            get;
            set;
        }
        public DelegateCommand DbOpenCommand {
            get;
            set;
        }
        public DelegateCommand DbCloseCommand {
            get;
            set;
        }
        public DelegateCommand DbCreateCommand {
            get;
            set;
        }
        public DelegateCommand PwdGeneratorCommand {
            get;
            set;
        }
        public DelegateCommand SecItemCreateCommand {
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

        public DelegateCommand SecItemFavItemCommand {
            get;
            set;
        }
        #endregion

        public AppMainViewModel(Logger logger, string viewHeader)
            : base(logger, viewHeader) {

            this.AppExitCommand = new DelegateCommand(this.RaiseAppExit);
            this.DbOpenCommand = new DelegateCommand(this.RaiseDbOpen);
            this.DbCloseCommand = new DelegateCommand(this.RaiseDbClose);
            this.DbCreateCommand = new DelegateCommand(this.RaiseDbCreate);
            this.SecItemCreateCommand = new DelegateCommand(this.RaiseSecItemCreate, CanRaiseSecItemCreate).ObservesProperty(() => CtrlSecItemCreateIsEnabled);
            this.PwdGeneratorCommand = new DelegateCommand(this.RaisePwdGenerator);
            this.NavMouseRightBtnUpCommand = new DelegateCommand(this.RaiseNavMouseRightBtnUp);
            this.SecItemCancelEditCommand = new DelegateCommand(this.RaiseSecItemCancelEdit);
            this.SecItemEditCommand = new DelegateCommand(this.RaiseSecItemEdit);
            this.SecItemEditSaveCommand = new DelegateCommand(this.RaiseSecItemSave);
            this.SecItemMoveToTrashCommand = new DelegateCommand(this.SecItemUpdateState);
            this.SecItemMouseRightBtnUpCommand = new DelegateCommand(this.RaiseSecItemMouseRightBtnUp);
            this.SecItemFavItemCommand = new DelegateCommand(this.RaiseUpdateFavItem);
                        
            this.ListSysNav = new ObservableCollection<NavItemViewModel>() {
                new NavItemViewModel(Util.ReadStringRes("StrAll")) {
                    NavType = NavItemType.NULL,
                    Group = Util.ReadStringRes("StrGrpCommon"),
                    ImgSource = Util.GetResPath("baseline_all_inclusive_white_18dp.png")
                },
                new NavItemViewModel(Util.ReadStringRes("StrFavorites")) {
                    NavType = NavItemType.ShowFavorites,
                    Group = Util.ReadStringRes("StrGrpCommon"),
                    ImgSource = Util.GetResPath("baseline_star_outline_white_18dp.png")
                },
                NavItemViewModel.Create(SecItemType.Login),
                NavItemViewModel.Create(SecItemType.Document),
                NavItemViewModel.Create(SecItemType.SecureNote),
                NavItemViewModel.Create(SecItemType.Password),
                new NavItemViewModel(Util.ReadStringRes("StrTrash")) {
                    NavType = NavItemType.ShowTrash,
                    ImgSource = Util.GetResPath("outline_delete_white_18dp.png") }
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
                                  Command = this.DbCreateCommand
                              }
                          }
                        },

                        new MenuItemViewModel(Util.ReadStringRes("StrOpen")) {
                            MenuItemList = new List<MenuItemViewModel>() {
                                new MenuItemViewModel(Util.ReadStringRes("StrDatabase", true)) {
                                    Command = this.DbOpenCommand,
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

                MenuItem menItemRestoreFromTrash = new MenuItem() {
                    Header = Util.ReadStringRes("StrRestoreFromTrash"),
                    Tag = "RESTORE_FROM_TRASH"
                };

                ContextMenu ctxMenu = new ContextMenu();
                switch ((DataItemState)this.SelectedLvSecItem.State) {
                    case DataItemState.Trash:
                        ctxMenu.Items.Add(menItemRestoreFromTrash);
                        break;
                    case DataItemState.Default:
                        ctxMenu.Items.Add(menItemMoveToTrash);
                        break;
                }

                if (ctxMenu.Items.Count > 0) {

                    foreach (MenuItem menItem in ctxMenu.Items) {
                        menItem.Click += ContextMenSecItenCClick;
                    }

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
                        case "RESTORE_FROM_TRASH":

                            this.SecItemUpdateState();
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

                ContextMenu ctxMenu = new ContextMenu();
                switch (this.SelectedNavItem.NavType) {
                    case NavItemType.ShowTrash:
                        ctxMenu.Items.Add(menItenClearTrash);
                        break;
                }

                if (ctxMenu.Items.Count > 0) {

                    foreach (MenuItem menItem in ctxMenu.Items) {
                        menItem.Click += ContextMenNavItenCClick;
                    }

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

        void RaiseSecItemCreate() {
            try {

                ContextMenu ctxMenu = new ContextMenu();
                foreach (SecItemType secType in Enum.GetValues(typeof(SecItemType))) {

                    var navItem = this.ListSysNav.FirstOrDefault(
                        navItem => navItem.SecItemType == secType && navItem.NavType == NavItemType.Category);

                    if (navItem != null) {

                        var menItem = new MenuItem() {
                            Header = navItem.Header,
                            Tag = navItem.SecItemType
                        };

                        menItem.Click += OnSecItemCreateClick;
                        ctxMenu.Items.Add(menItem);
                    }
                }

                ctxMenu.IsOpen = true;
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        private void OnSecItemCreateClick(object sender, RoutedEventArgs e) {

            try {

                if (this._db == null
                    || !(sender is MenuItem))
                    return;

                var menItem = sender as MenuItem;
                if (menItem.Tag != null
                    && menItem.Tag  is SecItemType) {

                    var id = this._db.Create(
                        SecItem.Create((SecItemType)menItem.Tag));

                    if (id > 0) {

                        this.LoadDatabase();
                    }                    
                }
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        bool CanRaiseSecItemCreate() {

            return this._ctrlSecItemCreateIsEnabled;
        }

        void RaiseUpdateFavItem() {

            try {

                if (this._db == null
                    || this.SelectedLvSecItem == null)
                    return;

                if (this._db.SecItemUpdateFav(this.SelectedLvSecItem.Id, !this.SecItemIsFavItem) == 1) {

                    this.UpdateSecItemProperties();
                }
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void SecItemUpdateState() {

            try {

                if (this._db == null
                    || this.SelectedLvSecItem == null)
                    return;

                if (this._db.SecItemUpdateState(this.SelectedLvSecItem.Id,
                    (DataItemState)this.SelectedLvSecItem.State == DataItemState.Default ? DataItemState.Trash : DataItemState.Default) == 1) {

                    this.UpdateSecItemProperties();
                }
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void UpdateSecItemProperties() {

            if (this.SelectedLvSecItem != null) {

                var secItem = this._db.SecItemRead(this.SelectedLvSecItem.Id);
                if (secItem != null) {

                    var lvItem = this.ListLvSecItem.FirstOrDefault(obj => obj.Id == this.SelectedLvSecItem.Id);
                    if (lvItem != null) {

                        lvItem.Update(secItem);
                        this.SelectedLvSecItem = lvItem;
                    }
                }                
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

        void RaiseDbCreate() {
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

        void RaiseDbClose() {
            try {

                this.LvSecItemCollectionView = null;
                this.ListLvSecItem = null;
                this.SelectedLvSecItem = null; 
                this._db = null;

                this.CtrlSecItemCreateIsEnabled = false;
                this.BtnDbCloseIsVisible = false;
                this.BtnDbOpenIsVisible = true;
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void RaiseDbOpen() {
            try {

                var appSettings = this.ReadCurrentAppSettings();
                new OpenDatabaseViewService(r => {
                    if (r.IsConfirmed
                    && !String.IsNullOrEmpty(r.DbFileName)) {

                        this._db = new Database(
                            this.Log, new FileInfo(r.DbFileName), r.Password);

                        this.LoadDatabase();
                        this.BtnDbOpenIsVisible = false;
                        this.BtnDbCloseIsVisible = true;

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

                var prevNavItemType = this.SelectedNavItem != null ? this.SelectedNavItem.NavType : NavItemType.NULL;
                var secItemList = this._db.SecItemReadList(navType);

                if (!DictionaryUtils.IsNullOrEmpty(secItemList)) {

                    var tmpList = new List<LvSecureItemViewModel>();
                    foreach (var secItem in secItemList) {

                        tmpList.Add(
                            LvSecureItemViewModel.Create(secItem));
                    }

                    this.ListLvSecItem = new ObservableCollection<LvSecureItemViewModel>(
                        tmpList.OrderBy(obj => obj.Header));
                }


                this.SelectedNavItem = this.ListSysNav.FirstOrDefault(obj => obj.NavType == prevNavItemType);
                this.AppStatusMsg = $"{this._db.DbName} loaded";
                this.CtrlSecItemCreateIsEnabled = true;
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
                        if (secureItem.State != DataItemState.Trash) {
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
