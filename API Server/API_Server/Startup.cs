using API_Server.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace API_Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Настройка сервисов, таких как DbContext и контроллеры
        public void ConfigureServices(IServiceCollection services)
        {
            // Настройка подключения к PostgreSQL
            services.AddDbContext<GameDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();

            // Добавление Swagger для генерации API-документации
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Server",
                    Version = "v1"
                });
            });
        }

        // Настройка пайплайна обработки запросов
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Включение страницы ошибок в режиме разработки
            }

            // Включение Swagger
            app.UseSwagger();

            // Настройка интерфейса Swagger UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Server V1");
                c.RoutePrefix = string.Empty; // Открывать Swagger по корневому адресу
            });

            app.UseRouting(); // Включение маршрутизации запросов

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Маршрутизация к контроллерам
            });
        }
    }
}
