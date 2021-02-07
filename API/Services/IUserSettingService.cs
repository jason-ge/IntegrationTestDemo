using System;
using System.Collections.Generic;
using IntegrationTestDemo.API.Models;

namespace IntegrationTestDemo.Services
{
    public interface IUserSettingService
    {
        UserSettingModel AddUserSetting(UserSettingModel userSetting);

        int DeleteUserSetting(Guid id);

        IEnumerable<UserSettingModel> GetUserSettingByUserId(string userId);

        UserSettingModel GetUserSettingById(Guid id);
    }
}
