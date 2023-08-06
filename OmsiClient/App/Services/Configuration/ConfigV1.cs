using System.ComponentModel;
using Newtonsoft.Json;

namespace OmsiClient.App.Services.Configuration;

public class ConfigV1
{
    public OmsiApiData OmsiClient { get; set; } = new();

    public class OmsiApiData
    {
        [JsonProperty("AppUrl")]
        [Description("The url to Connect to The Server")]
        public string AppUrl { get; set; } = "https://localhost:7223/";

        [JsonProperty("AuthData")] public AuthData AuthData { get; set; } = new();
    }
    
    public class AuthData
    {
        [JsonProperty("Token")]
        [Description("Your Token to Authenticate")]
        public string Token { get; set; } = "";
    }
}