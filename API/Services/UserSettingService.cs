using IntegrationTestDemo.API.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntegrationTestDemo.Services
{
    public class UserSettingService : IUserSettingService
    {
        private readonly IMemoryCache _cache;
        public UserSettingService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public UserSettingModel AddUserSetting(UserSettingModel userSetting)
        {
            List<UserSettingModel> settings = _cache.Get<List<UserSettingModel>>("settings");
            if (settings == null)
            {
                settings = new List<UserSettingModel>();
            }
            var setting = settings.FirstOrDefault(s => s.UserId == userSetting.UserId && s.SettingKey == userSetting.SettingKey);
            if (setting != null)
            {
                throw new ArgumentException("The user setting already exists");
            }
            else
            {
                settings.Add(userSetting);
                _cache.Set("settings", settings);

                return userSetting;
            }
        }

        public int DeleteUserSetting(Guid id)
        {
            List<UserSettingModel> settings = _cache.Get<List<UserSettingModel>>("settings");
            if (settings == null)
            {
                return 0;
            }
            int count = settings.RemoveAll(s => s.UserSettingId == id);
            _cache.Set("settings", settings);
            return count;
        }

        public UserSettingModel GetUserSettingById(Guid id)
        {
            List<UserSettingModel> settings = _cache.Get<List<UserSettingModel>>("settings");
            if (settings == null)
            {
                return null;
            }
            else
            {
                return settings.Where(s => s.UserSettingId == id).FirstOrDefault();
            }
        }

        public IEnumerable<UserSettingModel> GetUserSettingByUserId(string userId)
        {
            List<UserSettingModel> settings = _cache.Get<List<UserSettingModel>>("settings");
            if (settings == null)
            {
                return null;
            }
            else
            {
                return settings.Where(s => s.UserId == userId).ToList();
            }
        }
    }
}
