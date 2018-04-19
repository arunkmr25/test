using System.Linq;
using AutoMapper;
using connections.DTO;
using connections.Model;

namespace connections.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDTO>().ForMember(opt1 => opt1.PhotoUrl,opt2=> 
            {
                opt2.MapFrom(src =>src.Photos.FirstOrDefault(p =>p.IsMain).Url);
            }).
            ForMember(opt1 => opt1.Age,opt2=> 
            {
                opt2.MapFrom(d =>d.DateOfBirth.CalculateAge());
            });

            CreateMap<User,UserListDTO>().
            ForMember(opt1 => opt1.PhotoUrl,opt2=> 
            {
                opt2.MapFrom(src =>src.Photos.FirstOrDefault(p =>p.IsMain).Url);
            }).
            ForMember(opt1 => opt1.Age,opt2=> 
            {
                opt2.MapFrom(d =>d.DateOfBirth.CalculateAge());
            });

            CreateMap<Photo,PhotoDTO>();

            CreateMap<UserUpdateDTO,User>();

            CreateMap<PhotoForCreationDto,Photo>();

            CreateMap<Photo,PhotoForRetreival>();
            
            CreateMap<RegisterDtos,User>();
        }
    }
}