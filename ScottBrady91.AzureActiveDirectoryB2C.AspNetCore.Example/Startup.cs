using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ScottBrady91.AzureActiveDirectoryB2C.AspNetCore.Example
{
    public class Startup
    {
        public const string LocalAuthenticationScheme = "cookie";
        public const string ClientId = ""; // your application id
        public const string Tenant = ""; // the name of your directory
        public const string SignUpOrInPolicy = "B2C_1_Default_SignUpOrIn";
        public const string PasswordResetPolicy = "B2C_1_Default_Password";
        public const string ProfilePolicy = "B2C_1_Default_Profile";

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = LocalAuthenticationScheme
            });

            app.UseStaticFiles();

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                MetadataAddress = $"https://login.microsoftonline.com/{Tenant}/v2.0/.well-known/openid-configuration?p={SignUpOrInPolicy}",
                AuthenticationScheme = SignUpOrInPolicy,
                AutomaticChallenge = true,
                CallbackPath = new PathString($"/{SignUpOrInPolicy.ToLower()}"),
                PostLogoutRedirectUri = "/",
                ClientId = ClientId,
                SignInScheme = LocalAuthenticationScheme
            });
            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                MetadataAddress = $"https://login.microsoftonline.com/{Tenant}/v2.0/.well-known/openid-configuration?p={PasswordResetPolicy}",
                AuthenticationScheme = PasswordResetPolicy,
                AutomaticChallenge = false,
                CallbackPath = new PathString($"/{PasswordResetPolicy.ToLower()}"),
                PostLogoutRedirectUri = "/",
                ClientId = ClientId,
                SignInScheme = LocalAuthenticationScheme
            });
            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                MetadataAddress = $"https://login.microsoftonline.com/{Tenant}/v2.0/.well-known/openid-configuration?p={ProfilePolicy}",
                AuthenticationScheme = ProfilePolicy,
                AutomaticChallenge = false,
                CallbackPath = new PathString($"/{ProfilePolicy.ToLower()}"),
                PostLogoutRedirectUri = "/",
                ClientId = ClientId,
                SignInScheme = LocalAuthenticationScheme
            });
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
