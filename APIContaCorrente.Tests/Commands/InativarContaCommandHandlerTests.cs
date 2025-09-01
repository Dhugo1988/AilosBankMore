using Xunit;
using APIContaCorrente.Application.Commands.InativarConta;
using APIContaCorrente.Domain.Entities;
using APIContaCorrente.Domain.Repositories;
using APIContaCorrente.Domain.Services;
using APIContaCorrente.Application.Services;
using APIContaCorrente.Application.Common.Validators;
using APIContaCorrente.Tests.Common;
using Moq;

namespace APIContaCorrente.Tests.Commands
{
    public class InativarContaCommandHandlerTests
    {
        private readonly Mock<IContaCorrenteRepository> _mockContaCorrenteRepository;
        private readonly Mock<IPasswordHasherService> _mockPasswordHasherService;
        private readonly Mock<IValidationService> _mockValidationService;

        public InativarContaCommandHandlerTests()
        {
            _mockContaCorrenteRepository = new Mock<IContaCorrenteRepository>();
            _mockPasswordHasherService = new Mock<IPasswordHasherService>();
            _mockValidationService = new Mock<IValidationService>();
        }

        [Fact]
        public async Task Handle_QuandoCredenciaisValidas_DeveInativarContaComSucesso()
        {
            // Arrange
            var handler = new InativarContaCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _mockValidationService.Object);

            var command = new InativarContaCommand
            {
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID,
                Senha = TestConstants.TEST_PASSWORD
            };

            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Nome = TestConstants.TEST_NAME,
                Senha = "hashedPassword",
                Salt = "salt123",
                Ativo = true
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(command.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Valid(contaCorrente));

            _mockPasswordHasherService
                .Setup(x => x.VerifyPassword(command.Senha, contaCorrente.Senha, contaCorrente.Salt))
                .Returns(true);

            _mockContaCorrenteRepository
                .Setup(x => x.UpdateAsync(It.IsAny<ContaCorrente>()))
                .ReturnsAsync(contaCorrente);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _mockContaCorrenteRepository.Verify(x => x.UpdateAsync(It.Is<ContaCorrente>(c => !c.Ativo)), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoContaJaInativa_DeveRetornarSucesso()
        {
            // Arrange
            var handler = new InativarContaCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _mockValidationService.Object);

            var command = new InativarContaCommand
            {
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID,
                Senha = TestConstants.TEST_PASSWORD
            };

            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Nome = TestConstants.TEST_NAME,
                Senha = "hashedPassword",
                Salt = "salt123",
                Ativo = false
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(command.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Valid(contaCorrente));

            _mockPasswordHasherService
                .Setup(x => x.VerifyPassword(command.Senha, contaCorrente.Senha, contaCorrente.Salt))
                .Returns(true);

            _mockContaCorrenteRepository
                .Setup(x => x.UpdateAsync(It.IsAny<ContaCorrente>()))
                .ReturnsAsync(contaCorrente);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _mockContaCorrenteRepository.Verify(x => x.UpdateAsync(It.IsAny<ContaCorrente>()), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoSenhaIncorreta_DeveRetornarErro()
        {
            // Arrange
            var handler = new InativarContaCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _mockValidationService.Object);

            var command = new InativarContaCommand
            {
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID,
                Senha = "senha-errada"
            };

            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Nome = TestConstants.TEST_NAME,
                Senha = "hashedPassword",
                Salt = "salt123",
                Ativo = true
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(command.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Valid(contaCorrente));

            _mockPasswordHasherService
                .Setup(x => x.VerifyPassword(command.Senha, contaCorrente.Senha, contaCorrente.Salt))
                .Returns(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("USER_UNAUTHORIZED", result.ErrorType);
            _mockContaCorrenteRepository.Verify(x => x.UpdateAsync(It.IsAny<ContaCorrente>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoContaNaoEncontrada_DeveRetornarErro()
        {
            // Arrange
            var handler = new InativarContaCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _mockValidationService.Object);

            var command = new InativarContaCommand
            {
                ContaCorrenteId = "conta-inexistente",
                Senha = TestConstants.TEST_PASSWORD
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(command.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Invalid(
                    new InativarContaResponse(false, "Conta não encontrada", "ACCOUNT_NOT_FOUND")));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("ACCOUNT_NOT_FOUND", result.ErrorType);
            _mockPasswordHasherService.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoContaInativa_DeveRetornarErro()
        {
            // Arrange
            var handler = new InativarContaCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _mockValidationService.Object);

            var command = new InativarContaCommand
            {
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID,
                Senha = TestConstants.TEST_PASSWORD
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(command.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Invalid(
                    new InativarContaResponse(false, "Conta inativa", "INACTIVE_ACCOUNT")));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INACTIVE_ACCOUNT", result.ErrorType);
            _mockPasswordHasherService.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoContaCorrenteIdVazio_DeveRetornarErro()
        {
            // Arrange
            var handler = new InativarContaCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _mockValidationService.Object);

            var command = new InativarContaCommand
            {
                ContaCorrenteId = "",
                Senha = TestConstants.TEST_PASSWORD
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(command.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Invalid(
                    new InativarContaResponse(false, "Conta inválida", "INVALID_ACCOUNT")));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_ACCOUNT", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoSenhaVazia_DeveRetornarErro()
        {
            // Arrange
            var handler = new InativarContaCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _mockValidationService.Object);

            var command = new InativarContaCommand
            {
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID,
                Senha = ""
            };

            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Nome = TestConstants.TEST_NAME,
                Senha = "hashedPassword",
                Salt = "salt123",
                Ativo = true
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(command.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Valid(contaCorrente));

            _mockPasswordHasherService
                .Setup(x => x.VerifyPassword(command.Senha, contaCorrente.Senha, contaCorrente.Salt))
                .Returns(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("USER_UNAUTHORIZED", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoContaCorrenteIdNulo_DeveRetornarErro()
        {
            // Arrange
            var handler = new InativarContaCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _mockValidationService.Object);

            var command = new InativarContaCommand
            {
                ContaCorrenteId = null,
                Senha = TestConstants.TEST_PASSWORD
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(command.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Invalid(
                    new InativarContaResponse(false, "Conta inválida", "INVALID_ACCOUNT")));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_ACCOUNT", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoSenhaNula_DeveRetornarErro()
        {
            // Arrange
            var handler = new InativarContaCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _mockValidationService.Object);

            var command = new InativarContaCommand
            {
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID,
                Senha = null
            };

            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Nome = TestConstants.TEST_NAME,
                Senha = "hashedPassword",
                Salt = "salt123",
                Ativo = true
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(command.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Valid(contaCorrente));

            _mockPasswordHasherService
                .Setup(x => x.VerifyPassword(command.Senha, contaCorrente.Senha, contaCorrente.Salt))
                .Returns(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("USER_UNAUTHORIZED", result.ErrorType);
        }
    }
}
