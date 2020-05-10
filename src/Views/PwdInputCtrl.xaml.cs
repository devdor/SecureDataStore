using System.Windows;

namespace SecureDataStore.Views {
    /// <summary>
    /// Interaktionslogik für PwdInputCtrl.xaml
    /// </summary>
    public partial class PwdInputCtrl : BaseCtrl {
        #region Fields and Properties
        #region CtrlMode
        public static readonly DependencyProperty CtrlEditModeProperty = DependencyProperty.RegisterAttached(
            "CtrlEditMode",
            typeof(CtrlMode),
            typeof(PwdInputCtrl),
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
                typeof(PwdInputCtrl),
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
                typeof(PwdInputCtrl),
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
        bool _txtBlockIsVisible = true;
        public bool TxtBlockIsVisible {
            get => _txtBlockIsVisible;
            set {
                _txtBlockIsVisible = value;
                this.NotifyPropertyChanged();
            }
        }

        bool _txtBoxIsVisible;
        public bool TxtBoxIsVisible {
            get => _txtBoxIsVisible;
            set {
                _txtBoxIsVisible = value;
                this.NotifyPropertyChanged();
            }
        }
        #endregion
        public PwdInputCtrl() {

            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        static void CtrlModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {

            var mode = (CtrlMode)e.NewValue;
            ((PwdInputCtrl)d).TxtBlockIsVisible = mode == CtrlMode.Display ? true : false;
            ((PwdInputCtrl)d).TxtBoxIsVisible = mode == CtrlMode.Display ? false : true;
        }
    }
}
