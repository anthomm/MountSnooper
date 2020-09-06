using System;
using System.Collections.Generic;
using System.Linq;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using MountSnooper.Communication;
using Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

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
        /// Priming the cache usually takes about 5 minutes and it persists for about a day after each access or priming.
        /// </remarks>
        [HttpGet]
        public ActionResult<IEnumerable<MountDTO>> Get()
        {
            string cacheKey = "mounts";
            // Look for cache key.
            if (!_cache.TryGetValue(cacheKey, out List<MountDTO> cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = FullMountSet();
                //cacheEntry = PartialMountSet(); // For debugging

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

        private List<MountDTO> FullMountSet()
        {
            IEnumerable<Mount> rawMounts = _request.AllMounts();
            List<MountDTO> mounts = new List<MountDTO>();
            foreach (Mount mount in rawMounts)
            {
                mounts.Add(new MountDTO()
                {
                    Name = mount.Name,
                    ImageURL = _request.ImageURL(mount.Id) //TODO: Async/await
                });
            }
            return mounts;
        }

        //private List<MountDTO> PartialMountSet(int sizeOfSet = 20)
        //{
        //    IEnumerable<Mount> rawMounts = _request.AllMounts();
        //    List<MountDTO> mounts = new List<MountDTO>();
        //    Mount[] alphaArray = rawMounts.ToArray();
        //    for (int i = 0; i < sizeOfSet; i++)
        //    {
        //        mounts.Add(new MountDTO()
        //        {
        //            Name = alphaArray[i].Name,
        //            ImageURL = _request.ImageURL(alphaArray[i].Id)
        //        });
        //    }
        //    return mounts;
        //}
    }
}
