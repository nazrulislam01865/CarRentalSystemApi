using Azure.Core;
using DAL.EF;
using DAL.EF.Models.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace AppLayer.Auth
{

        public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            private readonly UMSContext _db;

            public BasicAuthenticationHandler(
                IOptionsMonitor<AuthenticationSchemeOptions> options,
                ILoggerFactory logger,
                UrlEncoder encoder,
                ISystemClock clock,
                UMSContext db) : base(options, logger, encoder, clock)
            {
                _db = db;
            }

            protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                    return AuthenticateResult.Fail("Missing Authorization header.");

                try
                {
                    var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                    if (!authHeader.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
                        return AuthenticateResult.Fail("Invalid auth scheme.");

                    var raw = authHeader.Parameter ?? "";
                    var credentialBytes = Convert.FromBase64String(raw);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

                    if (credentials.Length != 2)
                        return AuthenticateResult.Fail("Invalid Authorization header.");

                    var email = credentials[0];
                    var password = credentials[1];

                    var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.Role ==UserRole.Admin);
                    if (user == null)
                        return AuthenticateResult.Fail("Invalid email or password.");

                    var claims = new[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email ?? user.Name ?? "user"),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
                catch
                {
                    return AuthenticateResult.Fail("Invalid Authorization header.");
                }
            }
        }
}


