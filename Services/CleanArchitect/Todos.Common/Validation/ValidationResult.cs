using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todos.Common.Validation
{
    public class ValidationResult
    {
        public static ValidationResult Success => new ValidationResult();

        public static ValidationResult Failure(string error)
        {
            return new ValidationResult(error);
        }

        public static ValidationResult Valid()
        {
            return new ValidationResult();
        }

        public static ValidationResult Invalid(
            params string[] errors)
        {
            return errors.Any()
                ? new ValidationResult(string.Join(";", errors))
                : new ValidationResult("Invalid!");
        }

        public static ValidationResult FailFast(
            params ValidationResult[] validations)
        {
            return validations.Aggregate(Valid(), (current, nextValidation) => current.IsValid ? nextValidation : current);
        }

        public static ValidationResult HarvestErrors(
            params ValidationResult[] validations)
        {
            var errors = validations.Where(p => !p.IsValid).Select(p => p.ToString()).ToArray();
            return !errors.Any() ? Valid() : Invalid(errors);
        }

        public ValidationResult()
        {
        }

        public ValidationResult(string error)
        {
            Error = error;
        }

        public bool IsValid => Error == null && !MemberErrors.Any();
        public string? Error { get; set; }
        public List<KeyValuePair<string, string>> MemberErrors { get; set; } = new List<KeyValuePair<string, string>>();

        public override string ToString()
        {
            if (!IsValid && !string.IsNullOrEmpty(Error) && !MemberErrors.Any()) return Error;
            return IsValid ? "Valid" : $"Invalid. Error: {Error ?? "Invalid!"}.{SummaryMemberErrors()}";
        }

        public string SummaryMemberErrors()
        {
            return MemberErrors?.Any() == true ? $"MemberErrors: {string.Join("; ", MemberErrors.Select(memberError => $"{memberError.Key}: {memberError.Value}"))}" : "";
        }

        public void EnsureValid(Func<string, Exception> exceptionForError)
        {
            if (!IsValid) throw exceptionForError(ToString());
        }

        /// <summary>
        /// WHAT: Use and next validation.
        /// WHY: Use function instead of directly ValidationResult because
        /// when you want to multiple and, the next validation rule only
        /// executed when the previous is valid
        /// </summary>
        public ValidationResult And(Func<ValidationResult> nextValidation)
        {
            return !IsValid ? this : nextValidation();
        }
    }
}
