using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class AccessToken
    {
        [JsonPropertyName("access_token")]
        public string Value { get; set; }
        
        [JsonPropertyName("token_type")]
        public string Type { get; set; }
        
        [JsonPropertyName("expires_in")]
        public int Expires { get; set; } // TODO: Calculate when expiry is due.
    }
}
