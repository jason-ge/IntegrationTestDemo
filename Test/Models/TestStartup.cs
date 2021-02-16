using IntegrationTestDemo.API;
using IntegrationTestDemo.Controllers;
using IntegrationTestDemo.DAL;
using IntegrationTestDemo.DAL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTestDemo.IntegrationTests.Models
{
    internal class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddApplicationPart(typeof(UserSettingsController).Assembly);
            base.ConfigureServices(services);
        }

        protected override void AddDatabaseContext(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var connection = new SqliteConnection(connectionString);
            connection.Open();

            services.AddDbContext<UserSettingContext>(options =>
                options.UseSqlite(connection));
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserSettingContext context)
        {
            base.Configure(app, env, context);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
