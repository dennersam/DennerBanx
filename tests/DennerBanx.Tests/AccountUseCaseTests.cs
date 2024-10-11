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
            // Arrange
            var accountId = "100";
            var account = new Account() { Id = "100", Balance = 50 };
            _accountRepositoryMock.Setup(repo => repo.GetAccount(accountId)).Returns(account);

            // Act
            var result = _accountUseCase.GetBalance(accountId);

            // Assert
            Assert.Equal(50, result);
        }

        [Fact]
        public void GetBalance_ShouldReturnNull_WhenAccountDoesNotExist()
        {
            // Arrange
            var accountId = "1234";
            _accountRepositoryMock.Setup(repo => repo.GetAccount(accountId)).Returns((Account)null);

            // Act
            var result = _accountUseCase.GetBalance(accountId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void HandleWithdraw_ShouldReturnNull_WhenAccountDoesNotExist()
        {
            // Arrange
            var request = new RequestEventJson { Origin = "200", Amount = 10, Type = "withdraw" };
            _accountRepositoryMock.Setup(repo => repo.GetAccount(request.Origin)).Returns((Account)null);

            // Act
            var result = _accountUseCase.HandleWithdraw(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void HandleWithdraw_ShouldReturnNull_WhenInsufficientBalance()
        {
            // Arrange
            var existingAccount = new Account() { Id = "100", Balance = 5 };
            var request = new RequestEventJson { Origin = "200", Amount = 10, Type = "withdraw" };
            _accountRepositoryMock.Setup(repo => repo.GetAccount(request.Origin)).Returns(existingAccount);

            // Act
            var result = _accountUseCase.HandleWithdraw(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void HandleTransfer_ShouldReturnNull_WhenOriginAccountDoesNotExist()
        {
            // Arrange
            var request = new RequestEventJson { Origin = "100", Destination = "300", Amount = 15, Type = "transfer" };
            _accountRepositoryMock.Setup(repo => repo.GetAccount(request.Origin)).Returns((Account)null);

            // Act
            var result = _accountUseCase.HandleTransfer(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void HandleTransfer_ShouldReturnNull_WhenInsufficientBalanceInOriginAccount()
        {
            // Arrange
            var originAccount = new Account() { Id = "100", Balance = 10 };
            var request = new RequestEventJson { Origin = "100", Destination = "300", Amount = 15, Type = "transfer" };
            _accountRepositoryMock.Setup(repo => repo.GetAccount(request.Origin)).Returns(originAccount);

            // Act
            var result = _accountUseCase.HandleTransfer(request);

            // Assert
            Assert.Null(result);
        }

    }
}