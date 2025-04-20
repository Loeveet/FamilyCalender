using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

namespace FamilyCalender.Web.Pages
{
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	[IgnoreAntiforgeryToken]
	public class ErrorModel(ILogger<ErrorModel> logger) : PageModel
	{
		public string? RequestId { get; set; }

		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

		private readonly ILogger<ErrorModel> _logger = logger;

		public void OnGet()
		{
			RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            try
            {
                var ex = HttpContext.Features.Get<Exception>();

                Log.Error($"Error when {ex.Message} {ex.StackTrace}");
            }
            catch (Exception e)
            {
               
            }
        }
	}

}
