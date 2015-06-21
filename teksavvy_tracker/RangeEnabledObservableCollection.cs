using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace teksavvy_tracker {
    /**
     * Basically a ObservableCollection but with the AddRange() method to avoid
     * firing CollectionChanged multiple times.
     */
    public class RangeEnabledObservableCollection<T> : ObservableCollection<T> {
        public RangeEnabledObservableCollection() {
        }

        public RangeEnabledObservableCollection(IEnumerable<T> collection) : base(collection) {
        }

        public RangeEnabledObservableCollection(List<T> list) : base(list) {
        }

        public void AddRange(IEnumerable<T> range) {
            foreach (var item in range) {
                Items.Add(item);
            }

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Reset(IEnumerable<T> range) {
            Items.Clear();
            AddRange(range);
        }
    }
}
