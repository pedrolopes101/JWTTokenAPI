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
using Microsoft.AspNetCore.Http;
using APIWithJwt.Models.TokenModels;

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
        [ProducesResponseType(typeof(Users), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(typeof(Users), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost]
        public ActionResult<Users> CreateUser(LoginCredentials User)
        {

            if (String.IsNullOrEmpty(User.Username) || User.Username.All(char.IsDigit) || String.IsNullOrEmpty(User.Password) || User.Password.All(char.IsDigit))
                return BadRequest("Please make sure both fields are filled in correctly");

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
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public ActionResult RequestToken([FromBody] LoginCredentials request)
        {
            if (String.IsNullOrEmpty(request.Username) || request.Username.All(char.IsDigit) || String.IsNullOrEmpty(request.Password) || request.Password.All(char.IsDigit))
                return BadRequest("Sent value is not valid");
            Token token = new Token()
            {
                token = _userService.Authenticate(request.Username, request.Password)
            };

            if (String.IsNullOrEmpty(token.token))
            {
                return BadRequest("Could not verify username and password");
            }

            return Ok(new
            {
                token = token.token
            });
        }
    }
}
