using FptEcommerce.Core.DTOs;
using FptEcommerce.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FptEcommerce.Core.Helper
{
    public class Token
    {
        // để truy xuất những claim trong context tại thời điểm hiện tại (request hiện tại)
        // mà ở bên ngoài controller, ta dùng IHttpContextAccessor
        // còn bên MVC thuần là httpcontext.current.
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //public Token(IHttpContextAccessor httpContextAccessor)
        //{
        //    _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        //}

        // generate access token
        public static string GenerateToken(string secretKey, CustomerInfoDTO user, int hour, int minute)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    //new Claim(ClaimTypes.Name, user.FullName),  // Name, NameIdentifier, Role
                    //new Claim(ClaimTypes.Email, user.Email),
                    // roles: List<string>
                    //new Claim(ClaimTypes.Role, JsonSerializer.Serialize(roles), JsonClaimValueTypes.JsonArray),

                    //new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    //new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                    new Claim("UserName", user.Username),
                    new Claim("Email", user.Email == null ? "" : user.Email),
                    new Claim("Id", user.CustomerId.ToString()),

                    //roles

                    new Claim("TokenId", Guid.NewGuid().ToString())
                }),

                Expires = DateTime.UtcNow.AddMinutes(hour * 60 + minute),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            return jwtTokenHandler.WriteToken(token);
        }

        //generate refresh token
        public static string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        // Validate
        public static dynamic ValidateToken(string secretKey, string token)
        {
            var key = Encoding.ASCII.GetBytes(secretKey);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);

                var result = new
                {
                    NameClaim = claims.FindFirst(ClaimTypes.Name)?.Value,
                    EmailClaim = claims.FindFirst(ClaimTypes.Email)?.Value,
                    Username = claims.FindFirst("UserName")?.Value,
                    Email = claims.FindFirst("Email")?.Value,
                    Id = claims.FindFirst("Id")?.Value
                };
                //var x = 1;
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static int ValidateToken2(string secretKey, string token)
        {
            var key = Encoding.ASCII.GetBytes(secretKey);
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);

                var result = Convert.ToInt32(claims.FindFirst("Id")?.Value);
                //var x = 1;
                return result;
            }
            catch
            {
                return -1;
            }
        }

        //public int ValidateToken3()
        //{
        //    var userClaims = _httpContextAccessor.HttpContext.User.Claims;

        //    var id = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
        //    var authorities = userClaims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
        //    return Int32.Parse(id);
        //}

        // Cách này hay vì thậm chí ko cần secret Key
        public static Dictionary<string, string> GetInfo2(string token)
        {
            var TokenInfo = new Dictionary<string, string>();

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var claims = jwtSecurityToken.Claims.ToList();

            foreach (var claim in claims)
            {
                TokenInfo.Add(claim.Type, claim.Value);
            }

            return TokenInfo;
        }

    }
}
