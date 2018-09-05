using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace JasperSite.Models.Security
{
    public class Authentication
    {
        /// <summary>
        /// Creates HASH from plain text and salt
        /// </summary>
        /// <param name="password">Plain text to be HASHED</param>
        /// <param name="salt">Salt</param>
        /// <param name="hashedPassword">Result = output hashed password</param>
        public static void HashPassword(string password, out string salt, out string hashedPassword)
        {

            // salt
            byte[] _salt = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(_salt);

            //hash
            var h = new Rfc2898DeriveBytes(password, _salt, 10000);
            var hash = h.GetBytes(20);

            salt = Convert.ToBase64String(_salt);
            hashedPassword = Convert.ToBase64String(hash);

        }
    }

    
}
