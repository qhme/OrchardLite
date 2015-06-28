using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;
using Orchard.Logging;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Users.Events;
using Orchard.Users.Models;
using System.Collections.Generic;
using Orchard.Services;
using System.Web.Helpers;
using Orchard.Environment.Configuration;
using Orchard.Data;
using System.Configuration;
using Orchard.Validation;

namespace Orchard.Users.Services
{
    public class MembershipService : IMembershipService
    {
        private const string PBKDF2 = "PBKDF2";
        private const string DefaultHashAlgorithm = PBKDF2;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<UserRecord> _userRepository;
        private readonly IUserEventHandler _userEventHandlers;
        private readonly IEncryptionService _encryptionService;

        public MembershipService(
            IOrchardServices OrchardService,
            IRepository<UserRecord> userRepository,
            IUserEventHandler userEventHandlers,
            IClock clock,
            IEncryptionService encryptionService)
        {
            _orchardServices = OrchardService;
            _userEventHandlers = userEventHandlers;
            _encryptionService = encryptionService;
            _userRepository = userRepository;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public MembershipSettings GetSettings()
        {
            var settings = new MembershipSettings();
            // accepting defaults
            return settings;
        }

        public IUser CreateUser(CreateUserParams createUserParams)
        {
            Logger.Information("CreateUser {0} {1}", createUserParams.Username, createUserParams.Email);

            var user = new UserRecord();

            user.UserName = createUserParams.Username;
            user.Email = createUserParams.Email;
            user.NormalizedUserName = createUserParams.Username.ToLowerInvariant();
            user.HashAlgorithm = PBKDF2;
            SetPassword(user, createUserParams.Password);



            user.RegistrationStatus = UserStatus.Approved;
            user.EmailStatus = UserStatus.Approved;


            if (createUserParams.IsApproved)
            {
                user.RegistrationStatus = UserStatus.Approved;
                user.EmailStatus = UserStatus.Approved;
            }

            var userContext = new UserContext { User = user, Cancel = false, UserParameters = createUserParams };
            _userEventHandlers.Creating(userContext);

            if (userContext.Cancel)
            {
                return null;
            }

            _userRepository.Create(user);

            _userEventHandlers.Created(userContext);
            if (user.RegistrationStatus == UserStatus.Approved)
            {
                _userEventHandlers.Approved(user);
            }

            return user;
        }

        public IUser GetUser(string username)
        {
            var lowerName = username == null ? "" : username.ToLowerInvariant();
            return _userRepository.Table.FirstOrDefault(x => x.UserName == username);
        }

        public IUser GetUser(int id)
        {
            Argument.Validate(id > 0, "user id");
            return _userRepository.Table.FirstOrDefault(x => x.Id == id);
        }


        public IUser ValidateUser(string userNameOrEmail, string password)
        {
            var lowerName = userNameOrEmail == null ? "" : userNameOrEmail.ToLowerInvariant();

            var user = GetUser(userNameOrEmail) as UserRecord;

            if (user == null)
                user = _userRepository.Table.FirstOrDefault(x => x.Email == userNameOrEmail);

            if (user == null || ValidatePassword(user, password) == false)
                return null;

            if (user.EmailStatus != UserStatus.Approved)
                return null;

            if (user.RegistrationStatus != UserStatus.Approved)
                return null;

            return user;
        }

        public void SetPassword(IUser user, string password)
        {
            if (!(user is UserRecord))
                throw new InvalidCastException();

            var UserRecord = user as UserRecord;

            switch (GetSettings().PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    SetPasswordClear(UserRecord, password);
                    break;
                case MembershipPasswordFormat.Hashed:
                    SetPasswordHashed(UserRecord, password);
                    break;
                case MembershipPasswordFormat.Encrypted:
                    SetPasswordEncrypted(UserRecord, password);
                    break;
                default:
                    throw new ApplicationException("Unexpected password format value");
            }
        }

        private bool ValidatePassword(UserRecord UserRecord, string password)
        {
            // Note - the password format stored with the record is used
            // otherwise changing the password format on the site would invalidate
            // all logins
            switch (UserRecord.PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    return ValidatePasswordClear(UserRecord, password);
                case MembershipPasswordFormat.Hashed:
                    return ValidatePasswordHashed(UserRecord, password);
                case MembershipPasswordFormat.Encrypted:
                    return ValidatePasswordEncrypted(UserRecord, password);
                default:
                    throw new ApplicationException("Unexpected password format value");
            }
        }

        private static void SetPasswordClear(UserRecord UserRecord, string password)
        {
            UserRecord.PasswordFormat = MembershipPasswordFormat.Clear;
            UserRecord.Password = password;
            UserRecord.PasswordSalt = null;
        }

        private static bool ValidatePasswordClear(UserRecord UserRecord, string password)
        {
            return UserRecord.Password == password;
        }

        private static void SetPasswordHashed(UserRecord UserRecord, string password)
        {
            var saltBytes = new byte[0x10];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(saltBytes);
            }

            UserRecord.PasswordFormat = MembershipPasswordFormat.Hashed;
            UserRecord.Password = ComputeHashBase64(UserRecord.HashAlgorithm, saltBytes, password);
            UserRecord.PasswordSalt = Convert.ToBase64String(saltBytes);
        }

