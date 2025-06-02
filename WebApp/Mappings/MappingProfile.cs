/**
 * 
 * 
 * using AutoMapper;
using WebApp.Models;
using WebApp.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApp.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GameType, GameTypeViewModel>();
            // Add more mappings here as needed
        }
    }
}

 * 
 * 
 * 
 */
