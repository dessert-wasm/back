using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Dessert.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        [HttpPost("~/signin")]
        public async Task<IActionResult> SignIn([FromForm] string provider)
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
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, provider);
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