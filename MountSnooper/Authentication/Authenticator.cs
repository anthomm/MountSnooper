using Domain.Entities;
using Domain.JSON;
using Hangfire;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Text.Json;

namespace MountSnooper.Authentication
{
    public interface IAuthenticator
    {
        AccessToken Token { get; }
        void RefreshToken();
    }
    public class Authenticator : IAuthenticator
    {
        private readonly ClientSettings _clientSettings;
        private readonly int refreshInterval = 20; // Refreshes the access token every {resetInterval} hours.
        public AccessToken Token { get; set; }
        public Authenticator(IOptions<ClientSettings> settings)
        {
            _clientSettings = settings.Value;
            Token = RequestAccessToken(_clientSettings);
            BackgroundJob.Schedule<IAuthenticator>(service =>
                service.RefreshToken(),
                TimeSpan.FromHours(refreshInterval));
        }
        private AccessToken RequestAccessToken(ClientSettings settings)
        {
            var client = new RestClient("https://eu.battle.net/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", $"grant_type=client_credentials&client_id={settings.ClientId}&client_secret={settings.ClientSecret}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            AccessToken atoken = JsonSerializer.Deserialize<AccessToken>(response.Content);

            return atoken;
        }

        public void RefreshToken()
        {
            Token = RequestAccessToken(_clientSettings);
            BackgroundJob.Schedule<IAuthenticator>(service =>
                service.RefreshToken(),
                TimeSpan.FromHours(refreshInterval));
        }
    }
}
