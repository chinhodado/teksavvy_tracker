using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.Web.Http;
using Newtonsoft.Json;
using TeksavvyData;

namespace teksavvy_tracker {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {
        public MainPage() {
            InitializeComponent();
            var model = new MainViewModel();
            DataContext = model;

            model.Update();
        }
    }

    public class MainViewModel {
        private ObservableCollection<Usage> _usages = new ObservableCollection<Usage>();
        public ObservableCollection<Usage> Usages {
            get {
                return _usages;
            }
        }

        public async Task Update() {
            var values = await GetAllMonthlyUsage();
            foreach (var usageData in values) {
                _usages.Add(new Usage {
                    Name = usageData.EndDate.Substring(0, 7), 
                    Amount = usageData.OnPeakDownload
                });
            }
        }

        private static async Task<IList<UsageData>> GetAllMonthlyUsage() {
            // get the data
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("TekSavvy-APIKey", "36A99E286BCA90747D4C6E03EA0E3C49"); // <- my API key
            HttpResponseMessage response = await httpClient.GetAsync(new Uri("https://api.teksavvy.com/web/Usage/UsageSummaryRecords"));
            response.EnsureSuccessStatusCode();
            string responseBodyAsText = await response.Content.ReadAsStringAsync();

            // and parse it
            var result = JsonConvert.DeserializeObject<TeksavvyJson>(responseBodyAsText);
            return result.Value;
        }
    }

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
