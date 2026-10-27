namespace Gates.Services;

public sealed record AccessCodeValidationResult
{
    public bool IsValid { get; init; }

    public IReadOnlyList<string> Errors { get; init; } = [];

    public bool IsCodeMissing { get; init; }
}
