using Domain.Entities;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Text.Json;

namespace MountSnooper.Authorization
{
    public interface IAuthenticator 
    {
        AccessToken Token { get; }
    }
    public class Authenticator : IAuthenticator
    {
        private readonly ClientSettings _clientSettings;
        public AccessToken Token { get; private set; } // TODO: Check on get if expired, refresh if so.
        public Authenticator(IOptions<ClientSettings> settings)
        {
            _clientSettings = settings.Value;
            Token = RequestAccessToken(
                _clientSettings.ClientId, 
                _clientSettings.ClientSecret
                );
        }
        private AccessToken RequestAccessToken(string clientId, string clientSecret)
        {
            var client = new RestClient("https://eu.battle.net/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            AccessToken atoken = JsonSerializer.Deserialize<AccessToken>(response.Content);

            return atoken;
        }
    }
}
