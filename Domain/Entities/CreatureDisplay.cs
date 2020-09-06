using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class CreatureDisplay
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    /// <summary>
    /// Needed in order to deserialize Blizzard's JSON response.
    /// </summary>
    public class OuterCreatureDisplay
    {
        [JsonPropertyName("creature_displays")]
        public IEnumerable<CreatureDisplay> Outer { get; set; }
    }

    public class CreatureDisplayMedia
    {
        [JsonPropertyName("value")]
        public string ImageURL { get; set; }
    }

    /// <summary>
    /// Needed in order to deserialize Blizzard's JSON response.
    /// </summary>
    public class OuterCreatureDisplayMedia
    {
        [JsonPropertyName("assets")]
        public IEnumerable<CreatureDisplayMedia> Outer { get; set; }
    }
}
