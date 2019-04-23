using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIWithJwt.Models;
using Microsoft.Extensions.Configuration;
using APIWithJwt.Models.Hasher;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace APIWithJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserDatabaseContext _context;
        private readonly ICryptographyProcessor _crypto;
        private readonly IConfiguration _config;

        public UsersController(UserDatabaseContext context, ICryptographyProcessor crypto, IConfiguration config)
        {
            _context = context;
            _crypto = crypto;
            _config = config;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        //// GET: api/Users/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Users>> GetUsers(int id)
        //{
        //    var users = await _context.Users.FindAsync(id);

        //    if (users == null)
        //    {
        //        return NotFound();
        //    }

        //    return users;
        //}

        //// PUT: api/Users/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUsers(int id, Users users)
        //{
        //    if (id != users.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(users).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UsersExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Users
        [Authorize]
        [Route("CreateUser")]
        [HttpPost]
        public async Task<ActionResult<Users>> PostUsers(TokenRequest login)
        {
            string salt = _crypto.CreateSalt();
            Users user = new Users()
            {
                Salt = salt,
                Username = login.Username,
                Password = _crypto.GenerateHash(login.Password, salt),
                isActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsers", new { id = user.Id }, user);
        }

        //// DELETE: api/Users/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Users>> DeleteUsers(int id)
        //{
        //    var users = await _context.Users.FindAsync(id);
        //    if (users == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Users.Remove(users);
        //    await _context.SaveChangesAsync();

        //    return users;
        //}

        //private bool UsersExists(int id)
        //{
        //    return _context.Users.Any(e => e.Id == id);
        //}

        [Route("RequestToken")]
        [HttpPost]
        public ActionResult RequestToken([FromBody] TokenRequest request)
        {
          
            Users user = _context.Users.Where(x => x.Username == request.Username).FirstOrDefault();
            bool isActive = user.isActive ?? false;
            if (_crypto.AreEqual(request.Password,user.Password,user.Salt) && isActive)
            {
                var claims = new[]
                {
            new Claim(ClaimTypes.Name, request.Username)
        };
                string o = _config["SecurityKey"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "yourdomain.com",
                    audience: "yourdomain.com",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }

            return BadRequest("Could not verify username and password");
        }
    }
}
