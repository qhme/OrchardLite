using Orchard.Security;
using System;
namespace Orchard.Users.Services
{
    public interface IUserService : IDependency
    {
        bool VerifyUserUnicity(string userName, string email);
        bool VerifyUserUnicity(int id, string userName, string email);

        IUser ValidateChallenge(string challengeToken);

        IUser ValidateLostPassword(string nonce);

        string CreateNonce(IUser user, TimeSpan delay);
        bool DecryptNonce(string challengeToken, out string username, out DateTime validateByUtc);
    }
}