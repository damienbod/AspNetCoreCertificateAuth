using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace AspNetCoreCertificateAuth.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task OnGetAsync()
        {
            var test = await GetApiDataAsync();
        }

        private async Task<JArray> GetApiDataAsync()
        {
            try
            {
                var client = _clientFactory.CreateClient();

                client.BaseAddress = new Uri("https://localhost:44379/");

                var response = await client.GetAsync("api/values");
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
