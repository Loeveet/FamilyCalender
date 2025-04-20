using Microsoft.EntityFrameworkCore;
using FamilyCalender.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Infrastructure.Repositories;
using FamilyCalender.Infrastructure.Services;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Core.Interfaces;
using Serilog;
using System.Security.Claims;
using NuGet.Packaging;
using PublicHoliday;
using FamilyCalender.Web.Code;


namespace FamilyCalender
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

            
         

            builder.Services.AddHttpContextAccessor();

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultScheme = "Cookie";
			})
			.AddCookie("Cookie", options =>
			{
				options.LoginPath = "/Login"; 
				options.LogoutPath = "/Login";
				options.ExpireTimeSpan = TimeSpan.FromDays(365);
				options.SlidingExpiration = true;

			});

			builder.Services.AddRazorPages()
				.AddRazorPagesOptions(options =>
				{
					options.Conventions.AllowAnonymousToPage("/");
					

					options.Conventions.AuthorizeFolder("/CalendarOverview");
					options.Conventions.AuthorizeFolder("/CreateCalendar");
					options.Conventions.AllowAnonymousToPage("/Login");
					options.Conventions.AllowAnonymousToPage("/Register");
                    options.Conventions.AllowAnonymousToPage("/VerifyAccount");
                    options.Conventions.AllowAnonymousToPage("/AccountVerified");
                    options.Conventions.AllowAnonymousToPage("/ForgotPassword");
                    options.Conventions.AllowAnonymousToPage("/ResetPassword");
                    options.Conventions.AddPageRoute("/VerifyAccount", "/VerifyAccount/{token}");
                    options.Conventions.AddPageRoute("/ResetPassword", "/ResetPassword/{token}");
                    options.Conventions.AddPageRoute("/Invite", "/Invite/{inviteId}");

                });


            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("LogFiles\\log.txt",
                    rollingInterval: RollingInterval.Month)
                .CreateLogger();
            
            Log.Information("Application started");
           
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
			builder.Services.AddScoped<IEmailService, EmailService>();
			builder.Services.AddScoped<EventManagementService>();
			builder.Services.AddScoped<CalendarManagementService>();
			builder.Services.AddScoped<InviteService>();
            builder.Services.AddSingleton(new EncryptionService(EncryptionService.Magic));
            builder.Services.AddSingleton(new PublicHolidayService(new SwedenPublicHoliday(), "SWEDEN"));

            var app = builder.Build();

			//https://stackoverflow.com/questions/47598844/enabling-migrations-in-ef-core
			using var serviceScope = app.Services.CreateScope();
			using var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			context.Database.Migrate();
            Log.Information("Database update with latest migration");

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
