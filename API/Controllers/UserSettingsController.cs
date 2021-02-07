using System;
using IntegrationTestDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IntegrationTestDemo.API.Models;

namespace IntegrationTestDemo.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/usersettings")]

    public class UserSettingsController : ControllerBase
    {
        private readonly IUserSettingService _userSettingService;

        public UserSettingsController(IUserSettingService userSettingService)
        {
            _userSettingService = userSettingService;
        }

        [HttpPost("AddUserSetting")]
        [Authorize(Roles ="User")]
        public IActionResult AddUserSetting([FromBody] UserSettingModel userSetting)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, ModelState.ToString());
                }
                userSetting.UserId = HttpContext.User.Identity.Name;
                return Ok(_userSettingService.AddUserSetting(userSetting));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("DeleteUserSetting/{userSettingId}")]
        [Authorize(Roles = "User")]
        public IActionResult DeleteUserSetting([FromRoute] Guid userSettingId)
        {
            try
            {
                if (userSettingId == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Invalid user setting id");
                }
                var setting = _userSettingService.GetUserSettingById(userSettingId);
                if (setting == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "User setting ID is not found");
                }
                else if (setting.UserId != HttpContext.User.Identity.Name)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "You do not own this setting.");
                }
                int count = _userSettingService.DeleteUserSetting(userSettingId);

                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetUserSettings")]
        [Authorize(Roles = "User")]
        public IActionResult GetUserSettings()
        {
            try
            {
                return Ok(_userSettingService.GetUserSettingByUserId(HttpContext.User.Identity.Name));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetUserSettingsByUserId")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUserSettingsByUserId([FromQuery]string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid user id");
            }

            try
            {
                return Ok(_userSettingService.GetUserSettingByUserId(userId));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}