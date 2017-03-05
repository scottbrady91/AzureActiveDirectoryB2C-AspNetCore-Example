using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ScottBrady91.AzureActiveDirectoryB2C.AspNetCore.Example.Controllers
{
    public class AccountController : Controller
    {
        public async Task ProfileUpdate()
        {
            await HttpContext.Authentication.ChallengeAsync(
                Startup.ProfilePolicy,
                new AuthenticationProperties { RedirectUri = "/" });
        }

        public async Task PasswordReset()
        {
            await HttpContext.Authentication.ChallengeAsync(
                Startup.PasswordResetPolicy,
                new AuthenticationProperties { RedirectUri = "/" });
        }

        public async Task SignOut()
        {
            await HttpContext.Authentication.SignOutAsync(Startup.LocalAuthenticationScheme); // Local sign out

            var scheme = User.Claims.FirstOrDefault(x => x.Type == "tfp")?.Value; // Find most recently used oidc policy
            if (scheme != null)
                await HttpContext.Authentication.SignOutAsync(scheme);
        }

        public IActionResult ManualPassword()
        {
            var request =
                @"https://login.microsoftonline.com/scottbrady91adtest.onmicrosoft.com/oauth2/v2.0/authorize" 
                    + "?p=" + Startup.PasswordResetPolicy 
                    + "&client_Id=" + Startup.ClientId 
                    + "&nonce=defaultNonce" 
                    + "&redirect_uri=https%3A%2F%2Fgoogle.com"
                    + "&scope=openid"
                    + "&response_type=id_token"
                    + "&prompt=login";
            return Redirect(request);
        }
    }
}