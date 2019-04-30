using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIWithJwt.Models.Hasher
{
   public interface ICryptographyProcessor
    {
         string CreateSalt();


        string GenerateHash(string input, string salt);

        bool AreEqual(string plainTextInput, string hashedInput, string salt);

    }
}
