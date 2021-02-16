using System;
using System.ComponentModel.DataAnnotations;

namespace IntegrationTestDemo.DAL.Models
{
    public class UserSetting
    {
        [Key]
        public int UserSettingId { get; set; }

        [Required]
        public string SettingKey { get; set; }

        [Required]
        public string SettingValue { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}
