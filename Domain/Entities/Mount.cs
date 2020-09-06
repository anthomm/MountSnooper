using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Mount
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        public string ImageURL { get; set; }
    }

    /// <summary>
    /// Needed in order to deserialize Blizzard's JSON response.
    /// </summary>
    public class OuterMount
    {
        [JsonPropertyName("mount")]
        public Mount Mount { get; set; }

    }
}
