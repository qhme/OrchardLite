using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Users.ViewModels;

namespace Orchard.Users.Validators
{
    public class UserCreateValidator : AbstractValidator<UserCreateViewModel>
    {
        public UserCreateValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Please input the username.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Please input the password.");

            RuleFor(x => x.Email).EmailAddress().WithMessage("You must specify a valid email address.");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Password confirmation must match.");
        }
    }
}