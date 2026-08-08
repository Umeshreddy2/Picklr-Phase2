using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Picklr.Models
{
    // Custom validation attribute: the person booking must be at least a
    // certain number of years old.
    //
    // It implements BOTH halves of the Chapter 11 pattern:
    //   * ValidationAttribute.IsValid()          -> server-side check
    //   * IClientModelValidator.AddValidation()  -> emits the data-val-*
    //                                               attributes that
    //                                               minimum-age.js reacts to
    //
    // The client-side half is built on top of the server-side half, so if the
    // user disables JavaScript the rule is still enforced on the server.
    public class MinimumAgeAttribute : ValidationAttribute, IClientModelValidator
    {
        private int minYears;

        // Constructor accepts the age limit, e.g. [MinimumAge(13)]
        public MinimumAgeAttribute(int years)
        {
            minYears = years;
        }

        // ---- Server-side validation --------------------------------------
        // Overrides IsValid() of the ValidationAttribute base class.
        // Add minYears to the date of birth; if that lands on or before today,
        // the person is old enough.
        protected override ValidationResult IsValid(object? value,
            ValidationContext ctx)
        {
            if (value is DateTime)
            {
                DateTime dateToCheck = (DateTime)value;
                dateToCheck = dateToCheck.AddYears(minYears);
                if (dateToCheck <= DateTime.Today)
                {
                    return ValidationResult.Success!;
                }
            }

            return new ValidationResult(GetMsg(ctx.DisplayName ?? "Date"));
        }

        // ---- Client-side validation --------------------------------------
        // Implements AddValidation() of IClientModelValidator so MVC emits
        // data-val-* attributes into the HTML. jQuery unobtrusive validation
        // reads those and calls the matching method in minimum-age.js.
        public void AddValidation(ClientModelValidationContext ctx)
        {
            if (!ctx.Attributes.ContainsKey("data-val"))
                ctx.Attributes.Add("data-val", "true");

            ctx.Attributes.Add("data-val-minimumage-years",
                minYears.ToString());

            ctx.Attributes.Add("data-val-minimumage",
                GetMsg(ctx.ModelMetadata.DisplayName ??
                       ctx.ModelMetadata.Name ?? "Date"));
        }

        // Shared message builder -- removes duplicated code between the
        // server-side and client-side halves above.
        private string GetMsg(string name) =>
            base.ErrorMessage ??
                $"{name} must be at least {minYears} years ago.";
    }
}
