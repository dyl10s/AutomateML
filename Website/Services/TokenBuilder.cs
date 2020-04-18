using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Website.Services
{
    public class TokenBuilder : ITokenBuilder
    {
        private byte[] SecretKey;
        private string Domain;

        public TokenBuilder(string domain, byte[] secretKey)
        {
            SecretKey = secretKey;
            Domain = domain;
        }

        public string GetToken(List<Claim> claims)
        {
            var tokenKey = new SymmetricSecurityKey(SecretKey);
            var signinCredentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                issuer: Domain,
                audience: Domain,
                claims: claims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: signinCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }
    }
}
