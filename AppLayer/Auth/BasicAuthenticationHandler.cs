using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DAL.EF;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            // Must have Authorization header
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization header.");

            // Parse Authorization: Basic base64(email:password)
            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out var authHeader))
                return AuthenticateResult.Fail("Invalid Authorization header.");

            if (!"Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.Fail("Invalid auth scheme.");

            if (string.IsNullOrWhiteSpace(authHeader.Parameter))
                return AuthenticateResult.Fail("Missing credentials.");

            string email;
            string password;

            try
            {
                var rawBytes = Convert.FromBase64String(authHeader.Parameter);
                var raw = Encoding.UTF8.GetString(rawBytes);

                var parts = raw.Split(':', 2);
                if (parts.Length != 2)
                    return AuthenticateResult.Fail("Invalid Basic credentials format.");

                email = (parts[0] ?? "").Trim();
                password = (parts[1] ?? "").Trim();
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Base64 credentials.");
            }

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return AuthenticateResult.Fail("Email/password required.");

            // ✅ STRICT DB MATCH: Email AND Password must match
            // SQL Server is usually case-insensitive for Email by default, which is OK.
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user == null)
                return AuthenticateResult.Fail("Invalid email or password.");

            // ✅ Role comes from DB (Customer/Admin/Staff)
            var roleName = user.Role.ToString();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email ?? user.Name ?? "user"),
                new Claim(ClaimTypes.Role, roleName)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            // Helps clients (and Swagger) understand it needs Basic credentials
            Response.Headers["WWW-Authenticate"] = "Basic realm=\"CarRentalSystem\"";
            return base.HandleChallengeAsync(properties);
        }
    }
}