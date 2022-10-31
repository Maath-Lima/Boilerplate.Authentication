using AutoMapper;
using Boilerplate.Authentication.Data.Entities;
using Boilerplate.Authentication.Models.RequestModels;
using Boilerplate.Authentication.Models.ResponseModels;

namespace Boilerplate.Authentication.Config.MapperProfiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountResponse>();

            CreateMap<RegisterRequest, Account>();

            CreateMap<CreateRequest, Account>();

            CreateMap<UpdateRequest, Account>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        if (prop is null)
                        {
                            return false;
                        }

                        if (prop.GetType() == typeof(string) && string.IsNullOrWhiteSpace((string)prop))
                        {
                            return false;
                        }

                        if (x.DestinationMember.Name == "Role" && src.Role == null)
                        {
                            return false;
                        }

                        return true;
                    }));
        }
    }
}
