using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using APIWithJwt.Models.UserModels;

namespace APIWithJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
  
        private readonly ICryptographyProcessor _crypto;
        private readonly IUserService _userService;

        public UsersController(ICryptographyProcessor crypto, IUserService userService)
        {
      
            _crypto = crypto;
            _userService = userService;
        }

        // GET: api/Users
        [Authorize]
        [HttpGet]
        public IActionResult GetUsers()
        {
            var currentUserId = int.Parse(User.Identity.Name);
            if (!User.IsInRole(Role.Admin))
            {
                return Unauthorized();
            }
            else
            {
     
                return Ok(_userService.GetUsers());

            }
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
        [Authorize(Roles = Role.Admin)]
        [Route("CreateUser")]
        [HttpPost]
        public  ActionResult<Users> CreateUser(TokenRequest User)
        {

            if(User.Username == null || User.Password == null)
                return BadRequest("Please make sure both fields are filled in");

            return Ok(_userService.CreateUser(User));
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
          var token= _userService.Authenticate(request.Username, request.Password);
            if (token == null)
            {
                return BadRequest("Could not verify username and password");
            }

            return Ok(new
            {
                tokenString =  token
            });
        }
    }
}
