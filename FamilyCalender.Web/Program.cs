using Microsoft.EntityFrameworkCore;
using FamilyCalender.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Infrastructure.Repositories;
using FamilyCalender.Infrastructure.Services;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Core.Interfaces;


namespace FamilyCalender
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromDays(365);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultScheme = "Cookie"; // Definiera standardautentisering
			})
				.AddCookie("Cookie", options =>
				{
					options.LoginPath = "/Login"; // Ange vart användare ska skickas vid behov
					options.LogoutPath = "/Login"; // Anger var användare ska skickas vid utloggning
												   //options.AccessDeniedPath = "/Account/AccessDenied"; // Anger path vid nekad åtkomst
					options.SlidingExpiration = true; // Gör så att sessionen hålls aktiv längre om användaren är aktiv

				});

			builder.Services.AddRazorPages()
				.AddRazorPagesOptions(options =>
				{
					options.Conventions.AuthorizeFolder("/");
					options.Conventions.AllowAnonymousToPage("/Login");
					options.Conventions.AllowAnonymousToPage("/Register");
				});

			// Add DbContext with SQLite
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));




			builder.Services.AddScoped<ICalendarRepository, CalendarRepository>();
			builder.Services.AddScoped<IMemberRepository, MemberRepository>();
			builder.Services.AddScoped<IEventRepository, EventRepository>();
			builder.Services.AddScoped<ICalendarAccessRepository, CalendarAccessRepository>();
			builder.Services.AddScoped<ICalendarService, CalendarService>();
			builder.Services.AddScoped<IMemberService, MemberService>();
			builder.Services.AddScoped<IEventService, EventService>();
			builder.Services.AddScoped<ICalendarAccessService, CalendarAccessService>();
			builder.Services.AddScoped<IMemberCalendarService, MemberCalendarService>();
			builder.Services.AddScoped<IAuthService, AuthService>();
			builder.Services.AddScoped<EventManagementService>();
			builder.Services.AddScoped<CalendarManagementService>();



			var app = builder.Build();

			app.UseSession();

			//app.Use(async (context, next) =>
			//{
			//	if (context.Request.Path.StartsWithSegments("/Login"))
			//	{
			//		context.Response.Redirect("/Login");
			//		return;
			//	}

			//	await next();
			//});




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
