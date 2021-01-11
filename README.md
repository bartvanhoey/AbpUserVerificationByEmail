## Setup email verification after user registration in an ABP application

![.NET](https://github.com/bartvanhoey/AbpUserVerificationByEmail/workflows/.NET/badge.svg)

## Introduction

In this article, I will show you how to get a user verified by email after he fills in the registration form in a **Blazor APB application**.

The sample application has been developed with **Blazor** as UI framework and **SQL Server** as database provider.

In this article I make use of the free **Google SMTP Server** for sending emails,  in a real-world application however,  you probably would chose another **Email Delivery Service** like **SendGrid, Mailjet, etc.**

### Source Code

Source code of the completed application is [available on GitHub](https://github.com/bartvanhoey/AbpUserVerificationByEmail).

## Requirements

The following tools are needed to be able to run the solution.

* .NET 5.0 SDK
* VsCode, Visual Studio 2019 16.8.0+ or another compatible IDE

You also need a **Gmail** account to follow along.

## Development

### Creating a new Application

* Install or update the ABP CLI:

```bash
dotnet tool install -g Volo.Abp.Cli || dotnet tool update -g Volo.Abp.Cli
```

* Use the following ABP CLI command to create a new Blazor ABP application:

```bash
abp new AbpUserVerificationByEmail -u blazor
```

### Open & Run the Application

* Open the solution in Visual Studio (or your favorite IDE).
* Run the `AbpUserVerificationByEmail.DbMigrator` application to apply the migrations and seed the initial data.
* Run the `AbpUserVerificationByEmail.HttpApi.Host` application to start the server side.
* Run the `AbpUserVerificationByEmail.Blazor` application to start the Blazor UI project.

## Basic implementation of the RegisterModel

* Create a folder structure **Pages/Account** in the **HttpApi.Host** project of your application.
* Add a **RegisterModel.cs** file to the **Account** folder and paste in code below.

```csharp
using System.Threading.Tasks;
using AbpUserVerificationByEmail.Domain.Email;
using Volo.Abp.Account;
using Volo.Abp.Account.Web.Pages.Account;

namespace AbpUserVerificationByEmail.HttpApi.Host.Pages.Account
{
  public class CustomRegisterModel : RegisterModel
  {
    private readonly EmailService _emailService;

    public CustomRegisterModel(IAccountAppService accountAppService, EmailService emailService) : base(accountAppService)
    {
      _emailService = emailService;
    }

    protected override async Task RegisterLocalUserAsync()
    {
      await _emailService.SendEmailAsync();
      await base.RegisterLocalUserAsync();
    }
  }
}
```

* Add a **Register.cshtml** file to the **Account** folder.

```html
@page
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, Volo.Abp.AspNetCore.Mvc.UI
@addTagHelper *, Volo.Abp.AspNetCore.Mvc.UI.Bootstrap
@addTagHelper *, Volo.Abp.AspNetCore.Mvc.UI.Bundling

@using Microsoft.AspNetCore.Mvc.Localization
@using Volo.Abp.Account.Localization

@model AbpUserVerificationByEmail.HttpApi.Host.Pages.Account.CustomRegisterModel
@inject IHtmlLocalizer<AccountResource> L

<div class="card mt-3 shadow-sm rounded">
    <div class="card-body p-5">
        @* <h4>@L["Register"]</h4> *@
        <h4>My Custom Register Page</h4>
        <strong>
            @L["AlreadyRegistered"]
            <a href="@Url.Page("./Login", new {returnUrl = Model.ReturnUrl, returnUrlHash = Model.ReturnUrlHash})" class="text-decoration-none">@L["Login"]</a>
        </strong>
        <form method="post" class="mt-4">
            @if (!Model.IsExternalLogin)
            {
                <abp-input asp-for="Input.UserName" auto-focus="true"/>
            }

            <abp-input asp-for="Input.EmailAddress"/>

            @if (!Model.IsExternalLogin)
            {
                <abp-input asp-for="Input.Password"/>
            }
            <abp-button button-type="Primary" type="submit" class="btn-lg btn-block mt-4">@L["Register"]</abp-button>
        </form>
    </div>
</div>
```

## Implement Email Functionality

### Comment out statement that injects class NullEmailSender in the **Domain** project

* In file **AbpUserVerificationByEmailDomainModule.cs**  comment out the statement below.

```csharp
// #if DEBUG
//    context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
// #endif
```

If we don't comment out the statement above, the application will not send emails as class NullEmailSender will be injected by the Dependency Injection.

### Create a basic EmailService

* Create a folder  **Email** in the **Domain** project of your application.
* Add an **EmailService.cs** class to the **Email** folder. Copy/paste code below.

```csharp
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.Security.Encryption;

namespace AbpUserVerificationByEmail.Domain.Email
{
  public class EmailService : ITransientDependency
  {
    private readonly IEmailSender _emailSender;
    public IStringEncryptionService _encryptionService { get; set; }

    public EmailService(IEmailSender emailSender)
    {
      _emailSender = emailSender;
    }

    public async Task SendEmailAsync()
    {
      var encryptedGmailPassword = _encryptionService.Encrypt("your-gmail-password-here");
      await _emailSender.SendAsync("recipient-email-here", "Email subject", "This is the email body...");
    }
  }
}
```

### Get the encrypted Gmail password

* Set a breakpoint on line *await _emailSender.SendAsync("...");*
* Replace *your-gmail-password-here* with your Gmail password.
* Replace *recipient-email-here* with your own email address.
* Start both the **Blazor** and **HttpApi.Host** project to run the application.
* Navigate to the **Login** page and click on the  [Register](https://localhost:44367/) link.
* Fill in the form of the **My Custom Register Page** and click the **Register** button.
* Copy the value of the **encryptedGmailPassword** when the breakpoint gets hit.
* Stop both the Blazor and the HttpApi.Host project.
* Open file **appsettings.json** in project **HttpApi.Host** and update the **Smtp Settings** with the correct values.
  
```json
"Settings": {
    "Abp.Mailing.Smtp.Host": "smtp.gmail.com",
    "Abp.Mailing.Smtp.Port": "587",
    "Abp.Mailing.Smtp.UserName": "your-gmail-email-address-here",
    "Abp.Mailing.Smtp.Password": "your-encrypted-gmail-password-here",
    "Abp.Mailing.Smtp.Domain": "",
    "Abp.Mailing.Smtp.EnableSsl": "true",
    "Abp.Mailing.Smtp.UseDefaultCredentials": "false",
    "Abp.Mailing.DefaultFromAddress": "your-gmail-email-address-here",
    "Abp.Mailing.DefaultFromDisplayName": "Your-Custom-Text-Here"
  }
```

### Check if you can send and receive email with the EmailService

* Remove the **IStringEncryptionService** from  the **EmailService** class as no longer needed.

```csharp
public class EmailService : ITransientDependency
{
    private readonly IEmailSender _emailSender;

    public EmailService(IEmailSender emailSender)
    {
      _emailSender = emailSender;
    }

    public async Task SendEmailAsync()
    {
      // TODO replace recipient-email-here
      await _emailSender.SendAsync("recipient-email-here", "Email subject", "This is the email body...");
    }
 }
```

* If you already registered a user, delete it first in table **AbpUsers** in the database.
* Start both the **Blazor** and **HttpApi.Host** project to run the application.
* Navigate to the **Login** page again and click on the  [Register](https://localhost:44367/) link.
* Fill in the form of the **My Custom Register Page** and click the **Register** button.
* If all goes well, you should **receive an email** sent by the EmailService and the **user should have been registered**.

**ATTENTION**: It is possible you need to turn on setting **Access for less secure apps can be easily turned on here** in your **Google** account settings.

**WARNING**: Make sure you **don't publish** your **Google Credentials** to **GitHub** or **another Versioning System**.

## Change Index.razor of the Blazor project

* Open **Index.razor** and update **div class="container"** with the code below.

```html
<div class="container">
    <div class="p-5 text-center">
        @if (!CurrentUser.IsAuthenticated)
        {
            <Badge Color="Color.Danger" class="mb-4">
                <h5 class="m-1"> <i class="fas fa-email"></i> <strong>User is NOT Authenticated!</strong></h5>
            </Badge>
        }

        @if (CurrentUser.IsAuthenticated && !CurrentUser.EmailVerified)
        {
            <Badge Color="Color.Warning" class="mb-4">
                <h5 class="m-1"> <i class="fas fa-email"></i> <strong>User has not NOT been verified by email yet!</strong></h5>
            </Badge>
        }

        @if (CurrentUser.IsAuthenticated && CurrentUser.EmailVerified)
        {
            <Badge Color="Color.Success" class="mb-4">
                <h5 class="m-1"> <i class="fas fa-email"></i> <strong>User is successfully verified by email!</strong></h5>
            </Badge>
        }
    </div>
</div>
```

* Start both the **Blazor** and **HttpApi.Host** project to run the application.
* Go to the **Login** form and login with the credentials of the new user. The user is **logged in** but **not email verified**.

![User has not been email verified yet](images/usernotverifiedbyemail.jpg)

## Update SignIn IdentityOptions so a user has to confirm his email address

* Open file **AbpUserVerificationByEmailHttpApiModule.cs** in the **AbpUserVerificationByEmail.HttpApi.Host** project
* Add `ConfigureIdentityOptions(context);` as last statement in the **ConfigureServices** method.
* Add a private method **ConfigureIdentityOptions** just beneath the ****ConfigureServices**** method.
  
```csharp
public override void ConfigureServices(ServiceConfigurationContext context)
{
    var configuration = context.Services.GetConfiguration();
    var hostingEnvironment = context.Services.GetHostingEnvironment();

    ConfigureBundles();
    ConfigureUrls(configuration);
    ConfigureConventionalControllers();
    ConfigureAuthentication(context, configuration);
    ConfigureLocalization();
    ConfigureVirtualFileSystem(context);
    ConfigureCors(context, configuration);
    ConfigureSwaggerServices(context);
    ConfigureIdentityOptions(context);
}

private void ConfigureIdentityOptions(ServiceConfigurationContext context)
{
    context.Services.Configure<IdentityOptions>(options =>
    {
      options.SignIn.RequireConfirmedAccount = true;
      options.SignIn.RequireConfirmedEmail = true;
      options.SignIn.RequireConfirmedPhoneNumber = false;
    });
}
```

## Complete user registration flow

### Add `RegisterConfirmation` and `ConfirmEmail` pages to the Pages\Account folder of the **HttpApi.Host** project

* Create a new file **RegisterConfirmation.cshtml** to the **Pages\Account**  folder and copy/paste the code below.

```html
@page
@model AbpUserVerificationByEmail.HttpApi.Host.Pages.Account.CustomRegisterConfirmationModel
@{
    ViewData["Title"] = "Register confirmation";
}

<div class="card mt-3 shadow-sm rounded">
    <div class="card-body p-5">
        <h4>@ViewData["Title"]</h4>
        @{
            if (@Model.DisplayConfirmAccountLink)
            {
                <p>
                    This app does not currently have a real email sender registered, see <a
                        href="https://aka.ms/aspaccountconf">these docs</a> for how to configure a real email sender.
                    Normally this would be emailed: <a id="confirm-link" href="@Model.EmailConfirmationUrl">Click here to
                        confirm your account</a>
                </p>
            }
            else
            {
                <p>
                    Please check your email to confirm your account.
                </p>
            }
        }
    </div>
</div>
```  

* Create a new file **RegisterConfirmationModel.cs** to the **Pages\Account** folder and copy/paste the code below.
  
```csharp
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;

namespace AbpUserVerificationByEmail.HttpApi.Host.Pages.Account
{

  [AllowAnonymous]
  public class CustomRegisterConfirmationModel : PageModel
  {
    private readonly IdentityUserManager _userManager;
    private readonly IEmailSender _sender;
    public bool DisplayConfirmAccountLink { get; set; }
    public string EmailConfirmationUrl { get; set; }

    public CustomRegisterConfirmationModel(IdentityUserManager userManager, IEmailSender sender)
    {
      _userManager = userManager;
      _sender = sender;
    }

    public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
    {
      if (email == null)
      {
        return RedirectToPage("/Index");
      }

      var user = await _userManager.FindByEmailAsync(email);
      if (user == null)
      {
        return NotFound($"Unable to load user with email '{email}'.");
      }

      // TODO Set to true if you want to display the Account/ConfirmEmail page
      DisplayConfirmAccountLink = false;
      if (DisplayConfirmAccountLink)
      {
        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        EmailConfirmationUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { userId = userId, code = code },
            protocol: Request.Scheme);
      }
      return Page();
    }
  }
}
```

* Create a new file **ConfirmEmail.cshtml** to the **Pages\Account** folder and copy/paste the code below.

```html
@page
@model AbpUserVerificationByEmail.HttpApi.Host.Pages.Account.CustomConfirmEmailModel
@{
    ViewData["Title"] = "Email Confirmed";
}

<div class="card mt-3 shadow-sm rounded">
    <div class="card-body p-5">
        <h4>@ViewData["Title"]</h4>
        <hr>
        Email Successfully Confirmed
    </div>
</div>
```  

* Create a new file **ConfirmEmailModel.cs** to the **Pages\Account** folder and copy/paste the code below.

```csharp
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

        public CustomConfirmEmailModel(IdentityUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return Page();
        }
    }
}
```

### Update file RegisterModel in HttpApi.Host project

* Comment out/delete the **RegisterLocalUserAsync** method.
* Add method **OnPostAsync** to handle the Registration flow (see complete code below)

```csharp
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
```

## Test Registration flow and User Email Verification

* If you already registered a user, delete it first in table **AbpUsers** in the database.
* Start both the **Blazor** and **HttpApi.Host** project to run the application.
* Navigate to the **Login** page and click on the  [Register](https://localhost:44367/) link.
* Fill in the form of the **My Custom Register Page** and click the **Register** button.
* Goto your email inbox and click on the [clicking here](https://localhost:44367/) link to confirm your account.
* Navigate to the **Login** page and enter your credentials. Click **Login**.

Et voilà! This is the result. The User is successfully verified by email.

![User verified by email](images/userverifiedbyemail.jpg)

You can now force a user to confirm his email address before he can use your application.

Click on the links to find more about [Sending Emails](https://docs.abp.io/en/abp/latest/Emailing) and the [Account Module](https://docs.abp.io/en/abp/latest/Modules/Account)

Get the [source code](https://github.com/bartvanhoey/AbpUserVerificationByEmail) on GitHub.

Enjoy and have fun!
