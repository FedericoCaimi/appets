using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using Appets.Domain;
using Appets.DataAccess;
using Appets.DataAccess.Interface;
using Appets.BusinessLogic;
using Appets.BusinessLogic.Interface;
using Appets.WebApi.Filters;
using Appets.DataAccess.Repositories;

namespace Appets.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var server = Configuration["SQLServer"] ?? "127.0.0.1";
            var port = Configuration["DBPort"] ?? "1433";
            var user = Configuration["DBUser"] ?? "sqlserver";
            var password = Configuration["DBPassword"] ?? "Fede147895";
            var database = Configuration["Database"] ?? "Appets";

            services.AddControllers();

            services.AddDbContext<DbContext, AppetsContext>(options =>

    //options.UseSqlServer(@"Server=" + Configuration["SQLServer"] + ";Database=Appets;Trusted_Connection=True;MultipleActiveResultSets=True;"));
    options.UseSqlServer($"Server={server},{port};Database={database};User ID ={user};Password={password};"));


            // DataAccess interfaces
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRepository<Session>, SessionRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IRepository<Pet>, PetRepository>();

            // BusinessLogic interfaces
            services.AddScoped<IAuthenticationLogic, AuthenticationLogic>();
            services.AddScoped<IUserLogic, UserLogic>();
            services.AddScoped<IPostLogic, PostLogic>();
            services.AddScoped<IPetLogic, PetLogic>();

            // Auth service
            services.AddScoped<AuthenticationFilter>();

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddCors(
                options =>
                {
                    options.AddPolicy(
            "CorsPolicy",
            builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
                );
                }
            );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
