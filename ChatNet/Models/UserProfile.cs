using AutoMapper;
using ChatNet.Models.Dto;

namespace ChatNet.Models
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
