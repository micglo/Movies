using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Infrastructure;
using Movies.Service.Client;

namespace Movies.WebApi.Provider
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        private readonly IClientService _clientService;

        public RefreshTokenProvider(IClientService clientService)
        {
            _clientService = clientService;
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new System.NotImplementedException();
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["client_id"];

            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");
            var hashedRefreshTokenId = _clientService.GetHash(refreshTokenId);
            var refreshTokenLifeTime = context.OwinContext.Get<string>("clientRefreshTokenLifeTime");

            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime));

            var protectedTicket = context.SerializeTicket();

            var result = await _clientService.UpdateRefreshToken(clientid, hashedRefreshTokenId, protectedTicket);

            if (result)
                context.SetToken(refreshTokenId);
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new System.NotImplementedException();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            string hashedTokenId = _clientService.GetHash(context.Token);

            var client = await _clientService.GetByRefreshToken(hashedTokenId);

            if (client != null)
            {
                context.DeserializeTicket(client.ProtectedTicket);
                await _clientService.RemoveRefreshToken(client.Id);
            }
        }
    }
}