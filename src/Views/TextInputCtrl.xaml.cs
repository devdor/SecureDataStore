using System.Windows;

namespace SecureDataStore.Views {
    /// <summary>
    /// Interaktionslogik für TextInputCtrl.xaml
    /// </summary>
    public partial class TextInputCtrl : BaseCtrl {
        #region Fields and Properties
        #region CtrlMode
        public static readonly DependencyProperty CtrlEditModeProperty = DependencyProperty.RegisterAttached(
            "CtrlEditMode",
            typeof(CtrlMode),
            typeof(TextInputCtrl),
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
                typeof(TextInputCtrl),
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
                typeof(TextInputCtrl),
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
        public TextInputCtrl() {

            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        static void CtrlModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {

            var mode = (CtrlMode)e.NewValue;
            ((TextInputCtrl)d).TxtBlockIsVisible = mode == CtrlMode.Display ? true : false;
            ((TextInputCtrl)d).TxtBoxIsVisible = mode == CtrlMode.Display ? false : true;
        }
    }
}
