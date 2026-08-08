// Client-side half of the MinimumAge custom validation.
//
// MinimumAgeAttribute.AddValidation() emits these attributes onto the input:
//     data-val="true"
//     data-val-minimumage-years="13"
//     data-val-minimumage="Date of Birth must be at least 13 years ago."
//
// jQuery unobtrusive validation reads them and calls the method below.

jQuery.validator.addMethod("minimumage",
    function (value, element, param) {
        // no value entered -- let [Required] report it, don't double up
        if (value === '') return true;

        var dateToCheck = new Date(value);
        if (isNaN(dateToCheck)) return false;   // not a real date

        var minYears = Number(param);

        // add the minimum years to the date of birth
        dateToCheck.setFullYear(dateToCheck.getFullYear() + minYears);

        var today = new Date();
        return (dateToCheck <= today);
    });

// Register the method with the unobtrusive adapter. addSingleVal is used
// because this rule takes exactly one parameter ("years").
jQuery.validator.unobtrusive.adapters.addSingleVal(
    "minimumage", "years");
