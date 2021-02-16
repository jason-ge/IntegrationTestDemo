using AutoMapper;
using IntegrationTestDemo.API.Models;
using IntegrationTestDemo.DAL.Models;

namespace IntegrationTestDemo.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserSetting, UserSettingModel>();

            CreateMap<UserSettingModel, UserSetting>();

        }
    }
}
