using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIWithJwt.Models.UserModels
{
   public interface IUserService
    {
        String Authenticate(string username, string password);
        Task<IEnumerable<Users>> GetUsers();
        Users GetById(int id);
        Users CreateUser(TokenRequest User);
    }
}
