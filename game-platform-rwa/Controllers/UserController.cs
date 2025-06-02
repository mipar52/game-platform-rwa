using game_platform_rwa.DTOs;
using game_platform_rwa.Logger;
using game_platform_rwa.Models;
using game_platform_rwa.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace game_platform_rwa.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration config;
        private readonly GamePlatformRwaContext context;
        private readonly LogService logService;
        private readonly String? secureKey;
        public UserController(IConfiguration configuration, GamePlatformRwaContext context, LogService logService)
        {
            config = configuration;
            secureKey = config["JWT:SecureKey"];
            this.context = context;
            this.logService = logService;
        }

        //  https://jwt.io/  -> Token info
        [HttpGet("[action]")]
        public ActionResult GetToken()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // The same secure key must be used here to create JWT,
                // as the one that is used by middleware to verify JWT
                if (string.IsNullOrEmpty(secureKey))
                    return BadRequest("Something went wrong....");

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if there is such a username in the database already
                var trimmedUsername = userDto.Username.Trim();
                if (context.Users.Any(x => x.Username.Equals(trimmedUsername)))
                {
                    logService.Log($"Username {trimmedUsername} already exists", "Error");
                    return BadRequest($"Username {trimmedUsername} already exists");
                }

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
                };

                // Add user and save changes to database
                context.Add(user);
                context.SaveChanges();

                // Update DTO Id to return it to the client
                userDto.Id = user.Id;
                logService.Log($"Registered user with username='{trimmedUsername}'.", "Sucess");
                return Ok(userDto);

            }
            catch (Exception ex)
            {
                logService.Log($"Error with registering user with username='{userDto.Username}'. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("[action]")]
        public ActionResult Login([FromBody]LoginUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var genericLoginFail = "Incorrect username or password";

                // Try to get a user from database
                var existingUser = context.Users.FirstOrDefault(x => x.Username == userDto.Username);
                if (existingUser == null)
                {
                    logService.Log($"{genericLoginFail}. User: {userDto.Username}.", "Error");
                    return BadRequest(genericLoginFail);
                }

                // Check is password hash matches
                var b64hash = PasswordHashProvider.GetHash(userDto.Password, existingUser.PwdSalt);
                if (b64hash != existingUser.PwdHash)
                    return BadRequest(genericLoginFail);

                // Create and return JWT token
                var role = context.Roles.First(x => x.Id == existingUser.RoleId).Name;
                if (role == null)
                {
                    logService.Log($"Could not get correct data from user. User: {userDto.Username}.", "Error");
                    return BadRequest("Could not get correct data from user. Aborting.");
                }

                if (secureKey == null)
                {
                    logService.Log($"Could not get secure key needed for auth. User: {userDto.Username}.", "Error");
                    return BadRequest("Something went wrong...");
                }

                var serializedToken = JwtTokenProvider.CreateToken(
                    secureKey, 
                    120, 
                    userDto.Username,
                    role
                    );

                logService.Log($"Login successufuly. User: {userDto.Username}.", "Success");
                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                logService.Log($"Error with login. User: {userDto.Username}. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        [Authorize] // Only logged-in users can change password
        public IActionResult ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var username = identity?.FindFirst(ClaimTypes.Name)?.Value;

                var user = context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    logService.Log("User not found to change the password.", "Error");
                    return Unauthorized("User not found");
                }

                var currentHash = PasswordHashProvider.GetHash(dto.CurrentPassword, user.PwdSalt);
                if (currentHash != user.PwdHash)
                {
                    logService.Log("User tried using incorrect password for change.", "Error");
                    return BadRequest("Current password is incorrect");
                }

                var newSalt = PasswordHashProvider.GetSalt();
                var newHash = PasswordHashProvider.GetHash(dto.NewPassword, newSalt);

                user.PwdSalt = newSalt;
                user.PwdHash = newHash;
                context.SaveChanges();

                logService.Log("User successfully changed password.", "Success");
                return Ok("Password changed successfully");
            } catch (Exception ex)
            {
                logService.Log($"Could not change the users password. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        [Authorize(Roles = "Admin")] // Only Admins can promote
        public IActionResult PromoteUser(string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = context.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    logService.Log($"Could not find user with username={username} to promote to admin role", "Error");
                    return NotFound("User not found");
                }

                var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
                if (adminRole == null)
                {
                    logService.Log($"Could not find user with username={username} to promote to admin role", "Error");
                    return StatusCode(500, "Admin role not found");
                }

                user.RoleId = adminRole.Id;
                context.SaveChanges();
                logService.Log($"Succesfully promoted user with username={username} to admin role", "Success");
                return Ok($"{username} has been promoted to Admin");
            } catch (Exception ex)
            {
                logService.Log($"Could not promote to admin user with username={username}. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        // helper method to check the role of the user
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
