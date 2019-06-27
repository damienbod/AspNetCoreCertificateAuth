using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography;
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
            // var selfSigned = await GetApiDataAsyncSelfSigned();
            var chained = await GetApiDataAsyncChained(); 
        }

        private async Task<JArray> GetApiDataAsyncSelfSigned()
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

        private async Task<JArray> GetApiDataAsyncChained()
        {
            try
            {
                // This is a child created from the root cert, must work
                //var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "child_a_dev_damienbod.pfx"), "1234");

                // This is a child created from the child a cert created from the root cert, must work
                var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "child_b_from_a_dev_damienbod.pfx"), "1234");

                var chainPolicy = BuildChainPolicy(cert);
                var chain = new X509Chain
                {
                    ChainPolicy = chainPolicy
                };

                var certificateIsValid = chain.Build(cert);

                // This is a NOT child of the root cert, must fail
                //var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "sts_dev_cert.pfx"), "1234");

                var client = _clientFactory.CreateClient();

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://localhost:44378/api/values"),
                    Method = HttpMethod.Get,
                };

                request.Headers.Add("X-ARR-ClientCert", cert.GetRawCertDataString());
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

        private static readonly Oid ClientCertificateOid = new Oid("1.3.6.1.5.5.7.3.2");

        private X509ChainPolicy BuildChainPolicy(X509Certificate2 certificate)
        {
            // Now build the chain validation options.
            X509RevocationFlag revocationFlag = X509RevocationFlag.EntireChain;
            X509RevocationMode revocationMode = X509RevocationMode.NoCheck;

            var chainPolicy = new X509ChainPolicy
            {
                RevocationFlag = revocationFlag,
                RevocationMode = revocationMode
            };

            if (false)
            {
                chainPolicy.ApplicationPolicy.Add(ClientCertificateOid);
            }

            if (false)
            {
                chainPolicy.VerificationFlags |= X509VerificationFlags.AllowUnknownCertificateAuthority;
                chainPolicy.VerificationFlags |= X509VerificationFlags.IgnoreEndRevocationUnknown;
                chainPolicy.ExtraStore.Add(certificate);
            }

          
            return chainPolicy;
        }
    }
}
