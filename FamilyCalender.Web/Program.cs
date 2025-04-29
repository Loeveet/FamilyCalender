using Microsoft.EntityFrameworkCore;
using FamilyCalender.Infrastructure.Context;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Infrastructure.Services;
using FamilyCalender.Core.Interfaces;
using Serilog;
using PublicHoliday;
using FamilyCalender.Web.Code;
using static FamilyCalender.Infrastructure.Services.EmailService;
using Microsoft.AspNetCore.DataProtection;


namespace FamilyCalender
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
         

            builder.Services.AddHttpContextAccessor();


            var keyPath = Path.Combine(AppContext.BaseDirectory, "DataProtectionKeys");

            if (!Directory.Exists(keyPath))
            {
                Directory.CreateDirectory(keyPath);
            }

            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
                .SetApplicationName("FamilyCalendarApp"); //Blir tokigt om det ändras

            builder.Services.AddAuthentication()
			.AddCookie(GlobalSettings.AuthCookieName, 
				options =>
				{
					options.LoginPath = "/Login";
					options.AccessDeniedPath = "/"; //should be a Forbidden page
					options.LogoutPath = "/Logout";
					options.Cookie.Name = GlobalSettings.AuthCookieName;
					options.ExpireTimeSpan = TimeSpan.FromDays(365);
					options.Cookie.MaxAge = TimeSpan.FromDays(365);
					options.SlidingExpiration = false;
					options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                });

			builder.Services.AddRazorPages()
				.AddRazorPagesOptions(options =>
				{
					options.Conventions.AllowAnonymousToPage("/");
					

					options.Conventions.AuthorizeFolder("/CalendarOverview");
					options.Conventions.AuthorizeFolder("/CreateCalendar");
					options.Conventions.AuthorizePage("/SuperAdmin");
					//options.Conventions.AuthorizeFolder("/Invite");
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

			Log.Information($"Using database connectionstring {builder.Configuration.GetConnectionString("DefaultConnection")}");

			var emailSettings = builder.Configuration.GetSection("Email").Get<EmailSettings>() ?? new EmailSettings();

			//builder.Services.AddScoped<ICalendarRepository, CalendarRepository>();
			//builder.Services.AddScoped<IMemberRepository, MemberRepository>();
			//builder.Services.AddScoped<IEventRepository, EventRepository>();
            //builder.Services.AddScoped<IUserRepository, UserRepository>();
            //builder.Services.AddScoped<ICalendarAccessRepository, CalendarAccessRepository>();
			builder.Services.AddScoped<ICalendarService, CalendarService>();
			builder.Services.AddScoped<IMemberService, MemberService>();
			builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICalendarAccessService, CalendarAccessService>();
			builder.Services.AddScoped<IMemberCalendarService, MemberCalendarService>();
			builder.Services.AddScoped<IEmailService>(c => new EmailService(emailSettings));
			builder.Services.AddScoped<EventManagementService>();
			builder.Services.AddScoped<CalendarManagementService>();
			builder.Services.AddScoped<InviteService>();
			builder.Services.AddScoped<IAuthService, AuthService>();
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

			app.MapControllerRoute(
				"default",
				pattern: "{controller=Home}/{action=index}/{id?}");


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
