using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace JasperSiteCore.Models.Database
{
    public partial class User
    {
        /// <summary>
        /// Porovná zadané heslo s heslem v databázi.
        /// </summary>
        /// <param name="inputPassword">Heslo pro ověření.</param>
        /// <returns></returns>
        /// <exception cref="DatabaseHelperException"></exception>
        public bool ComparePassword(string inputPassword)
        {
            try
            {
                string dbHashedPassword = this.Password;
                string dbSalt = this.Salt;

                var salt = Convert.FromBase64String(dbSalt);
                var v = new Rfc2898DeriveBytes(inputPassword, salt, 10000);
                var vHash = v.GetBytes(20);

                var pswdFromDb = Convert.FromBase64String(dbHashedPassword);

                for (int i = 0; i < 20; i++)
                {
                    if (vHash[i] != pswdFromDb[i])
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {

                throw new DatabaseHelperException(ex) ;
            }
        }
        
       
    }

}
