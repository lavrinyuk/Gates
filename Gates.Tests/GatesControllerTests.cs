using Gates.Controllers;
using Gates.Models;
using Gates.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gates.Tests;

public sealed class GatesControllerTests
{
    private readonly GatesController _controller = new(new AccessCodeValidator());

    [Fact]
    public void Validate_ValidCode_ReturnsOkWithAcceptedResponse()
    {
        var result = _controller.Validate(new ValidateAccessCodeRequest { AccessCode = "Dragon7!" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<ValidateAccessCodeResponse>(ok.Value);
        Assert.True(body.IsValid);
        Assert.Empty(body.Errors);
        Assert.Equal("Код принят. Ворота открываются.", body.Message);
    }

    [Fact]
    public void Validate_InvalidCode_ReturnsOkWithRejectionReasons()
    {
        var result = _controller.Validate(new ValidateAccessCodeRequest { AccessCode = "dragon123" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var body = Assert.IsType<ValidateAccessCodeResponse>(ok.Value);
        Assert.False(body.IsValid);
        Assert.Equal(3, body.Errors.Count);
        Assert.Equal("Код отклонён.", body.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_MissingCode_ReturnsBadRequest(string? accessCode)
    {
        var result = _controller.Validate(accessCode is null ? null : new ValidateAccessCodeRequest { AccessCode = accessCode });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var body = Assert.IsType<ValidateAccessCodeResponse>(badRequest.Value);
        Assert.False(body.IsValid);
        Assert.Equal(new[] { AccessCodeValidator.MissingCodeError }, body.Errors);
        Assert.Equal("Некорректный запрос: код доступа отсутствует или пуст.", body.Message);
    }
}
