using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AspNetCoreCertificateAuthHandler
{
    public class ApiService
    {
        private readonly IOptions<AuthConfigurations> _authConfigurations;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ApiTokenInMemoryClient _apiTokenInMemoryClient;

        public ApiService(
            IOptions<AuthConfigurations> authConfigurations, 
            IHttpClientFactory clientFactory,
            ApiTokenInMemoryClient apiTokenClient)
        {
            _authConfigurations = authConfigurations;
            _clientFactory = clientFactory;
            _apiTokenInMemoryClient = apiTokenClient;
        }

        public async Task<JArray> GetApiDataAsync()
        {
            try
            {
                var clientCertificate = new X509Certificate2("child_b_from_a_dev_damienbod.pfx", "1234");
                //var clientCertificate = new X509Certificate2("child_a_dev_damienbod.pfx", "1234");

                //var access_token = await _apiTokenInMemoryClient.GetApiToken(
                //    "ProtectedApi",
                //    "protected_scope",
                //    "protected_secret"
                //);

                // NOT working
                //var client = _clientFactory.CreateClient("api");
                //var response = await client.GetAsync("https://localhost:44378/api/values");

                // working
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://localhost:44378/api/values"),
                    Method = HttpMethod.Get,
                };

                var client = _clientFactory.CreateClient();
                request.Headers.Add("X-ARR-ClientCert", clientCertificate.GetRawCertDataString());
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var data = JArray.Parse(responseContent);

                    return data;
                }

                throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }
    }
}
