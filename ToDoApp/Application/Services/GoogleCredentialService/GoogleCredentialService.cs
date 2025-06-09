using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using ToDoApp.Domains.AppSettingsConfigurations;

namespace ToDoApp.Application.Services.GoogleCredentialService
{
    public class GoogleCredentialService : IGoogleCredentialService
    {
        private readonly GoogleAuthSettings _gooGleAuthSettings;

        public GoogleCredentialService(IOptions<GoogleAuthSettings> gooGleAuthOptions)
        {
            _gooGleAuthSettings = gooGleAuthOptions.Value;
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyCredential(string credential)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = [_gooGleAuthSettings.ClientId]
            };

            try
            {
                // Verify the token and get the payload
                var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);
                return payload;
            }
            catch (Exception ex)
            {
                // Token invalid or verification failed
                Console.WriteLine($"Invalid ID token: {ex.Message}");
                return null;
            }
        }
    }
}
