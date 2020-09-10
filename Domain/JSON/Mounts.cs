using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Domain.JSON
{
    public class Mounts
    {
        [JsonPropertyName("page")]
        public int CurrentPage { get; set; }
        
        [JsonPropertyName("pageCount")]
        public int PageCount { get; set; }
        
        [JsonPropertyName("results")]
        public List<MountResult> Results { get; set; }
    }

    public class MountResult
    {
        [JsonPropertyName("data")]
        public MountData Data { get; set; }
    }

    public class MountData
    {
        [JsonPropertyName("creature_displays")]
        public List<MountCreatureDisplay> CreatureDisplay { get; set; }
        
        [JsonPropertyName("name")]
        public MountNames Names { get; set; }

    }

    public class MountCreatureDisplay
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class MountNames
    {
        [JsonPropertyName("en_GB")]
        public string NameGB { get; set; }
    }
}
