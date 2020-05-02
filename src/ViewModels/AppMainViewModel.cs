﻿using Newtonsoft.Json;
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
                this.FilterSecureItems(value != null ? value.NavType : NavItemType.NULL);
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
            set => SetProperty(ref _selectedLvSecItem, value);
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
            this.PwdGeneratorCommand = new DelegateCommand(this.RaisePwdGenerator);

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
                new NavItemViewModel("Trash") {
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
                        this.LoadDatabase(NavItemType.NULL);
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

                        this.LoadDatabase(NavItemType.NULL);

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

        void LoadDatabase(NavItemType navType) {

            try {

                if (this._db == null)
                    return;

                var secItemList = this._db.ReadListSecItem(navType);
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

                this.SelectedNavItem = this.ListSysNav.FirstOrDefault(obj => obj.NavType == NavItemType.NULL);
                this.AppStatusMsg = $"{this._db.DbName} loaded";
            }
            catch (Exception ex) {

                this.LogError(ex);
            }
        }

        void FilterSecureItems(NavItemType navType) {
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
    }
}