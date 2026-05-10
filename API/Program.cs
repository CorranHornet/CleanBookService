using BookService.Data;
using BookService.Models;
using BookService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
//test
namespace BookService
{
    public partial class Program { }

    public static class ProgramSetup
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:5500",
                            "http://127.0.0.1:5500",
                            "http://localhost:5173"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure database context
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // Register services
            builder.Services.AddScoped<IMediaItemService, MediaItemService>();
            builder.Services.AddScoped<IMediaUnitService, MediaUnitService>();
            builder.Services.AddScoped<ILoanService, LoanService>();
            builder.Services.AddScoped<IUserService, UserService>();

            var app = builder.Build();

            // TEMP DEV SEED DATA - REMOVE WHEN AUTH/ JWT IS IMPLEMENTED
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if(!db.Users.Any())
                {
                    db.Users.Add(new User
                    {
                        Username = "testuser",
                        Email = "test@test.com",
                        PasswordHash = "test",
                        Role = "User"
                    });
                    db.SaveChanges();
                }
            }




            // Configure middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowFrontend");
            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}
