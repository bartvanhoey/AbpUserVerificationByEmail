using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.Emailing;

namespace AbpUserVerificationByEmail.HttpApi.Host.Pages.Account
{
  public class CustomRegisterModel : RegisterModel
  {
    private readonly IAccountAppService _accountAppService;
    private readonly IEmailSender _emailSender;

    public CustomRegisterModel(IAccountAppService accountAppService, IEmailSender emailSender) : base(accountAppService)
    {
      _emailSender = emailSender;
      _accountAppService = accountAppService;
    }

    public override async Task<IActionResult> OnPostAsync()
    {
      try
      {
        await CheckSelfRegistrationAsync();

        if (IsExternalLogin)
        {
          var externalLoginInfo = await SignInManager.GetExternalLoginInfoAsync();
          if (externalLoginInfo == null)
          {
            Logger.LogWarning("External login info is not available");
            return RedirectToPage("./Login");
          }
          await RegisterExternalUserAsync(externalLoginInfo, Input.EmailAddress);
        }
        else
        {
          await RegisterLocalUserAsync();
        }

        return Redirect(ReturnUrl ?? "~/"); //TODO: How to ensure safety? IdentityServer requires it however it should be checked somehow!
      }
      catch (BusinessException e)
      {
        Alerts.Danger(e.Message);
        return Page();
      }
    }

    protected override async Task RegisterLocalUserAsync()
    {
      ValidateModel();

      var userDto = await AccountAppService.RegisterAsync(
          new RegisterDto
          {
            AppName = "YourAppName Here",
            EmailAddress = Input.EmailAddress,
            Password = Input.Password,
            UserName = Input.UserName
          }
      );

      var user = await UserManager.GetByIdAsync(userDto.Id);

      // Send user an email to confirm email address      
      await SendEmailToAskForEmailConfirmationAsync(user);
    }

    protected override async Task RegisterExternalUserAsync(ExternalLoginInfo externalLoginInfo, string emailAddress)
    {
      await IdentityOptions.SetAsync();

      var user = new Volo.Abp.Identity.IdentityUser(GuidGenerator.Create(), emailAddress, emailAddress, CurrentTenant.Id);

      (await UserManager.CreateAsync(user)).CheckErrors();
      (await UserManager.AddDefaultRolesAsync(user)).CheckErrors();

      var userLoginAlreadyExists = user.Logins.Any(x =>
          x.TenantId == user.TenantId &&
          x.LoginProvider == externalLoginInfo.LoginProvider &&
          x.ProviderKey == externalLoginInfo.ProviderKey);

      if (!userLoginAlreadyExists)
      {
        (await UserManager.AddLoginAsync(user, new UserLoginInfo(
            externalLoginInfo.LoginProvider,
            externalLoginInfo.ProviderKey,
            externalLoginInfo.ProviderDisplayName
        ))).CheckErrors();
      }

      await SendEmailToAskForEmailConfirmationAsync(user);
    }

  
    private async Task SendEmailToAskForEmailConfirmationAsync(Volo.Abp.Identity.IdentityUser user)
    {
      var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
      code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
      var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { userId = user.Id, code = code }, protocol: Request.Scheme);

      // TODO use EmailService instead of using IEmailSender directly
      await _emailSender.SendAsync(Input.EmailAddress, "Confirm your email",
          $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

      if (UserManager.Options.SignIn.RequireConfirmedAccount)
      {
        RedirectToPage("RegisterConfirmation", new { email = Input.EmailAddress, returnUrl = ReturnUrl });
      }
      else
      {
        await SignInManager.SignInAsync(user, isPersistent: true);
        LocalRedirect(ReturnUrl);
      }
    }

  }
}