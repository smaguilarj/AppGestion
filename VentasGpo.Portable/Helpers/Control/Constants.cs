using System;
namespace VentasGpo.Portable.Helpers
{
    public class Constants
    {
        public const bool _DEV = false;

        // -- urls
        const string API_DEV = "https://appscongelados.herdez.com:8092/api";
        const string API_PRO = "https://appscongelados.herdez.com:8091/api";

        public const string BASE_HERDEZ_API = _DEV ? API_DEV : API_PRO;

        // -- keys
        const string KEY_AC_DEV = "a0e69f5f-16c7-4795-b0ec-64d614aaec5c";
        const string KEY_AC_PROD = "b2da6862-fee8-4672-b1f0-9e80564eeab8";

        public const string APPCENTER_KEY_ANDROID = _DEV ? KEY_AC_DEV : KEY_AC_PROD;
    }
}
