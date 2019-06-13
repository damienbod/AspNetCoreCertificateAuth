using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace AspNetCoreCertificateAuth.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IWebHostEnvironment _environment;

        public IndexModel(IHttpClientFactory clientFactory, IWebHostEnvironment environment)
        {
            _clientFactory = clientFactory;
            _environment = environment;
        }

        public async Task OnGetAsync()
        {
            var test = await GetApiDataAsync();
        }

        private async Task<JArray> GetApiDataAsync()
        {
            try
            {
                var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "sts_dev_cert.pfx"), "1234");

                var client = _clientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:44379/");

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://localhost:44379/api/values"),
                    Method = HttpMethod.Get,
                };

                request.Headers.Add("X-ARR-ClientCert", cert.Thumbprint);
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
