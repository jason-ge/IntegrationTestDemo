using IntegrationTestDemo.API.Models;
using IntegrationTestDemo.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationTestDemo.Services
{
    public class UserSettingService : IUserSettingService
    {
        private readonly UserSettingContext _context;
        public UserSettingService(UserSettingContext context)
        {
            _context = context;
        }

        public async Task<UserSettingModel> AddUserSetting(UserSettingModel userSetting)
        {
            var setting = await _context.UserSettings.FirstOrDefaultAsync(s => s.UserId == userSetting.UserId && s.SettingKey == userSetting.SettingKey);
            if (setting != null)
            {
                throw new ArgumentException("The user setting already exists");
            }
            else
            {
                setting = new UserSetting
                {
                    SettingKey = userSetting.SettingKey,
                    SettingValue = userSetting.SettingValue,
                    UserId = userSetting.UserId
                };
                await _context.UserSettings.AddAsync(setting);
                await _context.SaveChangesAsync();

                userSetting.UserSettingId = setting.UserSettingId;
                return userSetting;
            }
        }

        public async Task<UserSettingModel> UpdateUserSetting(UserSettingModel userSetting)
        {
            var setting = await _context.UserSettings.FirstOrDefaultAsync(s => s.UserSettingId == userSetting.UserSettingId && s.SettingKey == userSetting.SettingKey);
            if (setting == null)
            {
                return null;
            }
            else
            {
                setting.SettingValue = userSetting.SettingValue;
                await _context.SaveChangesAsync();

                return userSetting;
            }
        }
        public async Task DeleteUserSetting(int id)
        {
            var setting = await _context.UserSettings.FirstOrDefaultAsync(s => s.UserSettingId == id);
            if (setting == null)
            {
                throw new ArgumentException("The user setting does not exist");
            }
            else
            {
                _context.UserSettings.Remove(setting);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserSettingModel> GetUserSettingById(int id)
        {
            var setting = await _context.UserSettings.FirstOrDefaultAsync(s => s.UserSettingId == id);
            if (setting == null)
            {
                return null;
            }
            else
            {
                return new UserSettingModel {
                    SettingKey = setting.SettingKey,
                    SettingValue = setting.SettingValue,
                    UserId = setting.UserId,
                    UserSettingId = setting.UserSettingId
                };
            }
        }

        public async Task<IEnumerable<UserSettingModel>> GetUserSettingByUserId(string userId)
        {
            return await _context.UserSettings.Where(s => s.UserId == userId).Select(s => new UserSettingModel
            {
                SettingKey = s.SettingKey,
                SettingValue = s.SettingValue,
                UserId = s.UserId,
                UserSettingId = s.UserSettingId
            }).ToListAsync();
        }
    }
}
