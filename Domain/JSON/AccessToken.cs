using System;
using System.Text.Json.Serialization;

namespace Domain.JSON
{
    public class AccessToken
    {
        [JsonPropertyName("access_token")]
        public string Value { get; set; }
        
        [JsonPropertyName("token_type")]
        public string Type { get; set; }
        
        [JsonPropertyName("expires_in")]
        public int Expires { get; set; } 
    }
}
