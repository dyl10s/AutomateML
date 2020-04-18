using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Website.Services
{
    public interface ITokenBuilder
    {
        public string GetToken(List<Claim> claims);

    }
}
