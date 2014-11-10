using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TeksavvyData;

namespace teksavvy_tracker {
    /// <summary>
    /// The view model for our main page
    /// </summary>
    public class MainViewModel {
        
        private ObservableCollection<Usage> _usages = new ObservableCollection<Usage>();
        public ObservableCollection<Usage> Usages {
            get {
                return _usages;
            }
        }

        /// <summary>
        /// Update the data to monthly usage
        /// </summary>
        /// <param name="directionPeakType">One of the magic strings depicting download/upload and onpeak/offpeak/total</param>
        /// <returns>A Task, in case you wanna await for it</returns>
        public async Task UpdateMonthly(string directionPeakType) {
            _usages.Clear();
            var values = await TeksavvyDataSource.GetAllMonthlyUsageOperation();
            foreach (var usageData in values) {
                _usages.Add(new Usage {
                    Name = usageData.EndDate.Substring(0, 7),
                    Amount = getUsageData(usageData, directionPeakType)
                });
            }
        }

        /// <summary>
        /// Update the data to daily usage
        /// </summary>
        /// <param name="directionPeakType">One of the magic strings depicting download/upload and onpeak/offpeak/total</param>
        /// <returns>A Task, in case you wanna await for it</returns>
        public async Task UpdateDaily(string directionPeakType) {
            _usages.Clear();
            var values = await TeksavvyDataSource.GetAllDailyUsageOperation();
            for (int i = values.Count - 30; i < values.Count; i++) {
                var usageData = values[i];
                _usages.Add(new Usage {
                    Name = usageData.Date.Substring(0, 10),
                    Amount = getUsageData(usageData, directionPeakType)
                });
            }
        }

        /// <summary>
        /// Get the usage amount based on whether it's download/upload and onpeak/offpeak/total
        /// </summary>
        /// <param name="usage">The usage</param>
        /// <param name="directionPeakType">One of the magic strings depicting download/upload and onpeak/offpeak/total</param>
        /// <returns></returns>
        private double getUsageData(UsageData usage, string directionPeakType) {
            switch (directionPeakType) { // ugly as hell, I know
                case "Download onpeak":
                    return usage.OnPeakDownload;
                case "Upload onpeak":
                    return usage.OnPeakUpload;
                case "Download offpeak":
                    return usage.OffPeakDownload;
                case "Upload offpeak":
                    return usage.OffPeakUpload;
                case "Download total":
                    return usage.OnPeakDownload + usage.OffPeakDownload;
                case "Upload total":
                    return usage.OnPeakUpload + usage.OffPeakUpload;
                default:
                    throw new Exception("Wrong directionPeakType, baka");
            }
        }
    }
}
