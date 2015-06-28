using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Security
{
    public class CreateUserParams
    {
        private readonly string _username;
        private readonly string _password;
        private readonly string _email;
        private readonly bool _isApproved;

        public CreateUserParams(string username, string password, string email, bool isApproved)
        {
            _username = username;
            _password = password;
            _email = email;
            _isApproved = isApproved;
        }

        public string Username
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _password; }
        }

        public string Email
        {
            get { return _email; }
        }


        public bool IsApproved
        {
            get { return _isApproved; }
        }
    }
}
