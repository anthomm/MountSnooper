using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Domain.JSON
{
    public class Player
    {
        [JsonPropertyName("mounts")]
        public List<PlayerMount> Mounts { get; set; }
    }

    public class PlayerMount
    {
        [JsonPropertyName("mount")]
        public PlayerMountInfo MountInfo { get; set; }
    }

    public class PlayerMountInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
