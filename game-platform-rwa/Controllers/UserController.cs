using game_platform_rwa.DTOs;
using game_platform_rwa.Models;
using game_platform_rwa.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace game_platform_rwa.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration config;
        private readonly GamePlatformRwaContext context;

        public UserController(IConfiguration configuration, GamePlatformRwaContext context)
        {
            config = configuration;
            this.context = context;
        }

        //  https://jwt.io/  -> Token info
        [HttpGet("[action]")]
        public ActionResult GetToken()
        {
            try
            {
                // The same secure key must be used here to create JWT,
                // as the one that is used by middleware to verify JWT
                var secureKey = config["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(secureKey, 120);

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public ActionResult<UserDto> RegisterUser(UserDto userDto)
        {
            try
            {
                // Check if there is such a username in the database already
                var trimmedUsername = userDto.Username.Trim();
                if (context.Users.Any(x => x.Username.Equals(trimmedUsername)))
                    return BadRequest($"Username {trimmedUsername} already exists");

                // Hash the password
                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(userDto.Password, b64salt);

                // Create user from DTO and hashed password
                var user = new User
                {
                    Id = userDto.Id,
                    Username = userDto.Username,
                    PwdHash = b64hash,
                    PwdSalt = b64salt,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    Phone = userDto.Phone,
                    RoleId = 2, // Default role for new users
                    Role = context.Roles.FirstOrDefault(x => x.Name == "User")
                };

                // Add user and save changes to database
                context.Add(user);
                context.SaveChanges();

                // Update DTO Id to return it to the client
                userDto.Id = user.Id;

                return Ok(userDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("[action]")]
        public ActionResult Login(LoginUserDto userDto)
        {
            try
            {
                var genericLoginFail = "Incorrect username or password";

                // Try to get a user from database
                var existingUser = context.Users.FirstOrDefault(x => x.Username == userDto.Username);
                if (existingUser == null)
                    return BadRequest(genericLoginFail);

                // Check is password hash matches
                var b64hash = PasswordHashProvider.GetHash(userDto.Password, existingUser.PwdSalt);
                if (b64hash != existingUser.PwdHash)
                    return BadRequest(genericLoginFail);

                // Create and return JWT token
                var secureKey = config["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(secureKey, 120, userDto.Username, existingUser.Role.Name);

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        [Authorize] // Only logged-in users can change password
        public IActionResult ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var username = identity?.FindFirst(ClaimTypes.Name)?.Value;

            var user = context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return Unauthorized("User not found");

            var currentHash = PasswordHashProvider.GetHash(dto.CurrentPassword, user.PwdSalt);
            if (currentHash != user.PwdHash)
                return BadRequest("Current password is incorrect");

            var newSalt = PasswordHashProvider.GetSalt();
            var newHash = PasswordHashProvider.GetHash(dto.NewPassword, newSalt);

            user.PwdSalt = newSalt;
            user.PwdHash = newHash;
            context.SaveChanges();

            return Ok("Password changed successfully");
        }


        [HttpGet("whoami")]
        [Authorize]
        public IActionResult WhoAmI()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var username = identity?.FindFirst(ClaimTypes.Name)?.Value;
            var role = identity?.FindFirst(ClaimTypes.Role)?.Value;
            return Ok(new { username, role });
        }
    }
}
