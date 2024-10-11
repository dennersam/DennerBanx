using DennerBanx.Domain.Entities;

namespace DennerBanx.Domain.Repositories.Accounts;
public interface IAccountRepository
{
    public Account GetAccount(string accountId);

    public void CreateAccount(string accountId, decimal initialBalance);

    public void UpdateAccountBalance(string accountId, decimal newBalance);

    public void Reset();
}