        private bool ValidatePasswordHashed(UserRecord UserRecord, string password)
        {
            var saltBytes = Convert.FromBase64String(UserRecord.PasswordSalt);

            bool isValid;
            if (UserRecord.HashAlgorithm == PBKDF2)
            {
                // We can't reuse ComputeHashBase64 as the internally generated salt repeated calls to Crypto.HashPassword() return different results.
                isValid = Crypto.VerifyHashedPassword(UserRecord.Password, Encoding.Unicode.GetString(CombineSaltAndPassword(saltBytes, password)));
            }
            else
            {
                isValid = SecureStringEquality(UserRecord.Password, ComputeHashBase64(UserRecord.HashAlgorithm, saltBytes, password));
            }

            // Migrating older password hashes to Default algorithm if necessary and enabled.
            if (isValid && UserRecord.HashAlgorithm != DefaultHashAlgorithm)
            {
                var keepOldConfiguration = ConfigurationManager.AppSettings["Orchard.Users.KeepOldPasswordHash"];
                if (String.IsNullOrEmpty(keepOldConfiguration) || keepOldConfiguration.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    UserRecord.HashAlgorithm = DefaultHashAlgorithm;
                    UserRecord.Password = ComputeHashBase64(UserRecord.HashAlgorithm, saltBytes, password);
                }
            }

            return isValid;
        }

        private static string ComputeHashBase64(string hashAlgorithmName, byte[] saltBytes, string password)
        {
            var combinedBytes = CombineSaltAndPassword(saltBytes, password);

            // Extending HashAlgorithm would be too complicated: http://stackoverflow.com/questions/6460711/adding-a-custom-hashalgorithmtype-in-c-sharp-asp-net?lq=1
            if (hashAlgorithmName == PBKDF2)
            {
                // HashPassword() already returns a base64 string.
                return Crypto.HashPassword(Encoding.Unicode.GetString(combinedBytes));
            }
            else
            {
                using (var hashAlgorithm = HashAlgorithm.Create(hashAlgorithmName))
                {
                    return Convert.ToBase64String(hashAlgorithm.ComputeHash(combinedBytes));
                }
            }
        }

        /// <summary>
        /// Compares two strings without giving hint about the time it takes to do so.
        /// </summary>
        /// <param name="a">The first string to compare.</param>
        /// <param name="b">The second string to compare.</param>
        /// <returns><c>true</c> if both strings are equal, <c>false</c>.</returns>
        private bool SecureStringEquality(string a, string b)
        {
            if (a == null || b == null || (a.Length != b.Length))
            {
                return false;
            }

            var aBytes = Encoding.Unicode.GetBytes(a);
            var bBytes = Encoding.Unicode.GetBytes(b);

            var bytesAreEqual = true;
            for (int i = 0; i < a.Length; i++)
            {
                bytesAreEqual &= (aBytes[i] == bBytes[i]);
            }

            return bytesAreEqual;
        }

        private static byte[] CombineSaltAndPassword(byte[] saltBytes, string password)
        {
            var passwordBytes = Encoding.Unicode.GetBytes(password);
            return saltBytes.Concat(passwordBytes).ToArray();
        }

        private void SetPasswordEncrypted(UserRecord UserRecord, string password)
        {
            UserRecord.Password = Convert.ToBase64String(_encryptionService.Encode(Encoding.UTF8.GetBytes(password)));
            UserRecord.PasswordSalt = null;
            UserRecord.PasswordFormat = MembershipPasswordFormat.Encrypted;
        }

        private bool ValidatePasswordEncrypted(UserRecord UserRecord, string password)
        {
            return String.Equals(password, Encoding.UTF8.GetString(_encryptionService.Decode(Convert.FromBase64String(UserRecord.Password))), StringComparison.Ordinal);
        }
    }
}
