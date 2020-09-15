using Domain.DTOs;
using Domain.JSON;
using MountSnooper.Authentication;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace MountSnooper.Communication
{
    public interface IRequester
    {
        /// <summary>
        /// <br>Checks if the character exists and if so retrieves the set of mounts belonging to that specific character.</br> 
        /// </summary>
        /// <param name="name">The character's name.</param>
        /// <param name="region">The region of the realm/server.</param>
        /// <param name="realm">The name of the realm/server.</param>
        /// <returns>
        /// <br>If the character was found, then playerExists will be true and all found mounts will be listed in mountDTOs.</br> 
        /// <br>If the character was NOT found, then playerExists will be false and mountDTOs will be null.</br> 
        /// </returns>
        Task<(bool playerExists, IEnumerable<MountDTO> mountDTOs)> PlayerMounts(string name, string region, string realm);

        /// <summary>
        /// Retrieves all mounts currently in the game along with a picture URL of each mount.
        /// </summary>
        /// <param name="region">In which region the pictures are hosted.</param>
        /// <returns>Set of MountDTOs</returns>
        IEnumerable<MountDTO> AllMountsAndURLs(string region = "eu");
    }
    public class Requester : IRequester
    {
        private readonly IAuthenticator _auth;
        private readonly string locale = "en_GB";

        public Requester(IAuthenticator auth)
        {
            _auth = auth;
        }


        public async Task<(bool playerExists, IEnumerable<MountDTO> mountDTOs)> PlayerMounts(string name, string region, string realm)
        {
            RestClient client = new RestClient(
                $"https://{region.ToLower()}.api.blizzard.com/profile/wow/character/{realm.ToLower()}/{name.ToLower()}/collections/mounts?namespace=profile-{region.ToLower()}&locale={locale}");

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", $"Bearer {_auth.Token.Value}");

            IRestResponse response = await client.ExecuteAsync(request);

            if ((int)response.StatusCode != 200)
                return (false, null);

            Player player = JsonSerializer.Deserialize<Player>(response.Content);
            List<MountDTO> mountDTOs = new List<MountDTO>();
            foreach (PlayerMount mount in player.Mounts)
            {
                mountDTOs.Add(new MountDTO() { Name = mount.MountInfo.Name });
            }

            return (true, mountDTOs);
        }

        public IEnumerable<MountDTO> AllMountsAndURLs(string region)
        {
            RestClient client = new RestClient(
               $"https://{region}.api.blizzard.com/data/wow/search/mount?namespace=static-{region}&locale=en_GB&orderby=id&_page=1");

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", $"Bearer {_auth.Token.Value}");

            List<MountDTO> dtos = new List<MountDTO>();

            int nextPage;
            int pageCount;
            do
            {
                IRestResponse response = client.Execute(request);
                Mounts mounts = JsonSerializer.Deserialize<Mounts>(response.Content);

                foreach (MountResult result in mounts.Results)
                {
                    dtos.Add(
                        new MountDTO()
                        {
                            Name = result.Data.Names.NameGB,
                            ImageURL = URLString(result.Data.CreatureDisplay[0].Id)
                        }
                    );
                }

                nextPage = mounts.CurrentPage + 1;
                pageCount = mounts.PageCount;

                client.BaseUrl =
                    new Uri($"https://{region}.api.blizzard.com/data/wow/search/mount?namespace=static-{region}&locale=en_GB&orderby=id&_page={nextPage}");

            } while (nextPage <= pageCount);

            return dtos;
        }

        private string URLString(int displayId, string region = "eu")
        {
            return $"https://render-{region}.worldofwarcraft.com/npcs/zoom/creature-display-{displayId}.jpg";
        }
    }
}
