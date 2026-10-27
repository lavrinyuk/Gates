namespace Gates.Models;

public sealed class ValidateAccessCodeResponse
{
    public bool IsValid { get; init; }

    public IReadOnlyList<string> Errors { get; init; } = [];

    public string Message { get; init; } = string.Empty;

    public static ValidateAccessCodeResponse Accepted() => new()
    {
        IsValid = true,
        Errors = [],
        Message = "Код принят. Ворота открываются."
    };

    public static ValidateAccessCodeResponse Rejected(
        IReadOnlyList<string> errors,
        string message = "Код отклонён.") => new()
    {
        IsValid = false,
        Errors = errors,
        Message = message
    };
}
