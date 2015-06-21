using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using TeksavvyData;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;
using Windows.Web.Http;

namespace BackgroundTask {
    public sealed class UpdateTileBackgroundTask : IBackgroundTask {
        public async void Run(IBackgroundTaskInstance taskInstance) {
            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();

            string thisMonth = await GetCurrentMonthUsageOperation();
            updateTile(thisMonth);

            _deferral.Complete();
        }

        private static void updateTile(string peakDownloadThisMonth) {
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text01);

            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = "Used this month: ";
            tileTextAttributes[1].InnerText = peakDownloadThisMonth + " / 400 GB";

            TileNotification tileNotification = new TileNotification(tileXml) {
                ExpirationTime = DateTimeOffset.UtcNow.AddDays(1)
            };
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        private static IAsyncOperation<string> GetCurrentMonthUsageOperation() {
            return getCurrentMonthUsage().AsAsyncOperation();
        }

        private static async Task<string> getCurrentMonthUsage() {
            // get the data
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("TekSavvy-APIKey", "36A99E286BCA90747D4C6E03EA0E3C49"); // <- my API key
            HttpResponseMessage response = await httpClient.GetAsync(new Uri("https://api.teksavvy.com/web/Usage/UsageSummaryRecords"));
            response.EnsureSuccessStatusCode();
            string responseBodyAsText = await response.Content.ReadAsStringAsync();

            // and parse it
            var result = JsonConvert.DeserializeObject<TeksavvyJson>(responseBodyAsText);
            return result.Value.Last().OnPeakDownload + "";
        }
    }
}
