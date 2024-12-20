using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class OptionalEmailAddressAttribute : ValidationAttribute
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success; // Пустое значение допустимо
        }

        if (!EmailRegex.IsMatch(value.ToString()))
        {
            return new ValidationResult("Некорректный формат Email.");
        }

        return ValidationResult.Success;
    }
}
