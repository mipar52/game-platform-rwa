using AutoMapper;
using GamePlatformBL.DTOs;
using GamePlatformBL.Models;
using GamePlatformBL.Utilities;
using GamePlatformBL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePlatformBL.AutoMappers
{
    public class MappingProfile: AutoMapper.Profile
    {
        public MappingProfile()
        {
            // Models to DTOs (API)
            CreateMap<Game, GameDto>();
            CreateMap<Game, SimpleGameDto>();
            CreateMap<Genre, GenreDto>();
            CreateMap<Review, GameReviewDto>();
            CreateMap<Review, UserGameReviewDto>();
            CreateMap<GameType, GameTypeDto>();
            CreateMap<LogEntry, LogDto>();
            CreateMap<User, AdminUserDto>();
            CreateMap<GameGenre, GameGenreDto>();
            CreateMap<Genre, GenreDto>();


            // DTOs to Models (API)
            CreateMap<GameCreateDto, Game>();
            CreateMap<GenreCreateDto, Genre>();
            CreateMap<GameReviewCreateDto, Review>();
            CreateMap<GameTypeCreateDto, GameType>();
            CreateMap<EditUserDto, User>();
            CreateMap<UserDto, User>();
            CreateMap<GameGenreDto, GameGenre>();
            CreateMap<GenreDto, Genre>();

            // DTOs to ViewModels (MVC)
            CreateMap<GameGenreDto, GameGenreViewModel>();
            CreateMap<GenreDto, GenreViewModel>();
            CreateMap<GameDto, GameListViewModel>();
            CreateMap<GameTypeDto, GameTypeViewModel>();
            CreateMap<GameDto, GameViewModel>();
            CreateMap<SimpleGameDto, GameListViewModel>();
            CreateMap<SimpleGameDto, GameViewModel>();
            CreateMap<SimpleGameDto, AdminGameViewModel>();
            CreateMap<GameDto, GameDetailsViewModel>();
            CreateMap<GameReviewDto, GameReviewViewModel>();
            CreateMap<GameGenreDto, GameGenreViewModel>();

            // ViewModels to DTOs (MVC)
            CreateMap<UserViewModel, UserDto>();
            CreateMap<GameTypeViewModel, GameTypeDto>();
            CreateMap<GameGenreViewModel, GameGenreDto>();
            CreateMap<GenreViewModel, GenreDto>();

            // VMs to VMs
            CreateMap<GameViewModel, GameListViewModel>()
                .ForMember(dest => dest.GenreName,opt => opt.MapFrom(src => string.Join(", ", src.GameGenres.Select(g => g.Genre.Name))));
        }
    }
}
