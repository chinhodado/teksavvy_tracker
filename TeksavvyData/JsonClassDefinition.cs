using System.Collections.Generic;

namespace TeksavvyData {
    public struct UsageData {
        public string Date;
        public string StartDate;
        public string EndDate;
        public string OID;
        public bool IsCurrent;
        public double OnPeakDownload;
        public double OnPeakUpload;
        public double OffPeakDownload;
        public double OffPeakUpload;
    }

    public sealed class TeksavvyJson {
        // there is also odata.metadata in the original Json, but it's not needed

        public IList<UsageData> Value { get; set; }
    }
}
