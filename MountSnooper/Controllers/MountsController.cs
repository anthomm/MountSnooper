using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MountSnooper.Communication;
using System;
using System.Collections.Generic;

namespace MountSnooper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MountsController : ControllerBase
    {
        private readonly IRequester _request;
        private readonly IMemoryCache _cache;
        public MountsController(IRequester request, IMemoryCache memoryCache)
        {
            _request = request;
            _cache = memoryCache;
        }

        // GET: api/mounts
        /// <summary>
        /// Retrieve all mounts currently in the game along with a URL of their picture.
        /// </summary>
        /// <remarks>
        /// This endpoint is cached, but only after being primed.
        /// <br></br>
        /// Priming the cache usually takes about 3 seconds and it persists for about a day after each access or priming.
        /// </remarks>
        [HttpGet]
        public ActionResult<IEnumerable<MountDTO>> Get()
        {
            string cacheKey = "mounts";
            // Look for cache key.
            if (!_cache.TryGetValue(cacheKey, out List<MountDTO> cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = (List<MountDTO>)_request.AllMountsAndURLs();

                // Set cache options.
                var cacheEntryOptions =
                    // Keep in cache for this time, reset time if accessed.
                    new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(1));

                // Save data in cache.
                _cache.Set(cacheKey, cacheEntry, cacheEntryOptions);
            }
            return Ok(cacheEntry);
        }
    }
}
