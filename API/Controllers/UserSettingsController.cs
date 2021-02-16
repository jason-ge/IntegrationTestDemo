using System;
using IntegrationTestDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IntegrationTestDemo.API.Models;
using System.Threading.Tasks;

namespace IntegrationTestDemo.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [ApiController]
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/usersettings")]

    public class UserSettingsController : ControllerBase
    {
        private readonly IUserSettingService _userSettingService;

        public UserSettingsController(IUserSettingService userSettingService)
        {
            _userSettingService = userSettingService;
        }

        [HttpPost]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> AddUserSetting([FromBody] UserSettingModel userSetting)
        {
            try
            {
                userSetting.UserId = HttpContext.User.Identity.Name;
                return Ok(await _userSettingService.AddUserSetting(userSetting));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteUserSetting([FromRoute] int id)
        {
            try
            {
                var setting = await _userSettingService.GetUserSettingById(id);
                if (setting == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "User setting ID is not found");
                }
                else if (setting.UserId != HttpContext.User.Identity.Name)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "You do not own this setting.");
                }
                await _userSettingService.DeleteUserSetting(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserSettings()
        {
            try
            {
                return Ok(await _userSettingService.GetUserSettingByUserId(HttpContext.User.Identity.Name));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserSettingById([FromRoute]int id)
        {
            try
            {
                var setting = await _userSettingService.GetUserSettingById(id);
                if (setting == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Cannot find the user setting.");
                }
                else if (setting.UserId != HttpContext.User.Identity.Name)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "You do not own this user setting.");
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("user/{userId:regex(^\\w[[a-zA-Z0-9_.-]]*$)}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserSettingsByUserId([FromRoute]string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid user id");
            }

            try
            {
                return Ok(await _userSettingService.GetUserSettingByUserId(userId));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}