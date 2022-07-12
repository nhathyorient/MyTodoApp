using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Todos.Common.Validation.Abstract;

namespace Todos.Common.Validation
{
    /// <summary>
    /// WHAT: ...
    /// WHY: Use ExpressionValidator help reuse logic for validation and make code cleaner
    /// You can reuse the ValidExpr and also and do EnsureXXX (throw exception) cleaner
    /// </summary>
    public class ExpressionValidator<T> : Validator<T>
    {
        public ExpressionValidator(Expression<Func<T, bool>> validExpr, string errorMsg) : base(errorMsg)
        {
            ValidExpr = validExpr;
        }

        public Expression<Func<T, bool>> ValidExpr { get; init; }

        public override ValidationResult Validate(T validationTarget)
        {
            var isValid = ValidExpr.Compile()(validationTarget);

            return isValid ? ValidationResult.Success : ValidationResult.Failure(ErrorMsg);
        }
    }
}
