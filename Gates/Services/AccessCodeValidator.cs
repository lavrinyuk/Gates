namespace Gates.Services;

public sealed class AccessCodeValidator : IAccessCodeValidator
{
    public const int MinLength = 8;
    public const int MaxLength = 16;

    public const string SpecialCharacters = "!@#$%";

    public const string MissingCodeError = "Код доступа не передан или пуст.";
    public const string LengthError = "Длина кода должна быть от 8 до 16 символов.";
    public const string MissingUppercaseError = "Нет заглавной латинской буквы.";
    public const string MissingLowercaseError = "Нет строчной латинской буквы.";
    public const string MissingDigitError = "Нет цифры.";
    public const string MissingSpecialCharacterError = "Нет специального символа.";
    public const string WhitespaceError = "Код не должен содержать пробелов.";
    public const string RepeatedCharacterError = "Обнаружены три одинаковых символа подряд.";
    public const string DigitSequenceError = "Обнаружена последовательность из трёх или более подряд идущих цифр.";
    public const string FirstCharacterError = "Первый символ должен быть латинской буквой.";
    public const string LastCharacterError = "Последний символ должен быть цифрой или специальным символом.";

    public AccessCodeValidationResult Validate(string? accessCode)
    {
        if (string.IsNullOrWhiteSpace(accessCode))
        {
            return new AccessCodeValidationResult
            {
                IsValid = false,
                Errors = [MissingCodeError],
                IsCodeMissing = true
            };
        }

        var errors = new List<string>();

        if (accessCode.Length is < MinLength or > MaxLength)
        {
            errors.Add(LengthError);
        }

        if (!ContainsLetter(accessCode, 'A', 'Z'))
        {
            errors.Add(MissingUppercaseError);
        }

        if (!ContainsLetter(accessCode, 'a', 'z'))
        {
            errors.Add(MissingLowercaseError);
        }

        if (!accessCode.Any(IsAsciiDigit))
        {
            errors.Add(MissingDigitError);
        }

        if (!accessCode.Any(IsSpecialCharacter))
        {
            errors.Add(MissingSpecialCharacterError);
        }

        if (accessCode.Any(char.IsWhiteSpace))
        {
            errors.Add(WhitespaceError);
        }

        if (ContainsRepeatedCharacterRun(accessCode))
        {
            errors.Add(RepeatedCharacterError);
        }

        if (ContainsLongDigitSequence(accessCode))
        {
            errors.Add(DigitSequenceError);
        }

        if (!IsLatinLetter(accessCode[0]))
        {
            errors.Add(FirstCharacterError);
        }

        var lastCharacter = accessCode[^1];
        if (!IsAsciiDigit(lastCharacter) && !IsSpecialCharacter(lastCharacter))
        {
            errors.Add(LastCharacterError);
        }

        return new AccessCodeValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors,
            IsCodeMissing = false
        };
    }

    private static bool ContainsLetter(string value, char from, char to) =>
        value.Any(c => c >= from && c <= to);

    private static bool ContainsRepeatedCharacterRun(string value)
    {
        for (var i = 2; i < value.Length; i++)
        {
            if (value[i] == value[i - 1] && value[i] == value[i - 2])
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsLongDigitSequence(string value)
    {
        var runLength = 0;

        foreach (var character in value)
        {
            if (!IsAsciiDigit(character))
            {
                runLength = 0;
                continue;
            }

            runLength++;
            if (runLength >= 3)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsLatinLetter(char character) =>
        character is >= 'A' and <= 'Z' or >= 'a' and <= 'z';

    private static bool IsAsciiDigit(char character) =>
        character is >= '0' and <= '9';

    private static bool IsSpecialCharacter(char character) =>
        SpecialCharacters.Contains(character);
}
