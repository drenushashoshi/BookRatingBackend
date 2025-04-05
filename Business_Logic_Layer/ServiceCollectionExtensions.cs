using Data_Access_Layer.UnitOfWork;
using Data_Access_Layer;
using FBookRating.Services.IServices;
using FBookRating.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic_Layer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the DbContext from the Data Access Layer
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register Unit of Work from the Data Access Layer
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register business services
            services.AddScoped<IBookService, BookService>();
            // Register other business services as needed

            return services;
        }
    }
}
