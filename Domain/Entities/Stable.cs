using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Stable
    {
        [JsonPropertyName("mounts")]
        public List<Mount> Mounts { get; set; }
    }

    /// <summary>
    /// Needed in order to deserialize Blizzard's JSON response.
    /// </summary>
    public class OuterStable
    {
        [JsonPropertyName("mounts")]
        public List<OuterMount>Mounts { get; set; }
    }
}
