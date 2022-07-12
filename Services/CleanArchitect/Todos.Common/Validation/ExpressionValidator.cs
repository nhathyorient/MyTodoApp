using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Todos.Common.Validation.Abstract;

namespace Todos.Common.Validation
{
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
