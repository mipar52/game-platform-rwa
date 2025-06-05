using game_platform_rwa.DTOs;
using game_platform_rwa.Logger;
using game_platform_rwa.Models;
using game_platform_rwa.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace game_platform_rwa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<AdminUserDto>> GetAllUsers()
        {
            try
            {
                var users = context.Users
                    .Include(u => u.Role)
                    .Select(u => new AdminUserDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Phone = u.Phone,
                        RoleId = u.RoleId
                    })
                    .ToList();

                logService.Log($"Successfully obtained all users! User count: {users.Count()}", "Success");
                return Ok(users);
            }
            catch (Exception ex)
            {
                logService.Log($"Error getting all users: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        [Authorize]
        public ActionResult<AdminUserDto> GetUserById(int id)
        {
            try
            {
                var user = context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Id == id);

                if (user == null)
                    return NotFound("User not found.");

                var result = new AdminUserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.Phone,
                    RoleId = user.RoleId
                };

                logService.Log($"Successfully obtained with ID {result.Id}, username: {result.Username}!", "Success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logService.Log($"Error retrieving user by ID: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] EditUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    logService.Log($"User with ID {id} not found for update", "Error");
                    return NotFound($"User with ID {id} not found");
                }

                // Update editable fields
                user.Username = dto.Username;
                user.Email = dto.Email;
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Phone = dto.Phone;

                if (dto.ConfirmPassword != dto.ConfirmPassword)
                {
                    logService.Log($"Updated user with ID={id} failed! Passwords do not match!", "Error");
                    return StatusCode(400, "Passwords do not match!");
                }

                // Optional password update
                if (!string.IsNullOrWhiteSpace(dto.NewPassword))
                {
                    var newSalt = PasswordHashProvider.GetSalt();
                    var newHash = PasswordHashProvider.GetHash(dto.NewPassword, newSalt);

                    user.PwdSalt = newSalt;
                    user.PwdHash = newHash;
                }

                await context.SaveChangesAsync();

                logService.Log($"Updated user info for user ID={id}", "Success");
                return Ok("User updated successfully");
            }
            catch (Exception ex)
            {
                logService.Log($"Failed to update user ID={id}. Error: {ex.Message}", "Error");
                return StatusCode(500, "Internal server error");
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
                    role,
                    $"{existingUser.Id}"
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
                    logService.Log($"Could not find user with username={username} to promote to change the role", "Error");
                    return NotFound("User not found");
                }

                var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
                if (adminRole == null)
                {
                    logService.Log($"Could not find user with username={username} to promote to change the role", "Error");
                    return StatusCode(500, "Admin role not found");
                
                }
                user.RoleId = user.RoleId == adminRole.Id ? 2 : adminRole.Id;
                
                context.SaveChanges();
                logService.Log($"Succesfully promoted user with username={username} to role with id={user.RoleId}", "Success");
                return Ok($"{username} has been promoted to Admin");
            } catch (Exception ex)
            {
                logService.Log($"Could not promote to admin user with username={username}. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = context.Users.FirstOrDefault(u => u.Id == id);

                if (user == null)
                {
                    logService.Log($"User with ID={id} not found for deletion.", "Error");
                    return NotFound($"User with ID={id} not found");
                }

                context.Users.Remove(user);
                context.SaveChanges();

                logService.Log($"Successfully deleted user with ID={id} and username='{user.Username}'", "Success");
                return Ok($"User with ID={id} has been deleted");
            }
            catch (Exception ex)
            {
                logService.Log($"Failed to delete user with ID={id}. Error: {ex.Message}", "Error");
                return StatusCode(500, "Internal server error");
            }
        }


        // helper method to check the role of the user
        [HttpGet("whoami")]
        [Authorize]
        public IActionResult WhoAmI()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity?.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var id))
            {
                logService.Log($"Issues with getting the token information!", "Error");
                return BadRequest("Could not get all of the information from the token!");
            }

            var username = identity?.FindFirst(ClaimTypes.Name)?.Value;
            var role = identity?.FindFirst(ClaimTypes.Role)?.Value;
            logService.Log($"Succesfully got the token from the user with username={username}", "Success");
            return Ok(new { id, username, role });
        }
    }
}
