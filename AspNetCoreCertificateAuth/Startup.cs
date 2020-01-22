using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace AspNetCoreCertificateAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });

            var clientCertificateIntermediate = new X509Certificate2("../Certs/client.pfx", "1234");
            var handlerClientCertificateIntermediate = new HttpClientHandler();
            handlerClientCertificateIntermediate.ClientCertificates.Add(clientCertificateIntermediate);

            services.AddHttpClient("client", c => {})
                .ConfigurePrimaryHttpMessageHandler(() => handlerClientCertificateIntermediate);

            var certificateIntermediate = new X509Certificate2("../Certs/intermediate_localhost.pfx", "1234");
            var handlerCertificateIntermediate = new HttpClientHandler();
            handlerCertificateIntermediate.ClientCertificates.Add(certificateIntermediate);

            services.AddHttpClient("intermediate_localhost", c => { })
                .ConfigurePrimaryHttpMessageHandler(() => handlerCertificateIntermediate);

            var selfSigned = new X509Certificate2("../Certs/sts_dev_cert.pfx", "1234");
            var handlerSelfSigned = new HttpClientHandler();
            handlerSelfSigned.ClientCertificates.Add(selfSigned);

            services.AddHttpClient("self_signed", c => { })
                .ConfigurePrimaryHttpMessageHandler(() => handlerSelfSigned);

            var incorrectDns = new X509Certificate2("../Certs/incorrectdns.pfx", "1234");
            var handlerIncorrectDns = new HttpClientHandler();
            handlerIncorrectDns.ClientCertificates.Add(incorrectDns);

            services.AddHttpClient("incorrect_dns", c => { })
                .ConfigurePrimaryHttpMessageHandler(() => handlerIncorrectDns);

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
