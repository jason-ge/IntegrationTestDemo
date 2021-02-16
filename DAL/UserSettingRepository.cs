using IntegrationTestDemo.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationTestDemo.DAL
{
    public class UserSettingRepository : IUserSettingRepository
    {
        private readonly UserSettingContext _context;
        public UserSettingRepository(UserSettingContext context)
        {
            _context = context;
        }

        public async Task<UserSetting> AddUserSetting(UserSetting userSetting)
        {
            var setting = await _context.UserSettings.FirstOrDefaultAsync(s => s.UserId == userSetting.UserId && s.SettingKey == userSetting.SettingKey);
            if (setting != null)
            {
                throw new ArgumentException("The user setting already exists");
            }
            else
            {
                await _context.UserSettings.AddAsync(userSetting);
                await _context.SaveChangesAsync();

                return userSetting;
            }
        }

        public async Task<UserSetting> UpdateUserSetting(UserSetting userSetting)
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

        public async Task<UserSetting> GetUserSettingById(int id)
        {
            var setting = await _context.UserSettings.FirstOrDefaultAsync(s => s.UserSettingId == id);
            if (setting == null)
            {
                return null;
            }
            else
            {
                return setting;
            }
        }

        public async Task<IEnumerable<UserSetting>> GetUserSettingByUserId(string userId)
        {
            return await _context.UserSettings.Where(s => s.UserId == userId).ToListAsync();
        }
    }
}
