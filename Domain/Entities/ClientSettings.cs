using System;

namespace Domain.Entities
{
    public interface IClientSettings
    {
        public string ClientId { get; }
        public string ClientSecret { get; }
    }

    public class ClientSettings : IClientSettings
    {
        public ClientSettings()
        {
            ClientId = Environment.GetEnvironmentVariable("clientID");
            ClientSecret = Environment.GetEnvironmentVariable("clientSecret");
        }
        public string ClientId { get; private set; }
        public string ClientSecret { get; private set;  }
    }
}
