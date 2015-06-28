using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Logging;
using Orchard.ContentManagement;
using Orchard.Settings;
using Orchard.Users.Models;
using Orchard.Security;
using System.Xml.Linq;
using Orchard.Services;
using System.Globalization;
using System.Text;
using Orchard.Environment.Configuration;
using Orchard.Data;

namespace Orchard.Users.Services
{
    public class UserService : IUserService
    {
        private static readonly TimeSpan DelayToValidate = new TimeSpan(7, 0, 0, 0); // one week to validate email
        private static readonly TimeSpan DelayToResetPassword = new TimeSpan(1, 0, 0, 0); // 24 hours to reset password

        private readonly IMembershipService _membershipService;
        private readonly IClock _clock;
        private readonly IRepository<UserRecord> _userRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly ISiteService _siteService;

        public UserService(
             IMembershipService membershipService,
            IClock clock,
            IRepository<UserRecord> userRepository,
             ShellSettings shellSettings,
            IEncryptionService encryptionService,
             ISiteService siteService
            )
        {
            _userRepository = userRepository;
            _membershipService = membershipService;
            _clock = clock;
            _encryptionService = encryptionService;
            _siteService = siteService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool VerifyUserUnicity(string userName, string email)
        {
            string normalizedUserName = userName.ToLowerInvariant();

            if (_userRepository.Table.Any(x => x.Email == email || x.UserName == userName))
            {
                return false;
            }

            return true;
        }

        public bool VerifyUserUnicity(int id, string userName, string email)
        {
            string normalizedUserName = userName.ToLowerInvariant();

            if (_userRepository.Table.Any(x => x.Id != id && (x.Email == email || x.UserName == userName)))
            {
                return false;
            }

            return true;
        }

        public string CreateNonce(IUser user, TimeSpan delay)
        {
            var challengeToken = new XElement("n", new XAttribute("un", user.UserName), new XAttribute("utc", _clock.UtcNow.ToUniversalTime().Add(delay).ToString(CultureInfo.InvariantCulture))).ToString();
            var data = Encoding.UTF8.GetBytes(challengeToken);
            return Convert.ToBase64String(_encryptionService.Encode(data));
        }

        public bool DecryptNonce(string nonce, out string username, out DateTime validateByUtc)
        {
            username = null;
            validateByUtc = _clock.UtcNow;

            try
            {
                var data = _encryptionService.Decode(Convert.FromBase64String(nonce));
                var xml = Encoding.UTF8.GetString(data);
                var element = XElement.Parse(xml);
                username = element.Attribute("un").Value;
                validateByUtc = DateTime.Parse(element.Attribute("utc").Value, CultureInfo.InvariantCulture);
                return _clock.UtcNow <= validateByUtc;
            }
            catch
            {
                return false;
            }

        }

        public IUser ValidateChallenge(string nonce)
        {
            string username;
            DateTime validateByUtc;

            if (!DecryptNonce(nonce, out username, out validateByUtc))
            {
                return null;
            }

            if (validateByUtc < _clock.UtcNow)
                return null;

            var user = _membershipService.GetUser(username);
            if (user == null)
                return null;

            (user as UserRecord).EmailStatus = UserStatus.Approved;

            return user;
        }


        public IUser ValidateLostPassword(string nonce)
        {
            string username;
            DateTime validateByUtc;

            if (!DecryptNonce(nonce, out username, out validateByUtc))
            {
                return null;
            }

            if (validateByUtc < _clock.UtcNow)
                return null;

            var user = _membershipService.GetUser(username);
            if (user == null)
                return null;

            return user;
        }
    }
}