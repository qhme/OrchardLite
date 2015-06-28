using FluentValidation.Attributes;
using Orchard.ContentManagement;
using Orchard.Users.Models;
using Orchard.Users.Validators;

namespace Orchard.Users.ViewModels
{
    [Validator(typeof(UserEditValidator))]
    public class UserEditViewModel
    {
        public string UserName
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public int Id { get; set; }

    }
}