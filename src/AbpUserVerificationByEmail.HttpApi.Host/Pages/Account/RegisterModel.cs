using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AbpUserVerificationByEmail.Domain.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Volo.Abp.Account;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;

namespace AbpUserVerificationByEmail.HttpApi.Host.Pages.Account
{
  public class CustomRegisterModel : RegisterModel
  {
    private readonly EmailService _emailService;
    private readonly IAccountAppService _accountAppService;
    private readonly IEmailSender _emailSender;

    public CustomRegisterModel(IAccountAppService accountAppService, EmailService emailService, IEmailSender emailSender) : base(accountAppService)
    {
      _emailSender = emailSender;
      _accountAppService = accountAppService;
      _emailService = emailService;
    }

    // protected override async Task RegisterLocalUserAsync()
    // {
    //   await _emailService.SendEmailAsync();
    //   await base.RegisterLocalUserAsync();
    // }

    public override async Task<IActionResult> OnPostAsync()
    {
      var returnUrl = Url.Content("~/");
      if (ModelState.IsValid)
      {
        var user = new IdentityUser(GuidGenerator.Create(), Input.UserName, Input.EmailAddress, CurrentTenant.Id);

        var result = await UserManager.CreateAsync(user, Input.Password);
        if (result.Succeeded)
        {
          var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
          code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
          var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { userId = user.Id, code = code }, protocol: Request.Scheme);

          // TODO use EmailService instead of using IEmailSender directly
          await _emailSender.SendAsync(Input.EmailAddress, "Confirm your email",
              $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

          if (UserManager.Options.SignIn.RequireConfirmedAccount)
          {
            return RedirectToPage("RegisterConfirmation", new { email = Input.EmailAddress, returnUrl = returnUrl });
          }
          else
          {
            await SignInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
          }
        }
      }

      // If we got this far, something failed, redisplay form
      return Page();
    }
  }
}