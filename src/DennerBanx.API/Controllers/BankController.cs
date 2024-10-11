using DennerBanx.Communication.Requests;
using DennerBanx.Infraestructure.DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DennerBanx.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BankController : ControllerBase
{
    private readonly AccountRepository _repository;

    public BankController(AccountRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("reset")]
    public IActionResult Reset()
    {
        _repository.Reset();
        return Ok();
    }

    [HttpGet("balance")]
    public IActionResult GetBalance([FromQuery] string account_id)
    {
        var account = _repository.GetAccount(account_id);
        if (account == null)
        {
            return NotFound(0);
        }
        return Ok(account.Balance);
    }


    [HttpPost("event")]
    public IActionResult Event([FromBody] RequestEventJson request)
    {
        switch (request.Type.ToLower())
        {
            case "deposit":
                return HandleDeposit(request);
            case "withdraw":
                return HandleWithdraw(request);
            case "transfer":
                return HandleTransfer(request);
            default:
                return BadRequest();
        }
    }

    private IActionResult HandleDeposit(RequestEventJson request)
    {
        var account = _repository.GetAccount(request.Destination);
        if (account == null)
        {
            _repository.CreateAccount(request.Destination, request.Amount);
            return Created("", new { destination = new { id = request.Destination, balance = request.Amount } });
        }

        account.Balance += request.Amount;
        _repository.UpdateAccountBalance(request.Destination, account.Balance);
        return Created("", new { destination = new { id = request.Destination, balance = account.Balance } });
    }

    private IActionResult HandleWithdraw(RequestEventJson request)
    {
        var account = _repository.GetAccount(request.Origin);
        if (account == null || account.Balance < request.Amount)
        {
            return NotFound(0);
        }

        account.Balance -= request.Amount;
        _repository.UpdateAccountBalance(request.Origin, account.Balance);
        return Created("", new { origin = new { id = request.Origin, balance = account.Balance } });
    }

    private IActionResult HandleTransfer(RequestEventJson request)
    {
        var originAccount = _repository.GetAccount(request.Origin);
        var destinationAccount = _repository.GetAccount(request.Destination);

        if (originAccount == null || originAccount.Balance < request.Amount)
        {
            return NotFound(0);
        }

        if (destinationAccount == null)
        {
            _repository.CreateAccount(request.Destination, 0);
            destinationAccount = _repository.GetAccount(request.Destination);
        }

        originAccount.Balance -= request.Amount;
        destinationAccount.Balance += request.Amount;

        _repository.UpdateAccountBalance(request.Origin, originAccount.Balance);
        _repository.UpdateAccountBalance(request.Destination, destinationAccount.Balance);

        return Created(string.Empty, new
        {
            origin = new { id = request.Origin, balance = originAccount.Balance },
            destination = new { id = request.Destination, balance = destinationAccount.Balance }
        });
    }
}
