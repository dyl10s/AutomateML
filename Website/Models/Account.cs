using NPoco;
using System;
using System.Security.Cryptography;
using System.Text;
using Website.Objects;

namespace Website.Models
{
    [PrimaryKey("AccountId")]
    public class Account
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        [ResultColumn]
        public string Password { get; set; }
        public string Email { get; set; }

        public static ReturnResult<Account> RegisterAccount(IDatabase db, Account account)
        {
            var results = new ReturnResult<Account>();

            //Validate Email
            if(!account.Email.Contains("@") || !account.Email.Contains("."))
            {
                results.Success = false;
                results.ErrorMessage = "Please enter a valid email";
                return results;
            }

            //Validate Password
            if(account.Password.Length < 8)
            {
                results.Success = false;
                results.ErrorMessage = "Password must be 8 or more characters";
                return results;
            }

            //Validate Username
            if (account.Username.Length < 6)
            {
                results.Success = false;
                results.ErrorMessage = "Username must be 6 or more characters";
                return results;
            }

            //Make sure username does not exist
            if(db.SingleOrDefault<int?>("SELECT TOP 1 AccountId from Account WHERE Username = @0", account.Username) != null)
            {
                results.Success = false;
                results.ErrorMessage = "The username already exists";
                return results;
            }

            try
            {
                account.PasswordHash = HashPassword(account.Password);
                db.Insert(account);

                results.Item = account;
                return results;
            }
            catch(Exception e)
            {
                results.Success = false;
                results.ErrorMessage = "Unknown error. Please try again";
                results.Exception = e;
                return results;
            }
        }

        public static ReturnResult<Account> Login(IDatabase db, Account account)
        {
            var results = new ReturnResult<Account>();

            try
            {
                var loginUser = db.SingleOrDefault<Account>("SELECT * FROM Account WHERE Username = @0 AND PasswordHash = @1",
                    account.Username,
                    HashPassword(account.Password));

                if(loginUser != null)
                {
                    results.Item = loginUser;
                }
                else
                {
                    results.Success = false;
                    results.ErrorMessage = "Invalid username or password";
                }
            }
            catch(Exception e)
            {
                results.ErrorMessage = "Unknown error. Please try again.";
                results.Success = false;
            }

            return results;
        }

        private static byte[] HashPassword(string rawPassword)
        {
            byte[] getBytesFromRow = Encoding.UTF8.GetBytes(rawPassword);
            byte[] hash = new SHA1CryptoServiceProvider().ComputeHash(getBytesFromRow);
            return hash;
        }
    }
}
