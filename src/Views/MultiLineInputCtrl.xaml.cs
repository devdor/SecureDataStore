using System.Windows;

namespace SecureDataStore.Views {
    /// <summary>
    /// Interaktionslogik für MultiLineInputCtrl.xaml
    /// </summary>
    public partial class MultiLineInputCtrl : BaseCtrl {
        #region Fields and Properties
        #region CtrlMode
        public static readonly DependencyProperty CtrlEditModeProperty = DependencyProperty.RegisterAttached(
            "CtrlEditMode",
            typeof(CtrlMode),
            typeof(MultiLineInputCtrl),
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
                typeof(MultiLineInputCtrl),
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
                typeof(MultiLineInputCtrl),
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

        bool _txtBoxIsReadOnly = true;
        public bool TxtBoxIsReadOnly {
            get => _txtBoxIsReadOnly;
            set {
                _txtBoxIsReadOnly = value;
                this.NotifyPropertyChanged();
            }
        }
        #endregion
        public MultiLineInputCtrl() {

            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        static void CtrlModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {

            var mode = (CtrlMode)e.NewValue;
            ((MultiLineInputCtrl)d).TxtBoxIsReadOnly = mode == CtrlMode.Display ? true : false;
        }
    }
}
