using IntegrationTestDemo.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationTestDemo.DAL
{
    public interface IUserSettingRepository
    {
        Task<UserSetting> AddUserSetting(UserSetting userSetting);

        Task DeleteUserSetting(int id);

        Task<IEnumerable<UserSetting>> GetUserSettingByUserId(string userId);

        Task<UserSetting> GetUserSettingById(int id);
    }
}
