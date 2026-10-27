using Gates.Services;

namespace Gates.Tests;

public sealed class AccessCodeValidatorTests
{
    private readonly AccessCodeValidator _validator = new();

    [Theory]
    [InlineData("Dragon7!")]
    [InlineData("Abcdef1!")]
    [InlineData("Dragon7!Dragon7!")]
    [InlineData("Abc@def$%5")]
    [InlineData("Aabb7!xy7")]
    [InlineData("Ab12!cde7")]
    public void Validate_ValidCode_ReturnsNoErrors(string accessCode)
    {
        var result = _validator.Validate(accessCode);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.False(result.IsCodeMissing);
    }

    [Theory]
    [InlineData("dragon123",
        "Нет заглавной латинской буквы.",
        "Нет специального символа.",
        "Обнаружена последовательность из трёх или более подряд идущих цифр.")]
    [InlineData("abcdefg1!", "Нет заглавной латинской буквы.")]
    [InlineData("ABCDEFG1!", "Нет строчной латинской буквы.")]
    [InlineData("Abcdefgh!", "Нет цифры.")]
    [InlineData("Abcdefg12", "Нет специального символа.")]
    [InlineData("Ab1!", "Длина кода должна быть от 8 до 16 символов.")]
    [InlineData("Abcdefgh1!Abcdefg7", "Длина кода должна быть от 8 до 16 символов.")]
    [InlineData("Abc def1!", "Код не должен содержать пробелов.")]
    [InlineData("Aaaa7!xy7", "Обнаружены три одинаковых символа подряд.")]
    [InlineData("Abc123!x7", "Обнаружена последовательность из трёх или более подряд идущих цифр.")]
    [InlineData("Abc12345!7", "Обнаружена последовательность из трёх или более подряд идущих цифр.")]
    [InlineData("7Ragon7!", "Первый символ должен быть латинской буквой.")]
    [InlineData("Dragon7!x", "Последний символ должен быть цифрой или специальным символом.")]
    [InlineData("Abcdef1&", "Нет специального символа.", "Последний символ должен быть цифрой или специальным символом.")]
    [InlineData("Дракон7!",
        "Нет заглавной латинской буквы.",
        "Нет строчной латинской буквы.",
        "Первый символ должен быть латинской буквой.")]
    public void Validate_InvalidCode_ReturnsAllViolations(string accessCode, params string[] expectedErrors)
    {
        var result = _validator.Validate(accessCode);

        Assert.False(result.IsValid);
        Assert.Equal(expectedErrors, result.Errors);
        Assert.False(result.IsCodeMissing);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("        ")]
    public void Validate_MissingOrEmptyCode_ReportsMissingCode(string? accessCode)
    {
        var result = _validator.Validate(accessCode);

        Assert.False(result.IsValid);
        Assert.True(result.IsCodeMissing);
        Assert.Equal(new[] { AccessCodeValidator.MissingCodeError }, result.Errors);
    }

    [Fact]
    public void Validate_ReportsEveryViolationOfSeveralRules()
    {
        var result = _validator.Validate("1a 234567");

        Assert.False(result.IsValid);
        Assert.Equal(
            new[]
            {
                AccessCodeValidator.MissingUppercaseError,
                AccessCodeValidator.MissingSpecialCharacterError,
                AccessCodeValidator.WhitespaceError,
                AccessCodeValidator.DigitSequenceError,
                AccessCodeValidator.FirstCharacterError
            }, result.Errors);
    }
}
