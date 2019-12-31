using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            // var selfSigned = await CallApiSelfSignedWithXARRClientCertHeader();
            var client_intermediate_localhost = await CallApiClientIntermediateLocalhost();
            var intermediate_localhost = await CallApiWithintermediateLocalhost();
            try
            {
                // This cert must fail, it is trusted, but not valid checked in the cert validation event
                var selfSigned = await CallApiWithSelfSigned();
            }
            catch(Exception ex)
            {
                var message = ex.Message;
            }

            try
            {
                // This cert must fail, it is trusted, but not an incorrect dns
                var incorrectDns = await CallApiWithincorrectDns();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
            
        }

        private async Task<JsonDocument> CallApiSelfSignedWithXARRClientCertHeader()
        {
            try
            {
                var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "sts_dev_cert.pfx"), "1234");
                var client = _clientFactory.CreateClient();
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://localhost:44379/api/values"),
                    Method = HttpMethod.Get,
                };

                request.Headers.Add("X-ARR-ClientCert", cert.GetRawCertDataString());
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var data = JsonDocument.Parse(responseContent);
                    return data;
                }

                throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }
        private async Task<JsonDocument> CallApiWithSelfSigned()
        {
            try
            {
                // This is a NOT child of the root cert or the intermediate certificate, must fail
                //var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "sts_dev_cert.pfx"), "1234");
                var client = _clientFactory.CreateClient("self_signed");

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://localhost:44378/api/values"),
                    Method = HttpMethod.Get,
                };

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var data = JsonDocument.Parse(responseContent);

                    return data;
                }

                throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }
        private async Task<JsonDocument> CallApiWithintermediateLocalhost()
        {
            try
            {
                // This is a child created from the root cert, must work
                //var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "intermediate_localhost.pfx"), "1234");
                var client = _clientFactory.CreateClient("intermediate_localhost");

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://localhost:44378/api/values"),
                    Method = HttpMethod.Get,
                };

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var data = JsonDocument.Parse(responseContent);

                    return data;
                }

                throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }
        private async Task<JsonDocument> CallApiClientIntermediateLocalhost()
        {
            try
            {
                // This is a child created from the intermediate certificate which is a cert created from the root cert, must work
                //var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "client_intermediate_localhost.pfx"), "1234");
                var client = _clientFactory.CreateClient("client_intermediate_localhost");

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://localhost:44378/api/values"),
                    Method = HttpMethod.Get,
                };

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var data = JsonDocument.Parse(responseContent);

                    return data;
                }

                throw new ApplicationException($"Status code: {response.StatusCode}, Error: {response.ReasonPhrase}");
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }
        }

        private async Task<JsonDocument> CallApiWithincorrectDns()
        {
            try
            {
                // This is is created for the incorrect DNS, must fail
                //var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "incorrectdns.pfx"), "1234");
                var client = _clientFactory.CreateClient("incorrect_dns");

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://localhost:44378/api/values"),
                    Method = HttpMethod.Get,
                };

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var data = JsonDocument.Parse(responseContent);

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
