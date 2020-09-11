using Domain.DTOs;
using Domain.JSON;
using MountSnooper.Authentication;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MountSnooper.Communication
{
    public interface IRequester
    {
        IEnumerable<MountDTO> PlayerMounts(string name, string region, string realm);
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

        public IEnumerable<MountDTO> PlayerMounts(string name, string region, string realm)
        {
            RestClient client = new RestClient(
                $"https://{region.ToLower()}.api.blizzard.com/profile/wow/character/{realm.ToLower()}/{name.ToLower()}/collections/mounts?namespace=profile-{region.ToLower()}&locale={locale}");

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", $"Bearer {_auth.Token.Value}");

            IRestResponse response = client.Execute(request);

            Player player = JsonSerializer.Deserialize<Player>(response.Content);
            List<MountDTO> mountDTOs = new List<MountDTO>();
            foreach (PlayerMount mount in player.Mounts)
            {
                mountDTOs.Add(new MountDTO() { Name = mount.MountInfo.Name});
            }

            return mountDTOs;
        }

        public IEnumerable<MountDTO> AllMountsAndURLs(string region = "eu")
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
