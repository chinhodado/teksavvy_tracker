using De.TorstenMandelkow.MetroChart;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace teksavvy_tracker {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {
        /// <summary>
        /// Our main view model
        /// </summary>
        private MainViewModel model;

        /// <summary>
        /// The last period type chosen. I should have made the buttons radio boxes or something, but wtv
        /// </summary>
        private string lastPeriodType;

        public MainPage() {
            InitializeComponent();

            // create a chart and add it to our main page
            ColumnChart mc = new ColumnChart {Name = "ColumnChart"};
            Grid mainGrid = (Grid) FindName("MainGrid");
            mainGrid.Children.Add(mc);

            // set our data context
            model = new MainViewModel();
            DataContext = model;

            // default chart when starting up is the monthly onpeak download chart
            UpdateColumnChart("Month", null);
        }

        private async void MonthlyButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            await UpdateColumnChart("Month", null);
        }

        private async void DailyButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            await UpdateColumnChart("Day", null);
        }

        private async void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            string selection = (e.AddedItems[0] as ComboBoxItem).Content as string;
            await UpdateColumnChart(null, selection);
        }

        private async Task UpdateColumnChart(string periodType, string directionPeakType) {
            Grid mainGrid = (Grid) FindName("MainGrid");
            if (mainGrid == null) { // just started the app
                return;
            }
            ClusteredColumnChart chart =
                (ClusteredColumnChart) ((ColumnChart) mainGrid.FindName("ColumnChart")).FindName("Chart");
            ChartSeries series = chart.Series[0];

            if (periodType == null) {
                periodType = lastPeriodType;
            }

            series.SeriesTitle = periodType;
            lastPeriodType = periodType;

            if (directionPeakType == null) {
                directionPeakType =
                    ((ComboBoxItem) comboBox.SelectedItem).Content.ToString();
            }

            if (periodType == "Day") {
                chart.ChartTitle = directionPeakType + " in the last 30 days";
                await model.UpdateDaily(directionPeakType);
            }
            else if (periodType == "Month") {
                chart.ChartTitle = directionPeakType + " by months";
                await model.UpdateMonthly(directionPeakType);
            }
        }
    }
}
