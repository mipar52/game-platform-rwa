using GamePlatformBL.DTOs;
using GamePlatformBL.Models;
using System.ComponentModel.DataAnnotations;

namespace GamePlatformBL.DTO_generator
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
