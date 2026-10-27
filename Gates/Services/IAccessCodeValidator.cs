namespace Gates.Services;

public interface IAccessCodeValidator
{
    AccessCodeValidationResult Validate(string? accessCode);
}
