using Domain.DTOs;
using Domain.Entities;
using MountSnooper.Authorization;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MountSnooper.Communication
{
    public interface IRequester
    {
        IEnumerable<MountDTO> PlayerMounts(string name, string region, string realm);
        public IEnumerable<Mount> AllMounts(string region="eu");
        public string ImageURL(int mountId, string region = "eu");
    }
    public class Requester : IRequester
    {
        private readonly IAuthenticator _auth;
        private readonly string accessToken;
        private readonly string profileApiNamespace = "profile-eu";
        private readonly string gameDataApiNamespace = "static-eu";
        private readonly string locale = "en_GB";

        public Requester(IAuthenticator auth)
        {
            _auth = auth;
            accessToken = _auth.Token.Value;
        }

        public IEnumerable<MountDTO> PlayerMounts(string name, string region, string realm)
        {
            RestClient client = new RestClient(
                $"https://{region.ToLower()}.api.blizzard.com/profile/wow/character/{realm.ToLower()}/{name.ToLower()}/collections/mounts?namespace={profileApiNamespace}&locale={locale}");

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", $"Bearer {accessToken}");

            IRestResponse response = client.Execute(request);

            OuterStable mounts = JsonSerializer.Deserialize<OuterStable>(response.Content);
            List<MountDTO> mountDTOs = new List<MountDTO>();
            foreach (OuterMount m in mounts.Mounts)
            {
                mountDTOs.Add(new MountDTO() { Name = m.Mount.Name });
            }

            return mountDTOs;
        }

        public IEnumerable<Mount> AllMounts(string region)
        {
            RestClient client = new RestClient(
                $"https://{region.ToLower()}.api.blizzard.com/data/wow/mount/index?namespace={gameDataApiNamespace}&locale={locale}");

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", $"Bearer {accessToken}");

            IRestResponse response = client.Execute(request);

            Stable mounts = JsonSerializer.Deserialize<Stable>(response.Content);

            return mounts.Mounts;
        }

        public string ImageURL(int mountId, string region)
        {
            int displayId = CreatureDisplayId(mountId, region);
            return CreatureDisplayMediaURL(displayId, region);
        }

        private int CreatureDisplayId(int mountId, string region)
        {
            RestClient client = new RestClient(
               $"https://{region}.api.blizzard.com/data/wow/mount/{mountId}?namespace={gameDataApiNamespace}&locale={locale}");

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", $"Bearer {accessToken}");

            IRestResponse response = client.Execute(request);

            OuterCreatureDisplay ocd = JsonSerializer.Deserialize<OuterCreatureDisplay>(response.Content);

            return ocd.Outer.ToList()[0].Id;
        }

        private string CreatureDisplayMediaURL(int displayId, string region)
        {
            RestClient client = new RestClient(
               $"https://{region}.api.blizzard.com/data/wow/media/creature-display/{displayId}?namespace={gameDataApiNamespace}&locale={locale}");

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", $"Bearer {accessToken}");

            IRestResponse response = client.Execute(request);

            OuterCreatureDisplayMedia ocdm = JsonSerializer.Deserialize<OuterCreatureDisplayMedia>(response.Content);

            return ocdm.Outer.ToList()[0].ImageURL;
        }
    }
}
