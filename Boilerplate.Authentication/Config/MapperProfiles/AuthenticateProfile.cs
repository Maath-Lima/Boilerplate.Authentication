using AutoMapper;
using Boilerplate.Authentication.Data.Entities;
using Boilerplate.Authentication.Models.ResponseModels;

namespace Boilerplate.Authentication.Config.MapperProfiles
{
    public class AuthenticateProfile : Profile
    {
        public AuthenticateProfile()
        {
            CreateMap<Account, AuthenticateResponse>();
        }
    }
}
