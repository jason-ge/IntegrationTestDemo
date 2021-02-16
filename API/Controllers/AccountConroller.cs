using IntegrationTestDemo.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IntegrationTestDemo.API.Controllers
{
    [EnableCors("SiteCorsPolicy")]
    [ApiController]
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/account")]
    public class AccountConroller : ControllerBase
    {
        private IConfiguration _configuration;

        public AccountConroller(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody]LoginRequest login)
        {
            var authClaims = new List<Claim>();
            if (login.Username.StartsWith("testuser"))
            {
                authClaims.Add(new Claim(ClaimTypes.Name, login.Username));
                authClaims.Add(new Claim(ClaimTypes.Role, "User"));
            }
            else if (login.Username.StartsWith("testadmin"))
            {
                authClaims.Add(new Claim(ClaimTypes.Name, login.Username));
                authClaims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else
            {
                return Unauthorized();
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new LoginResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ValidUntil = token.ValidTo
            });
        }
    }
}
