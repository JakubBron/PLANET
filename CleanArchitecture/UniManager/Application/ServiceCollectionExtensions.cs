using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Application.Services;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application // or Application namespace
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddDbContext<UniversityDbContext>((sp, options) =>
            {
                var factory = new UniversityDbContextFactory();
                var context = factory.CreateDbContext(Array.Empty<string>());
                options.UseSqlServer(context.Database.GetConnectionString());
            });

            // Register all your application services
            services.AddScoped<ILicznikIndeksowService, LicznikIndeksowService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IStudentStudiowMgrService, StudentStudiowMgrService>();
            services.AddScoped<IProfesorService, ProfesorService>();
            services.AddScoped<IKursService, KursService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IWydzialService, WydzialService>();
            services.AddScoped<IGabinetService, GabinetService>();

            // Register your generator
            services.AddScoped<BogusGenerator>();

            return services;
        }
    }
}