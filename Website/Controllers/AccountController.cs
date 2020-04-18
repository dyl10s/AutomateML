using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NPoco;
using Website.Models;
using Website.Objects;
using Website.Services;

namespace Website.Controllers
{
    [Route("api/[Controller]/[Action]")]
    public class AccountController : Controller
    {
        IDatabase Db;
        ITokenBuilder TokenBuilder;

        public AccountController(IDatabase db, ITokenBuilder tokenBuilder)
        {
            TokenBuilder = tokenBuilder;
            Db = db;
        }

        public ReturnResult<string> Register([FromBody]Account account)
        {
            var results = Account.RegisterAccount(Db, account);

            if (results.Success)
            {
                var tokenString = TokenBuilder.GetToken(new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                    new Claim(ClaimTypes.Name, account.Username),
                    new Claim(ClaimTypes.Role, "User")
                });

                return new ReturnResult<string>()
                {
                    Success = true,
                    Item = tokenString
                };
            }
            else
            {
                return new ReturnResult<string>()
                {
                    Success = false,
                    ErrorMessage = results.ErrorMessage
                };
            }
        }

        public ReturnResult<string> Login([FromBody]Account account)
        {
            var results = Account.Login(Db, account);
            if (results.Success)
            {
                var tokenString = TokenBuilder.GetToken(new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, results.Item.AccountId.ToString()),
                    new Claim(ClaimTypes.Name, results.Item.Username),
                    new Claim(ClaimTypes.Role, "User")
                });

                return new ReturnResult<string>()
                {
                    Success = true,
                    Item = tokenString
                };
            }
            else
            {
                return new ReturnResult<string>()
                {
                    Success = false,
                    ErrorMessage = results.ErrorMessage
                };
            }
        }
    }
}