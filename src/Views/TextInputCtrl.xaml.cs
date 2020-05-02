﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace SecureDataStore.Views {
    /// <summary>
    /// Interaktionslogik für TextInputCtrl.xaml
    /// </summary>
    public partial class TextInputCtrl : UserControl, INotifyPropertyChanged {
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

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        public TextInputCtrl() {

            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        static void CtrlModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {

            var mode = (CtrlMode)e.NewValue;
            ((TextInputCtrl)d).TextBlockIsVisible = mode == CtrlMode.Display ? true : false;
            ((TextInputCtrl)d).TextBoxIsVisible = mode == CtrlMode.Display ? false : true;
        }
    }
}
