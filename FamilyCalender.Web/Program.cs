using Microsoft.EntityFrameworkCore;
using FamilyCalender.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using FamilyCalender.Core.Models;
using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Infrastructure.Repositories;
using FamilyCalender.Infrastructure.Services;


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
                    options.Conventions.AuthorizeFolder("/"); 
                    options.Conventions.AllowAnonymousToPage("/Identity/Account/Login");
                    options.Conventions.AllowAnonymousToPage("/Identity/Account/Register");
                });


            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                //options.LoginPath = "/Identity/Account/Login"; //Får inte att fungera. Använder middleware för omdirigering tills vidare
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            // Add DbContext with SQLite
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false; //Behåller som false tills man behöver e-mailbekräftelser
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.AddScoped<ICalendarRepository, CalendarRepository>();
            builder.Services.AddScoped<IMemberRepository, MemberRepository>();
			builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ICalendarAccessRepository, CalendarAccessRepository>();
			builder.Services.AddScoped<IMemberEventRepository, MemberEventRepository>();
			builder.Services.AddScoped<ICalendarService, CalendarService>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<ICalendarAccessService, CalendarAccessService>();
			builder.Services.AddScoped<IMemberEventService, MemberEventService>();
			builder.Services.AddScoped<IMemberCalendarService, MemberCalendarService>();


			var app = builder.Build();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/Account/Login"))
                {
                    context.Response.Redirect("/Identity/Account/Login");
                    return;
                }

                await next();
            });


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
