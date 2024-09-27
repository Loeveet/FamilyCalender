using Microsoft.EntityFrameworkCore;
using FamilyCalender.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using FamilyCalender.Core.Models;


namespace FamilyCalender
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

            builder.Services.AddRazorPages()
				.AddRazorPagesOptions(options =>
				{
					options.Conventions.AuthorizePage("/Account/Login");
				});


            // Add DbContext with SQLite
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

			app.MapRazorPages();

			app.Run();
		}
	}
}
