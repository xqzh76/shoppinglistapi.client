namespace Checkout.Helpers
{
    using System;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using IdentityModel.Client;

    public class AccessTokenClient
    {
        private AccessToken cachedToken;
        private DateTime expires = DateTime.MinValue;

        public string GetBearerToken()
        {
            if (DateTime.UtcNow > this.expires)
            {
                this.cachedToken = GetTokenFromIdentityServer().Result;
                this.expires = DateTime.UtcNow + this.cachedToken.ExpiresIn - TimeSpan.FromSeconds(10); // expires 10 seconds earlier
            }

            return $"Bearer {this.cachedToken.Token}";
        }

        private static async Task<AccessToken> GetTokenFromIdentityServer()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            var tokenClient = new TokenClient(disco.TokenEndpoint, AppSettings.ClientId, AppSettings.ClientSecret);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync(AppSettings.ClientScope);

            if (tokenResponse.IsError)
            {
                throw new AuthenticationException($"Unable to get token for {AppSettings.ClientId}");
            }

            return new AccessToken(tokenResponse.AccessToken, TimeSpan.FromSeconds(tokenResponse.ExpiresIn));
        }

        private class AccessToken
        {
            public AccessToken(string token, TimeSpan expiresIn)
            {
                this.Token = token;
                this.ExpiresIn = expiresIn;
            }

            public string Token { get; }

            public TimeSpan ExpiresIn { get; }
        }
    }
}