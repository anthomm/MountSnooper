using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using MountSnooper.Communication;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MountSnooper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IRequester _request;

        public PlayersController(IRequester request)
        {
            _request = request;
        }

        // GET api/players/{name}
        /// <summary>
        /// Retrieve all mounts belonging to a player's character.
        /// </summary>
        /// <remarks>
        /// Name - The character's name, eg: Denaya
        /// <br></br>
        /// Region - The region of the character's realm, eg: eu
        /// <br></br>
        /// Realm - The character's realm, eg: Outland
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="region"></param>
        /// <param name="realm"></param>
        [HttpGet("{name}")]
        public async Task<ActionResult<PlayerDTO>> Get([Required] string name, [Required] string region, [Required] string realm)
        {
            var (playerExists, mountDTOs) = await _request.PlayerMounts(name, region, realm);
            if (!playerExists)
                return BadRequest($"Unable to find character:{name} at realm:{realm} in region:{region}.");

            PlayerDTO playerDTO = new PlayerDTO()
            {
                Name = name,
                Mounts = mountDTOs
            };

            return Ok(playerDTO);
        }
    }
}
