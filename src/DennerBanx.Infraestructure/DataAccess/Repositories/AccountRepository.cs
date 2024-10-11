using DennerBanx.Domain.Entities;
using DennerBanx.Domain.Repositories.Accounts;

namespace DennerBanx.Infraestructure.DataAccess.Repositories;
public class AccountRepository : IAccountRepository
{
    private readonly Dictionary<string, Account> _accounts = new();

    public Account GetAccount(string accountId)
    {
        _accounts.TryGetValue(accountId, out var account);
        return account;
    }

    public void CreateAccount(string accountId, decimal initialBalance)
    {
        _accounts[accountId] = new Account { Id = accountId, Balance = initialBalance };
    }

    public void UpdateAccountBalance(string accountId, decimal newBalance)
    {
        if (_accounts.ContainsKey(accountId))
        {
            _accounts[accountId].Balance = newBalance;
        }
    }

    public void Reset()
    {
        _accounts.Clear();
    }
}
