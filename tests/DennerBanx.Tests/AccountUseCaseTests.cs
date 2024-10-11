using DennerBanx.Application.UseCases;
using DennerBanx.Communication.Requests;
using DennerBanx.Domain.Entities;
using DennerBanx.Domain.Repositories.Accounts;
using Moq;
using Xunit;

namespace DennerBanx.Tests
{
    public class AccountUseCaseTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly AccountUseCase _accountUseCase;

        public AccountUseCaseTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _accountUseCase = new AccountUseCase(_accountRepositoryMock.Object);
        }

        [Fact]
        public void GetBalance_ShouldReturnBalance_WhenAccountExists()
        {
            var accountId = "100";
            var account = new Account() { Id = "100", Balance = 50 };
            _accountRepositoryMock.Setup(repo => repo.GetAccount(accountId)).Returns(account);

            var result = _accountUseCase.GetBalance(accountId);

            Assert.Equal(50, result);
        }

        [Fact]
        public void GetBalance_ShouldReturnNull_WhenAccountDoesNotExist()
        {
            var accountId = "1234";
            _accountRepositoryMock.Setup(repo => repo.GetAccount(accountId)).Returns((Account)null);

            var result = _accountUseCase.GetBalance(accountId);

            Assert.Null(result);
        }

        [Fact]
        public void HandleWithdraw_ShouldReturnNull_WhenAccountDoesNotExist()
        {
            var request = new RequestEventJson { Origin = "200", Amount = 10, Type = "withdraw" };
            _accountRepositoryMock.Setup(repo => repo.GetAccount(request.Origin)).Returns((Account)null);

            var result = _accountUseCase.HandleWithdraw(request);

            Assert.Null(result);
        }

        [Fact]
        public void HandleWithdraw_ShouldReturnNull_WhenInsufficientBalance()
        {
            var existingAccount = new Account() { Id = "100", Balance = 5 };
            var request = new RequestEventJson { Origin = "200", Amount = 10, Type = "withdraw" };
            _accountRepositoryMock.Setup(repo => repo.GetAccount(request.Origin)).Returns(existingAccount);

            var result = _accountUseCase.HandleWithdraw(request);

            Assert.Null(result);
        }

        [Fact]
        public void HandleTransfer_ShouldReturnNull_WhenOriginAccountDoesNotExist()
        {
            var request = new RequestEventJson { Origin = "100", Destination = "300", Amount = 15, Type = "transfer" };
            _accountRepositoryMock.Setup(repo => repo.GetAccount(request.Origin)).Returns((Account)null);

            var result = _accountUseCase.HandleTransfer(request);

            Assert.Null(result);
        }

        [Fact]
        public void HandleTransfer_ShouldReturnNull_WhenInsufficientBalanceInOriginAccount()
        {
            var originAccount = new Account() { Id = "100", Balance = 10 };
            var request = new RequestEventJson { Origin = "100", Destination = "300", Amount = 15, Type = "transfer" };
            _accountRepositoryMock.Setup(repo => repo.GetAccount(request.Origin)).Returns(originAccount);

            var result = _accountUseCase.HandleTransfer(request);

            Assert.Null(result);
        }

    }
}