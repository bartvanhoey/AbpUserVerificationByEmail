using Volo.Abp.Application.Services;
using Volo.Abp.Security.Encryption;

namespace AbpUserVerificationByEmail.Application
{

  // TODO remove this class completely after you grabbed your encrypted password in Swagger UI
  public class EncryptGmailPasswordAppService : ApplicationService
  {
    public IStringEncryptionService _encryptionService { get; set; }

    public string EncryptGmailPassword()
    {
      return _encryptionService.Encrypt("YourGmailPasswordHere");
    }
  }
}