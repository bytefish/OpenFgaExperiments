//using Microsoft.OData.UriParser;

//namespace RebacExperiments.Server.Api.Infrastructure.Authentication
//{
//    public class AuthenticationHttpClient
//    {
//        private readonly HttpClient _client;

//        public AuthenticationHttpClient(HttpClient client)
//        {
//            _client = client;
//        }

//        public async Task<string?> GetOrCreateUserAsync(string provider, ExternalUserInfo userInfo)
//        {
//            var response = await _client.PostAsJsonAsync($"users/token/{provider}", userInfo);

//            if (!response.IsSuccessStatusCode)
//            {
//                return null;
//            }

//            var token = await response.Content.ReadFromJsonAsync<AuthenticationToken>();

//            return token?.Token;
//        }
//    }
//}
