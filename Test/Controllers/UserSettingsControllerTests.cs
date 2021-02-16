using IntegrationTestDemo.API.Models;
using IntegrationTestDemo.IntegrationTests.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTestsDemo.IntegrationTests.Controllers
{
    public class UserSettingsControllerTests
    {
        private HttpClient client;
        private LoginResponse _jwtToken;

        public UserSettingsControllerTests()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configuration = (new ConfigurationBuilder().SetBasePath(projectDir).AddJsonFile("appsettings.json")).Build();
            var builder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .UseEnvironment("Development")
                .UseStartup<TestStartup>();
            //builder.Configure(app =>
            //{
            //    var seeder = app.ApplicationServices.GetService<TestDatabaseSeeder>();
            //    seeder.SeedUserSettings();
            //});
            TestServer testServer = new TestServer(builder);
            client = testServer.CreateClient();
        }

        [Fact]
        public async Task UserSetting_Test()
        {
            // Arrange
            // Login as testuser2
            _jwtToken = await Login("testuser2");
            UserSettingModel setting3 = new UserSettingModel
            {
                SettingKey = "test key3",
                SettingValue = "test value3",
                UserId = "testuser2"
            };
            setting3 = await AddUserSetting_Verify(setting3);

            // Login as testuser1
            _jwtToken = await Login("testuser1");
            UserSettingModel setting1 = new UserSettingModel
            {
                SettingKey = "test key1",
                SettingValue = "test value1",
                UserId = "testuser1"
            };
            UserSettingModel setting2 = new UserSettingModel
            {
                SettingKey = "test key2",
                SettingValue = "test value2",
                UserId = "testuser1"
            };
            setting1 = await AddUserSetting_Verify(setting1);
            setting2 = await AddUserSetting_Verify(setting2);

            await Get_UserSetting(2);
            await Get_UserSetting_Unauthorized();
            await Delete_UserSetting_Successful(setting1.UserSettingId);
            await Delete_UserSetting_NotAuthorized(setting3.UserSettingId);
            await Delete_UserSetting_NotFound(10000);
            await Get_UserSetting_NotFound(setting1.UserSettingId);

            await Get_UserSetting_ByUserId_Unauthorized("testuser2");

            // Login as admin
            _jwtToken = await Login("testadmin");
            await Get_UserSetting_ByUserId_Successful("testuser2", 1);
        }

        private async Task<LoginResponse> Login(string name)
        {
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "v1/account/login")
            {
                Content = new JsonContent(new LoginRequest() { Username = name, Password = "Password@1" })
            };

            HttpResponseMessage response = await client.SendAsync(postRequest);
            response.EnsureSuccessStatusCode();

            // Get the response as a string
            return JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());
        }

        private async Task<UserSettingModel> AddUserSetting_Verify(UserSettingModel setting)
        {
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "v1/usersettings")
            {
                Content = new JsonContent(setting)
            };
            postRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken.Token);

            HttpResponseMessage response = await client.SendAsync(postRequest);
            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<UserSettingModel>(await response.Content.ReadAsStringAsync());
            Assert.IsType<UserSettingModel>(result);
            Assert.Equal(result.SettingKey, setting.SettingKey);
            Assert.Equal(result.SettingValue, setting.SettingValue);
            Assert.Equal(result.UserId, setting.UserId);

            return result;
        }

        private async Task Get_UserSetting_ByUserId_Unauthorized(string userId)
        {
            // Act
            HttpRequestMessage getRequest = new HttpRequestMessage(HttpMethod.Get, $"v1/usersettings/user/{userId}");
            getRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken.Token);

            HttpResponseMessage response = await client.SendAsync(getRequest);

            // Fail the test if non-success result
            Assert.Equal(StatusCodes.Status403Forbidden, (int)response.StatusCode);
        }

        private async Task Get_UserSetting_Unauthorized()
        {
            // Act
            HttpRequestMessage getRequest = new HttpRequestMessage(HttpMethod.Get, $"v1/usersettings");

            HttpResponseMessage response = await client.SendAsync(getRequest);

            // Fail the test if non-success result
            Assert.Equal(StatusCodes.Status401Unauthorized, (int)response.StatusCode);
        }

        private async Task Get_UserSetting_NotFound(int id)
        {
            // Act
            HttpRequestMessage getRequest = new HttpRequestMessage(HttpMethod.Get, $"v1/usersettings/{id}");
            getRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken.Token);

            HttpResponseMessage response = await client.SendAsync(getRequest);

            // Fail the test if non-success result
            Assert.Equal(StatusCodes.Status404NotFound, (int)response.StatusCode);
        }

       private async Task Get_UserSetting(int expectedCount)
        {
            // Act
            HttpRequestMessage getRequest = new HttpRequestMessage(HttpMethod.Get, $"v1/usersettings");
            getRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken.Token);

            HttpResponseMessage response = await client.SendAsync(getRequest);

            // Fail the test if non-success result
            response.EnsureSuccessStatusCode();

            // Get the response as a string
            var result = JsonConvert.DeserializeObject<IEnumerable<UserSettingModel>>(await response.Content.ReadAsStringAsync());

            // Assert on correct content
            Assert.IsAssignableFrom<IEnumerable<UserSettingModel>>(result);
            Assert.Equal(expectedCount, result.Count());
        }

        private async Task Get_UserSetting_ByUserId_Successful(string userId, int expectedCount)
        {
            // Act
            HttpRequestMessage getRequest = new HttpRequestMessage(HttpMethod.Get, $"v1/usersettings/user/{userId}");
            getRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken.Token);

            HttpResponseMessage response = await client.SendAsync(getRequest);

            // Fail the test if non-success result
            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<IEnumerable<UserSettingModel>>(await response.Content.ReadAsStringAsync());

            // Assert on correct content
            Assert.IsAssignableFrom<IEnumerable<UserSettingModel>>(result);
            Assert.Equal(expectedCount, result.Count());
        }

        private async Task Delete_UserSetting_Successful(int id)
        {
            // Arrange
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Delete, $"v1/usersettings/{id}");
            postRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken.Token);

            // Act
            HttpResponseMessage response = await client.SendAsync(postRequest);

            // Fail the test if non-success result
            response.EnsureSuccessStatusCode();
        }

        private async Task Delete_UserSetting_NotAuthorized(int id)
        {
            // Arrange
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Delete, $"v1/usersettings/{id}");
            postRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken.Token);

            // Act
            HttpResponseMessage response = await client.SendAsync(postRequest);

            // Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, (int)response.StatusCode);
        }

        private async Task Delete_UserSetting_NotFound(int id)
        {
            // Arrange
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Delete, $"v1/usersettings/{id}");
            postRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken.Token);

            // Act
            HttpResponseMessage response = await client.SendAsync(postRequest);

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, (int)response.StatusCode);
        }
    }
}