using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Dessert.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        [HttpPost("~/register")]
        public IActionResult Register([FromForm] string provider)
        {
            // Note: the "provider" parameter corresponds to the external
            // authentication provider choosen by the user agent.
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest();
            }

            // Instruct the middleware corresponding to the requested external identity
            // provider to redirect the user agent to its own authorization endpoint.
            // Note: the authenticationScheme parameter must match the value configured in Startup.cs
            return Challenge(new AuthenticationProperties { RedirectUri = _configuration["Github:RedirectUrl"] }, provider);
        }
        
        [HttpPost("~/signin")]
        public IActionResult SignIn([FromForm] string provider)
        {
            // Note: the "provider" parameter corresponds to the external
            // authentication provider choosen by the user agent.
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest();
            }

            // Instruct the middleware corresponding to the requested external identity
            // provider to redirect the user agent to its own authorization endpoint.
            // Note: the authenticationScheme parameter must match the value configured in Startup.cs
            //TODO: redirect in config
            return Challenge(new AuthenticationProperties { RedirectUri = "https://dessert.dev/" }, provider);
        }

        [HttpGet("~/signin")]
        public IActionResult SignIn()
        {
            return new ContentResult {
                ContentType = "text/html",
                StatusCode = (int) HttpStatusCode.OK,
                Content = @"
                          <div>
                                          <h1>Authentication</h1>
                                          <p >Sign in using one of these external providers:</p>
                          
                                          <form action=""/signin"" method=""post"">
                                          <input type=""hidden"" name=""Provider"" value=""GitHub"" />
                          
                                          <button type=""submit"">Connect using GitHub</button>
                                          </form>
                                          </div>
                                          "
            };
        }
    }
}