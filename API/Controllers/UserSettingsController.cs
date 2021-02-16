using System;
using IntegrationTestDemo.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IntegrationTestDemo.API.Models;
using System.Threading.Tasks;
using AutoMapper;
using System.Collections.Generic;
using IntegrationTestDemo.DAL.Models;

namespace IntegrationTestDemo.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [ApiController]
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/usersettings")]

    public class UserSettingsController : ControllerBase
    {
        private readonly IUserSettingRepository _userSettingRepository;
        private IMapper _mapper;
        public UserSettingsController(IUserSettingRepository userSettingRepository, IMapper mapper)
        {
            _userSettingRepository = userSettingRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> AddUserSetting([FromBody] UserSettingModel userSetting)
        {
            try
            {
                userSetting.UserId = HttpContext.User.Identity.Name;
                var setting = await _userSettingRepository.AddUserSetting(_mapper.Map<UserSetting>(userSetting));
                return Ok(_mapper.Map<UserSettingModel>(setting));
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
                var setting = await _userSettingRepository.GetUserSettingById(id);
                if (setting == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "User setting ID is not found");
                }
                else if (setting.UserId != HttpContext.User.Identity.Name)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "You do not own this setting.");
                }
                await _userSettingRepository.DeleteUserSetting(id);

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
                var settings = await _userSettingRepository.GetUserSettingByUserId(HttpContext.User.Identity.Name);
                return Ok(_mapper.Map<IEnumerable<UserSettingModel>>(settings));
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
                var setting = await _userSettingRepository.GetUserSettingById(id);
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
                var settings = await _userSettingRepository.GetUserSettingByUserId(userId);
                return Ok(_mapper.Map<IEnumerable<UserSettingModel>>(settings));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}