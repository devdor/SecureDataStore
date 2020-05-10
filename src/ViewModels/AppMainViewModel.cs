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

        bool _isEnabled = false;
        public bool IsEnabled {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
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
        #endregion

        public AppMainViewModel(Logger logger, string viewHeader)
            : base(logger, viewHeader) {

            this.AppExitCommand = new DelegateCommand(this.RaiseAppExit);
            this.OpenDbCommand = new DelegateCommand(this.RaiseOpenDb);
            this.NewDbCommand = new DelegateCommand(this.RaiseNewDb);
            this.SecItemCreateCommand = new DelegateCommand(this.RaiseSecItemCreate, CanRaiseSecItemCreate).ObservesProperty(() => IsEnabled);            ;
            this.SecItemCancelEditCommand = new DelegateCommand(this.RaiseSecItemCancelEdit);
            this.SecItemEditCommand = new DelegateCommand(this.RaiseSecItemEdit);
            this.SecItemEditSaveCommand = new DelegateCommand(this.RaiseSecItemSave);
            this.SecItemMoveToTrashCommand = new DelegateCommand(this.SelectedSecItemUpdateState);
            this.PwdGeneratorCommand = new DelegateCommand(this.RaisePwdGenerator);
            this.NavMouseRightBtnUpCommand = new DelegateCommand(this.RaiseNavMouseRightBtnUp);
            this.SecItemMouseRightBtnUpCommand = new DelegateCommand(this.RaiseSecItemMouseRightBtnUp);

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

                            this.SelectedSecItemUpdateState();
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

            return this._isEnabled;
        }

        void SelectedSecItemUpdateState() {

            try {

                if (this._db == null
                    || this.SelectedLvSecItem == null)
                    return;

                this._db.UpdateItemState(this.SelectedLvSecItem.Id, 
                    (DataItemState)this.SelectedLvSecItem.State == DataItemState.Default ? DataItemState.Trash : DataItemState.Default );
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

                var prevNavItemType = this.SelectedNavItem != null ? this.SelectedNavItem.NavType : NavItemType.NULL;
                var secItemList = this._db.SecItemReadList(navType);

                if (!DictionaryUtils.IsNullOrEmpty(secItemList)) {

                    var tmpList = new List<LvSecureItemViewModel>();
                    foreach (var secItem in secItemList) {

                        tmpList.Add(
                            new LvSecureItemViewModel(secItem.Name, secItem.Id) {
                                Created = secItem.Created.ToLocalTime(),
                                Updated = secItem.Updated?.ToLocalTime(),
                                IsFavItem = secItem.IsFavItem,
                                State = secItem.State,
                                ItemType = (SecItemType)secItem.ItemType
                            });
                    }

                    this.ListLvSecItem = new ObservableCollection<LvSecureItemViewModel>(
                        tmpList.OrderBy(obj => obj.Header));
                }


                this.SelectedNavItem = this.ListSysNav.FirstOrDefault(obj => obj.NavType == prevNavItemType);
                this.AppStatusMsg = $"{this._db.DbName} loaded";
                this.IsEnabled = true;
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
