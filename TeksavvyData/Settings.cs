using System;

namespace TeksavvyData {
    internal sealed class Settings {
        // shitty WinRT doesn't allow public const in public class, for some bogus reasons...
        public const String ApiKey = "36A99E286BCA90747D4C6E03EA0E3C49";
        public const int DownloadLimit = 400;
    }
}
