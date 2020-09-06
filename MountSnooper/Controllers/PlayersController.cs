using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using MountSnooper.Communication;
using System.ComponentModel.DataAnnotations;

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
        public ActionResult<PlayerDTO> Get([Required] string name, [Required] string region, [Required] string realm)
        {
            //TODO: Validate params
            //TODO: Try clause requests
            PlayerDTO boomer = new PlayerDTO()
            {
                Name = name,
                Mounts = _request.PlayerMounts(name, region, realm) //TODO: Async/await
            };
            
            return Ok(boomer);
        }
    }
}
