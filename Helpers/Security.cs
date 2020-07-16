using Microsoft.IdentityModel.Tokens;
using OK_OnBoarding.Domains;
using OK_OnBoarding.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OK_OnBoarding.Helpers
{
    public static class Security
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace. Only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be whitespace or empty.Only string");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid Lenght of Password hash. 64 bytes expected.");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid Length of Password salt. 128 bytes expected.");

            using(var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i=0; i<computeHash.Length; i++)
                {
                    if (computeHash[i] != storedHash[i]) return false;
                }
            }
            return true;
        }

    }
}
