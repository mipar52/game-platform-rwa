using game_platform_rwa.DTOs;
using game_platform_rwa.Models;
using System.ComponentModel.DataAnnotations;

namespace game_platform_rwa.DTO_generator
{
    public static class UserDtoGenerator
    {
        public static UserDto generateUserDto(User user) 
        { 
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone
            };
        }
    }
}
