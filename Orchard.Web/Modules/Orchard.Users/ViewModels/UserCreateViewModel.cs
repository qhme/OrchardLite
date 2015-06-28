using FluentValidation.Attributes;
using Orchard.ContentManagement;
using Orchard.Users.Models;
using Orchard.Users.Validators;

namespace Orchard.Users.ViewModels
{

    [Validator(typeof(UserCreateValidator))]
    public class UserCreateViewModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}