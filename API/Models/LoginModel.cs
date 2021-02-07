using System;
using System.ComponentModel.DataAnnotations;

namespace IntegrationTestDemo.API.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }

        public DateTime ValidUntil { get; set; }
    }

}
