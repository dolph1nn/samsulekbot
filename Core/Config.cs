using System;

namespace SSB.Core
{
    /// <summary>
    /// This is the class for the Config file.
    /// </summary>
    public class SSBConfig
    {
        public string Token { get; set; } = String.Empty;
        public string DBDriver { get; set; } = String.Empty;
        public string DBHostname { get; set; } = String.Empty;
        public string DBDatabase { get; set; } = String.Empty;
        public string DBPort { get; set; } = String.Empty;
        public bool DBSSL { get; set; } = false;
        public bool DBAuthType { get; set; } = false;
        public string DBUsername { get; set; } = String.Empty;
        public string DBPassword { get; set; } = String.Empty;
    }
}
