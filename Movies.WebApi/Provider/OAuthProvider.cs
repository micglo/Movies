using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Movies.Dal;
using Movies.Domain.Entity;
using Movies.Service.Client;
using Movies.WebApi.IdentityConfig;

namespace Movies.WebApi.Provider
{
    public class OAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly IClientService _clientService;

        public OAuthProvider(IClientService clientService)
        {
            _clientService = clientService;
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                context.Validated();
                //context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }

            var client = _clientService.GetForOath(clientId);

            if (client == null)
            {
                context.SetError("invalid_clientId", $"Client '{context.ClientId}' is not registered in the system.");
                return Task.FromResult<object>(null);
            }

            if (!client.Active)
            {
                context.SetError("invalid_clientId", $"Client '{context.ClientId}' is deactivated.");
                return Task.FromResult<object>(null);
            }

            if (client.ApplicationType == ApplicationType.JavaScript)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult<object>(null);
                }
                if (client.Secret != _clientService.GetHash(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret is invalid.");
                    return Task.FromResult<object>(null);
                }
            }

            context.OwinContext.Set("clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set("clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("clientAllowedOrigin") ?? "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var movieContext = new Context();
            var userManager = new UserManager(new UserStore(movieContext.GetUserCollection()));
            User user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (!user.EmailConfirmed)
            {
                context.SetError("invalid_grant", "The user email is not confirmed.");
                return;
            }

            if (!user.IsActive)
            {
                context.SetError("invalid_grant", "The user is deleted.");
                return;
            }

            if (user.IsBanned)
            {
                context.SetError("invalid_grant", "The user is banned.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");
            //oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            var properties = new AuthenticationProperties(new Dictionary<string, string>
            {
                {
                    "client_id", context.ClientId ?? string.Empty
                }
            });
            var ticket = new AuthenticationTicket(oAuthIdentity, properties);

            context.Validated(ticket);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["client_id"];
            var currentClient = context.ClientId;

            if (!originalClient.Equals(currentClient))
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            //newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }
    }
}