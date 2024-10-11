using DennerBanx.Domain.Entities;

namespace DennerBanx.Domain.Repositories.Accounts;
public interface IAccountRepository
{
    Account GetAccount(string accountId);

    void CreateAccount(string accountId, decimal initialBalance);

    void UpdateAccountBalance(string accountId, decimal newBalance);

    void Reset();
}
