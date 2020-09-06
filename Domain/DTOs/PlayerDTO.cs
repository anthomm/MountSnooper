using System.Collections.Generic;

namespace Domain.DTOs
{
    public class PlayerDTO
    {
        public string Name { get; set; }
        public IEnumerable<MountDTO> Mounts { get; set; }
    }
}
