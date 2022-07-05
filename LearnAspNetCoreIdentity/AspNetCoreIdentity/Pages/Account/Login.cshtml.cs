using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AspNetCoreIdentity.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCoreIdentity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> signInManager;

        public LoginModel(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }

        [BindProperty]
        public CredentialViewModel Credential { get; set; } = new CredentialViewModel();

        [BindProperty] public IEnumerable<AuthenticationScheme> ExternalLoginProviders { get; set; }

        public async Task<IActionResult> OnGet()
        {
            ExternalLoginProviders = await signInManager.GetExternalAuthenticationSchemesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromQuery] string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await signInManager.PasswordSignInAsync(
                    userName: Credential.Email,
                    password: Credential.Password,
                    isPersistent: Credential.RememberMe,
                    lockoutOnFailure: false);

                if (signInResult.Succeeded)
                    return Redirect(returnUrl ?? "~/");
                else if (signInResult.RequiresTwoFactor)
                {
                    if (Credential.LoginTwoFactorWithAuthenticator)
                    {
                        return RedirectToPage(
                            "/Account/LoginTwoFactorWithAuthenticator",
                            new
                            {
                                RememberMe = Credential.RememberMe
                            });
                    }
                    else
                    {
                        return RedirectToPage(
                            "/Account/LoginTwoFactor",
                            new
                            {
                                Email = Credential.Email,
                                RememberMe = Credential.RememberMe
                            });
                    }
                }
                else if (signInResult.IsLockedOut)
                    ModelState.AddModelError("Login", "You are locked out.");
                else
                    ModelState.AddModelError("Login", "Failed to login");
            }

            return Page();
        }

        // string provider <=> name="provider"
        // asp-page-handler="LoginExternally" <=> LoginExternally
        public IActionResult OnPostLoginExternally(string provider)
        {
            var externalAuthenticationProperties = signInManager.ConfigureExternalAuthenticationProperties(
                provider,
                redirectUrl: Url.Action(controller: "Account", action: "ExternalLoginCallback"));

            return Challenge(properties: externalAuthenticationProperties);
        }
    }

    public class CredentialViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        [Display(Name = "LoginTwoFactorWithAuthenticator")]
        public bool LoginTwoFactorWithAuthenticator { get; set; }
    }
}
