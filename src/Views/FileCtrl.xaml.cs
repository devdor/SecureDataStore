using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecureDataStore.Views {
    /// <summary>
    /// Interaktionslogik für FileCtrl.xaml
    /// </summary>
    public partial class FileCtrl : BaseCtrl {
        #region Fields and Properties
        #region CtrlMode
        public static readonly DependencyProperty CtrlEditModeProperty = DependencyProperty.RegisterAttached(
            "CtrlEditMode",
            typeof(CtrlMode),
            typeof(FileCtrl),
            new PropertyMetadata(CtrlModeChangedCallback));
        public CtrlMode CtrlEditMode {
            get {
                return (CtrlMode)GetValue(CtrlEditModeProperty);
            }
            set {
                SetValue(CtrlEditModeProperty, value);
            }
        }
        #endregion

        #region Label
        public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached(
                "Label",
                typeof(string),
                typeof(FileCtrl),
                new PropertyMetadata());
        public string Label {
            get {
                return (string)GetValue(LabelProperty);
            }
            set {
                SetValue(LabelProperty, value);
            }
        }
        #endregion

        #region Value
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
                "Value",
                typeof(string),
                typeof(FileCtrl),
                new PropertyMetadata());
        public string Value {
            get {
                return (string)GetValue(ValueProperty);
            }
            set {
                SetValue(ValueProperty, value);
            }
        }
        #endregion

        bool _txtBlockDownloadIsVisible = true;
        public bool TxtBlockDownloadIsVisible {
            get => _txtBlockDownloadIsVisible;
            set {
                _txtBlockDownloadIsVisible = value;
                this.NotifyPropertyChanged();
            }
        }

        bool _txtBlockUploadIsVisible;
        public bool TxtBlockUploadIsVisible {
            get => _txtBlockUploadIsVisible;
            set {
                _txtBlockUploadIsVisible = value;
                this.NotifyPropertyChanged();
            }
        }
        #endregion
        public FileCtrl() {

            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        static void CtrlModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {

            var mode = (CtrlMode)e.NewValue;
            ((FileCtrl)d).TxtBlockDownloadIsVisible = mode == CtrlMode.Display ? true : false;
            ((FileCtrl)d).TxtBlockUploadIsVisible = mode == CtrlMode.Display ? false : true;
        }
    }
}
