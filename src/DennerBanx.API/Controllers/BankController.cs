using DennerBanx.Application.UseCases;
using DennerBanx.Communication.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DennerBanx.API.Controllers;
[Route("/")]
[ApiController]
public class BankController : ControllerBase
{
    private readonly AccountUseCase _accountUseCase;

    public BankController(AccountUseCase accountUseCase)
    {
        _accountUseCase = accountUseCase;
    }

    [HttpPost("reset")]
    public IActionResult Reset()
    {
        _accountUseCase.Reset();
        return Content("OK", "text/plan");
    }

    [HttpGet("balance")]
    public IActionResult GetBalance([FromQuery] string account_id)
    {
        var balance = _accountUseCase.GetBalance(account_id);
        if (balance == null)
        {
            return NotFound(0);
        }
        return Ok(balance);
    }


    [HttpPost("event")]
    public IActionResult Event([FromBody] RequestEventJson request)
    {
        object result = request.Type.ToLower() switch
        {
            "deposit" => _accountUseCase.HandleDeposit(request),
            "withdraw" => _accountUseCase.HandleWithdraw(request),
            "transfer" => _accountUseCase.HandleTransfer(request),
            _ => BadRequest()
        };

        if (result == null)
        {
            return NotFound(0);
        }

        return Created(string.Empty, result);
    }
}
