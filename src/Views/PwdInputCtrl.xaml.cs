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
        bool _textBlockIsVisible = true;
        public bool TextBlockIsVisible {
            get => _textBlockIsVisible;
            set {
                _textBlockIsVisible = value;
                this.NotifyPropertyChanged();
            }
        }

        bool _textBoxIsVisible;
        public bool TextBoxIsVisible {
            get => _textBoxIsVisible;
            set {
                _textBoxIsVisible = value;
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
            ((PwdInputCtrl)d).TextBlockIsVisible = mode == CtrlMode.Display ? true : false;
            ((PwdInputCtrl)d).TextBoxIsVisible = mode == CtrlMode.Display ? false : true;
        }
    }
}
