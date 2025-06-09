using Google.Apis.Auth;

namespace ToDoApp.Application.Services.GoogleCredentialService
{
    public interface IGoogleCredentialService
    {
        public Task<GoogleJsonWebSignature.Payload> VerifyCredential(string credential);
    }
}
