using Gates.Models;
using Gates.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gates.Controllers;

[ApiController]
[Route("api/gates")]
[Produces("application/json")]
public sealed class GatesController : ControllerBase
{
    private readonly IAccessCodeValidator _validator;

    public GatesController(IAccessCodeValidator validator)
    {
        _validator = validator;
    }

    /// <summary>
    /// Проверяет код доступа и возвращает все найденные нарушения.
    /// </summary>
    /// <response code="200">Результат проверки: код принят или отклонён.</response>
    /// <response code="400">Код доступа не передан или пуст.</response>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidateAccessCodeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidateAccessCodeResponse), StatusCodes.Status400BadRequest)]
    public ActionResult<ValidateAccessCodeResponse> Validate(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ValidateAccessCodeRequest? request)
    {
        var result = _validator.Validate(request?.AccessCode);

        if (result.IsCodeMissing)
        {
            return BadRequest(ValidateAccessCodeResponse.Rejected(
                result.Errors,
                "Некорректный запрос: код доступа отсутствует или пуст."));
        }

        return result.IsValid
            ? Ok(ValidateAccessCodeResponse.Accepted())
            : Ok(ValidateAccessCodeResponse.Rejected(result.Errors));
    }
}
