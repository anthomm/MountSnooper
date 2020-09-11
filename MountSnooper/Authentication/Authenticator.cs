using Domain.Entities;
using Domain.JSON;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Text.Json;

namespace MountSnooper.Authentication
{
    public interface IAuthenticator
    {
        AccessToken Token { get; }
    }
    public class Authenticator : IAuthenticator
    {
        private readonly ClientSettings _clientSettings;
        private AccessToken _token;
        public AccessToken Token
        {
            get
            {
                if (_token.HoursSinceCreated() >= 20) // Blizzard Access Tokens last 24 hours
                    _token = RequestAccessToken(_clientSettings);

                return _token;
            }

            private set { _token = value; }
        }
        public Authenticator(IOptions<ClientSettings> settings)
        {
            _clientSettings = settings.Value;
            Token = RequestAccessToken(_clientSettings);
        }
        private AccessToken RequestAccessToken(ClientSettings settings)
        {
            string clientId = settings.ClientId;
            string clientSecret = settings.ClientSecret;

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
