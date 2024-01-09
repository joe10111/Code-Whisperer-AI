using CodeWhispererAI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CodeWhispererAI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var currentContext = HttpContext;
            SetUserCookie(currentContext);

            SetUserIPCookie(currentContext, GetUserIPAddress(currentContext));

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        } 

        // Getting Ip so I can compare with user cookie to make sure even if they clear cookie they cant keep sumbitting requests.
        public string GetUserIPAddress(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress.ToString();
            return ipAddress;
        }

          // Making a anom user idenity since I am getting rid of log in
        public void SetUserCookie(HttpContext httpContext)
        {
            // Check if the user cookie already exists
            if (httpContext.Request.Cookies["UserIdentifier"] == null)
            {
                // Generate a new unique identifier
                string userId = Guid.NewGuid().ToString();

                // Create a new cookie option
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(30); // Set expiration for 30 days
                option.HttpOnly = true; // To prevent XSS attacks
                option.Secure = true; // Send the cookie over HTTPS only

                // Set the cookie in the response
                httpContext.Response.Cookies.Append("UserIdentifier", userId, option);
            }
        }

        public void SetUserIPCookie(HttpContext httpContext, string ipAddress)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30), // Set an expiration date
                HttpOnly = true, // Enhance security by making the cookie accessible only on the server
                Secure = true   // Ensure the cookie is sent over HTTPS
            };

            httpContext.Response.Cookies.Append("UserIP", ipAddress, options);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}