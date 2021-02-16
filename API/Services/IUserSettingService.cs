using System.Collections.Generic;
using System.Threading.Tasks;
using IntegrationTestDemo.API.Models;

namespace IntegrationTestDemo.Services
{
    public interface IUserSettingService
    {
        Task<UserSettingModel> AddUserSetting(UserSettingModel userSetting);

        Task DeleteUserSetting(int id);

        Task<IEnumerable<UserSettingModel>> GetUserSettingByUserId(string userId);

        Task<UserSettingModel> GetUserSettingById(int id);
    }
}
