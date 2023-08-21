using System.ComponentModel;

namespace OmsiApiServer.App.Services.Configuration;

using Newtonsoft.Json;

public class ConfigV1
{
    [JsonProperty("Moonlight")]
    public OmsiClientData OmsiClient { get; set; } = new();

    public class OmsiClientData
    {
        [JsonProperty("AppUrl")]
        [Description("The url OmsiApiServer is accesible with from the internet")]
        public string? AppUrl { get; set; }

        [JsonProperty("Version")]
        [Description("Do not Edit used by the Apllication")]
        public string Version { get; set; } = "1.0.0";

        [JsonProperty("Security")] public Security Security { get; set; } = new();
        
        [JsonProperty("Database")] public DatabaseData Database { get; set; } = new();
    }
    
    public class Security
    {
        [JsonProperty("Secret")]
        [Description("Secret at least 32 Characters")]
        public string Secret { get; set; } = "";
        
        [JsonProperty("Expires")]
        [Description("Set how long a Token is Vialid in Day(s)")]
        public int Expires { get; set; }
    }
    
    public class DatabaseData
    {
        [JsonProperty("Database")] public string Database { get; set; } = "omsi_db";

        [JsonProperty("Host")] public string Host { get; set; } = "your.database.host";
        
        [JsonProperty("Password")]public string Password { get; set; } = "secret";

        [JsonProperty("Port")] public long Port { get; set; } = 3306;

        [JsonProperty("Username")] public string Username { get; set; } = "omsi_user";
    }


}