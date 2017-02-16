using System;
using System.Configuration;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Movies.Dal;
using Movies.Service.Client;
using Movies.WebApi.Provider;
using Owin;
using SimpleInjector.Extensions.ExecutionContextScoping;

[assembly: OwinStartup(typeof(Movies.WebApi.Startup))]
namespace Movies.WebApi
{
    public class Startup
    {
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            EnsureAuthIndexes.Exist();
            HttpConfiguration config = new HttpConfiguration();
            DataProtectionProvider = app.GetDataProtectionProvider();
            var container = SimpleInjectorInitializer.Initialize(config);

            IClientService clientService;
            using (container.BeginExecutionContextScope())
            {
                clientService = container.GetInstance<IClientService>();
            }

            WebApiConfig.Register(config);
            app.UseOAuthAuthorizationServer(OAuthTokenOptions(clientService));
            app.UseJwtBearerAuthentication(OAuthTokenConsumption());
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        private OAuthAuthorizationServerOptions OAuthTokenOptions(IClientService clientService)
        {
            return new OAuthAuthorizationServerOptions
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new OAuthProvider(clientService),
                AccessTokenFormat = new Provider.JwtFormat(),
                RefreshTokenProvider = new RefreshTokenProvider(clientService)
            };
        }

        private JwtBearerAuthenticationOptions OAuthTokenConsumption()
        {
            var issuer = ConfigurationManager.AppSettings["Issuer"];
            string audienceId = ConfigurationManager.AppSettings["AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["AudienceSecret"]);

            return new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] { audienceId },
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                {
                    new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                }
            };
        }
    }
}