using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp_UnderTheHood.Auth;

namespace WebApp_UnderTheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; }

        public void OnGet()
        {
            ViewData.SetIsLoginPage(true);
        }

        public async Task<IActionResult> OnPostAsync([FromQuery] string? returnUrl)
        {
            // verify the credentials
            if (ModelState.IsValid && Credential.UserName == "admin" && Credential.Password == "password")
            {
                var claimsPrincipal = CreateAuthenticationUserContext();

                // Generating Security Context, in this case is cookie
                // WHY: The scheme name will be used to find the correct authentication handler linked to it
                // .AddCookie(DefaultAuthenticationSchemes.CookieScheme) will add a cookie handler and it will handle this signin
                // But you can use other type of authentication like jwt with the same name and it may still works
                await HttpContext.SignInAsync(scheme: AppAuthenticationSchemes.CookieScheme, claimsPrincipal, new AuthenticationProperties()
                {
                    IsPersistent = Credential.RememberMe // When true, the cookie will be persisted. Default cookie expires on session (close browser)
                });

                return Redirect(returnUrl ?? "~/");
            }

            return Page();
        }

        private ClaimsPrincipal CreateAuthenticationUserContext()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, Credential.UserName),
                new Claim(ClaimTypes.Email, $"{Credential.UserName}@mywebsite.com"),
                new Claim(AppAuthenticationClaims.HrDepartment.Type, AppAuthenticationClaims.HrDepartment.Value),
                new Claim(AppAuthenticationClaims.Admin.Type, AppAuthenticationClaims.Admin.Value),
                new Claim(AppAuthenticationClaims.Manager.Type, AppAuthenticationClaims.Manager.Value),
                new Claim(AppAuthenticationClaims.EmploymentDate.Type, new DateTime(2021,1,1).ToString())
            };

            var identity = new ClaimsIdentity(claims, AppAuthenticationSchemes.CookieScheme);

            var claimsPrincipal = new ClaimsPrincipal(identity);

            return claimsPrincipal;
        }
    }

    public class Credential
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me (Still keep cookie session after user close the browser. The cookie IsPersistent = true)")]
        public bool RememberMe { get; set; }
    }
}