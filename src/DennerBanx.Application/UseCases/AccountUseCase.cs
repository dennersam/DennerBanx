using DennerBanx.Communication.Requests;
using DennerBanx.Domain.Repositories.Accounts;

namespace DennerBanx.Application.UseCases;
public class AccountUseCase
{
    private readonly IAccountRepository _accountRepository;

    public AccountUseCase(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public void Reset()
    {
        _accountRepository.Reset();
    }

    public decimal? GetBalance(string accountId)
    {
        var account = _accountRepository.GetAccount(accountId);
        return account?.Balance;
    }

    public object HandleDeposit(RequestEventJson request)
    {
        var account = _accountRepository.GetAccount(request.Destination);
        if (account == null)
        {
            _accountRepository.CreateAccount(request.Destination, request.Amount);
            return new { destination = new { id = request.Destination, balance = request.Amount } };
        }

        account.Balance += request.Amount;
        _accountRepository.UpdateAccountBalance(request.Destination, account.Balance);
        return new { destination = new { id = request.Destination, balance = account.Balance } };
    }

    public object HandleWithdraw(RequestEventJson request)
    {
        var account = _accountRepository.GetAccount(request.Origin);
        if (account == null || account.Balance < request.Amount)
        {
            return null;
        }

        account.Balance -= request.Amount;
        _accountRepository.UpdateAccountBalance(request.Origin, account.Balance);
        return new { origin = new { id = request.Origin, balance = account.Balance } };
    }

    public object HandleTransfer(RequestEventJson request)
    {
        var originAccount = _accountRepository.GetAccount(request.Origin);
        var destinationAccount = _accountRepository.GetAccount(request.Destination);

        if (originAccount == null || originAccount.Balance < request.Amount)
        {
            return null;
        }

        if (destinationAccount == null)
        {
            _accountRepository.CreateAccount(request.Destination, 0);
            destinationAccount = _accountRepository.GetAccount(request.Destination);
        }

        originAccount.Balance -= request.Amount;
        destinationAccount.Balance += request.Amount;

        _accountRepository.UpdateAccountBalance(request.Origin, originAccount.Balance);
        _accountRepository.UpdateAccountBalance(request.Destination, destinationAccount.Balance);

        return new
        {
            origin = new { id = request.Origin, balance = originAccount.Balance },
            destination = new { id = request.Destination, balance = destinationAccount.Balance }
        };
    }
}
