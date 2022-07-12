namespace Todos.Common.Validation.Abstract;

public abstract class Validator<T>
{
    public Validator(string errorMsg)
    {
        ErrorMsg = errorMsg;
    }

    public string ErrorMsg { get; init; }

    public abstract ValidationResult Validate(T validationTarget);
}