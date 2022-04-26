using FluentValidation;
using ParkingManagement.ViewModels.Systems.RequestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.ViewModels.FluentValidations
{
    public class RoleCreateRequestValidator : AbstractValidator<RoleCreateRequest>
    {
        public RoleCreateRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id value is required")
                .MaximumLength(50).WithMessage("Role id cannot over limit 50 characters");

            RuleFor(x => x.Name).NotEmpty().WithMessage("Role name is required");
        }
    }
}
