using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace teksavvy_tracker {
    public class Usage : INotifyPropertyChanged {
        private string _name = string.Empty;
        private double _amount;

        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public double Amount {
            get {
                return _amount;
            }
            set {
                _amount = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
