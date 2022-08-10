using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Volo.Abp.Identity;

namespace AbpUserVerificationByEmail.HttpApi.Host.Pages.Account
{
    [AllowAnonymous]
    public class CustomConfirmEmailModel : PageModel
    {
        private readonly IdentityUserManager _userManager;

        public CustomConfirmEmailModel(IdentityUserManager userManager) => _userManager = userManager;

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId.IsNullOrWhiteSpace()|| code.IsNullOrWhiteSpace()) return RedirectToPage("/Index");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)return NotFound($"Unable to load user with ID '{userId}'.");

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return Page();
        }
    }
}