using APIWithJwt.Models.Hasher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APIWithJwt.Models.UserModels
{
    public class UserService : IUserService
    {
        private readonly UserDatabaseContext _context;
        private readonly ICryptographyProcessor _crypto;
        private readonly IConfiguration _config;

        public UserService(UserDatabaseContext context, ICryptographyProcessor crypto, IConfiguration config)
        {
            _context = context;
            _crypto = crypto;
            _config = config;
        }
        public String Authenticate(string username, string password)
        {
            Users user = _context.Users.Where(x => x.Username == username).FirstOrDefault();
            bool isActive = user.isActive ?? false;
            if (_crypto.AreEqual(password, user.Password, user.Salt) && isActive)
            {
                var claims = new[]
                {
            new Claim(ClaimTypes.Name, user.Id.ToString()),
             new Claim(ClaimTypes.Role, user.Role)
        };
                string o = _config["SecurityKey"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "pfslopes.com",
                    audience: "pfslopes.com",
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds);

                string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                return tokenString;
              
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<Users>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public Users GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Users CreateUser(LoginCredentials User)
        {
            string salt = _crypto.CreateSalt();
            Users user = new Users()
            {
                Salt = salt,
                Username = User.Username,
                Password = _crypto.GenerateHash(User.Password, salt),
                Role = Role.User,
                isActive = true
            };

            _context.Users.Add(user);

             _context.SaveChanges();

            return _context.Users.Where(x=>x.Id == user.Id).FirstOrDefault();
        }
    }
}
