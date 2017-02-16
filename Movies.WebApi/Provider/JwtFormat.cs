using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Thinktecture.IdentityModel.Tokens;

namespace Movies.WebApi.Provider
{
    public class JwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            string audienceId = ConfigurationManager.AppSettings["AudienceId"];
            string symmetricKeyAsBase64 = ConfigurationManager.AppSettings["AudienceSecret"];
            string issuer = ConfigurationManager.AppSettings["Issuer"];

            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);
            var signingKey = new HmacSigningCredentials(keyByteArray);

            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            var token = new JwtSecurityToken(issuer, audienceId, data.Identity.Claims, issued?.UtcDateTime, expires?.UtcDateTime, signingKey);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(token);

            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new System.NotImplementedException();
        }
    }
}