using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Http;
using Newtonsoft.Json;

namespace TeksavvyData {
    /// <summary>
    /// Class for fetching Teksavvy data usage
    /// </summary>
    public static class TeksavvyDataSource {
        enum RequestType {
            UsageRecords,       // records for each day
            UsageSummaryRecords // records for each month
        }

        /// <summary>
        /// Cache the request
        /// </summary>
        private static Dictionary<string, string> requestCache = new Dictionary<string, string>();

        /// <summary>
        /// Get a response based on the request type
        /// </summary>
        /// <param name="type">The request type</param>
        /// <returns>The response from Teksavvy's server</returns>
        private static async Task<string> GetRequestResponse(RequestType type) {
            string requestUrl;
            switch (type) {
                case RequestType.UsageRecords:
                    requestUrl = "https://api.teksavvy.com/web/Usage/UsageRecords";
                    break;
                case RequestType.UsageSummaryRecords:
                    requestUrl = "https://api.teksavvy.com/web/Usage/UsageSummaryRecords";
                    break;
                default:
                    throw new Exception("Invalid type of request!");
            }

            if (!requestCache.ContainsKey(requestUrl)) {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("TekSavvy-APIKey", "36A99E286BCA90747D4C6E03EA0E3C49"); // <- my API key
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(requestUrl));
                response.EnsureSuccessStatusCode();
                string responseBodyAsText = await response.Content.ReadAsStringAsync();
                requestCache[requestUrl] = responseBodyAsText;
            }

            return requestCache[requestUrl];
        }

        /// <summary>
        /// Get all monthly usage
        /// </summary>
        /// <returns>A list of UsageData of all months</returns>
        public static IAsyncOperation<IList<UsageData>> GetAllMonthlyUsageOperation() {
            return GetAllMonthlyUsage().AsAsyncOperation();
        }

        private static async Task<IList<UsageData>> GetAllMonthlyUsage() {
            // get the data
            string response = await GetRequestResponse(RequestType.UsageSummaryRecords);

            // and parse it
            var result = JsonConvert.DeserializeObject<TeksavvyJson>(response);
            return result.Value;
        }

        /// <summary>
        /// Get all daily usage
        /// </summary>
        /// <returns>A list of UsageData of all days</returns>
        public static IAsyncOperation<IList<UsageData>> GetAllDailyUsageOperation() {
            return GetAllDailyUsage().AsAsyncOperation();
        }

        private static async Task<IList<UsageData>> GetAllDailyUsage() {
            // get the data
            string response = await GetRequestResponse(RequestType.UsageRecords);

            // and parse it
            var result = JsonConvert.DeserializeObject<TeksavvyJson>(response);
            return result.Value;
        }
    }
}
