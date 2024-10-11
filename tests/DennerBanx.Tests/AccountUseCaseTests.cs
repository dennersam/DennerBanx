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
        public void HandleDeposit_ShouldCreateNewAccount_WhenAccountDoesNotExist()
        {
            // Arrange
            var request = new RequestEventJson { Destination = "100", Amount = 10, Type = "deposit" };
            _accountRepositoryMock.Setup(repo => repo.GetAccount(request.Destination)).Returns((Account)null);

            // Act
            var result = _accountUseCase.HandleDeposit(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("100", ((dynamic)result).destination.id);
            Assert.Equal(10, ((dynamic)result).destination.balance);
            _accountRepositoryMock.Verify(repo => repo.CreateAccount("100", 10), Times.Once);
        }

        [Fact]
        public void HandleDeposit_ShouldUpdateBalance_WhenAccountExists()
        {
            // Arrange
            var existingAccount = new Account() { Id= "100", Balance = 20 };
            var request = new RequestEventJson { Destination = "100", Amount = 10, Type = "deposit" };
            _accountRepositoryMock.Setup(repo => repo.GetAccount(request.Destination)).Returns(existingAccount);

            // Act
            var result = _accountUseCase.HandleDeposit(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("100", ((dynamic)result).destination.id);
            Assert.Equal(30, ((dynamic)result).destination.balance);
            _accountRepositoryMock.Verify(repo => repo.UpdateAccountBalance("100", 30), Times.Once);
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
        public void HandleWithdraw_ShouldUpdateBalance_WhenSufficientBalance()
        {
            // Arrange
            var existingAccount = new Account() { Id = "100", Balance = 20 };
            var request = new RequestEventJson { Origin = "100", Amount = 5, Type = "withdraw" };
            _accountRepositoryMock.Setup(repo => repo.GetAccount(request.Origin)).Returns(existingAccount);

            // Act
            var result = _accountUseCase.HandleWithdraw(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("100", ((dynamic)result).origin.id);
            Assert.Equal(15, ((dynamic)result).origin.balance);
            _accountRepositoryMock.Verify(repo => repo.UpdateAccountBalance("100", 15), Times.Once);
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

        [Fact]
        public void HandleTransfer_ShouldTransferFunds_WhenAccountsExistAndSufficientBalance()
        {
            // Arrange
            var originAccount = new Account() { Id = "100", Balance = 20 };
            var destinationAccount = new Account() { Id = "100", Balance = 0 };
            var request = new RequestEventJson { Origin = "100", Destination = "300", Amount = 15, Type = "transfer" };

            _accountRepositoryMock.Setup(repo => repo.GetAccount(request.Origin)).Returns(originAccount);
            _accountRepositoryMock.Setup(repo => repo.GetAccount(request.Destination)).Returns(destinationAccount);

            // Act
            var result = _accountUseCase.HandleTransfer(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("100", ((dynamic)result).origin.id);
            Assert.Equal(5, ((dynamic)result).origin.balance);
            Assert.Equal("300", ((dynamic)result).destination.id);
            Assert.Equal(15, ((dynamic)result).destination.balance);
            _accountRepositoryMock.Verify(repo => repo.UpdateAccountBalance("100", 5), Times.Once);
            _accountRepositoryMock.Verify(repo => repo.UpdateAccountBalance("300", 15), Times.Once);
        }
    }
}