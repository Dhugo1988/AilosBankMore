using Xunit;
using APIContaCorrente.Application.Commands.Login;
using APIContaCorrente.Domain.Entities;
using APIContaCorrente.Domain.Repositories;
using APIContaCorrente.Domain.Services;
using APIContaCorrente.Infrastructure.Security;
using APIContaCorrente.Tests.Common;
using Moq;

namespace APIContaCorrente.Tests.Commands
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IContaCorrenteRepository> _mockContaCorrenteRepository;
        private readonly Mock<IPasswordHasherService> _mockPasswordHasherService;
        private readonly JwtTokenService _jwtTokenService;

        public LoginCommandHandlerTests()
        {
            _mockContaCorrenteRepository = new Mock<IContaCorrenteRepository>();
            _mockPasswordHasherService = new Mock<IPasswordHasherService>();
            _jwtTokenService = new JwtTokenService("test-key-1234567890123456789012345678901234567890", "test-issuer", "test-audience");
        }

        [Fact]
        public async Task Handle_QuandoCredenciaisValidas_DeveRetornarToken()
        {
            // Arrange
            var handler = new LoginCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _jwtTokenService);

            var command = new LoginCommand
            {
                Identificador = TestConstants.TEST_CPF_VALID,
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

            _mockContaCorrenteRepository
                .Setup(x => x.GetByCpfAsync(command.Identificador))
                .ReturnsAsync(contaCorrente);

            _mockPasswordHasherService
                .Setup(x => x.VerifyPassword(command.Senha, contaCorrente.Senha, contaCorrente.Salt))
                .Returns(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Token);
            Assert.NotEmpty(result.Token);
        }

        [Fact]
        public async Task Handle_QuandoLoginPorNumeroConta_DeveRetornarToken()
        {
            // Arrange
            var handler = new LoginCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _jwtTokenService);

            var command = new LoginCommand
            {
                Identificador = TestConstants.TEST_ACCOUNT_NUMBER.ToString(),
                Senha = TestConstants.TEST_PASSWORD
            };

            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Numero = TestConstants.TEST_ACCOUNT_NUMBER,
                Nome = TestConstants.TEST_NAME,
                Senha = "hashedPassword",
                Salt = "salt123",
                Ativo = true
            };

            _mockContaCorrenteRepository
                .Setup(x => x.GetByCpfAsync(command.Identificador))
                .ReturnsAsync((ContaCorrente)null);

            _mockContaCorrenteRepository
                .Setup(x => x.GetByNumeroAsync(TestConstants.TEST_ACCOUNT_NUMBER))
                .ReturnsAsync(contaCorrente);

            _mockPasswordHasherService
                .Setup(x => x.VerifyPassword(command.Senha, contaCorrente.Senha, contaCorrente.Salt))
                .Returns(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async Task Handle_QuandoContaNaoEncontrada_DeveRetornarErro()
        {
            // Arrange
            var handler = new LoginCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _jwtTokenService);

            var command = new LoginCommand
            {
                Identificador = "cpf-inexistente",
                Senha = TestConstants.TEST_PASSWORD
            };

            _mockContaCorrenteRepository
                .Setup(x => x.GetByCpfAsync(command.Identificador))
                .ReturnsAsync((ContaCorrente)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("USER_UNAUTHORIZED", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoContaInativa_DeveRetornarErro()
        {
            // Arrange
            var handler = new LoginCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _jwtTokenService);

            var command = new LoginCommand
            {
                Identificador = TestConstants.TEST_CPF_VALID,
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

            _mockContaCorrenteRepository
                .Setup(x => x.GetByCpfAsync(command.Identificador))
                .ReturnsAsync(contaCorrente);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("USER_UNAUTHORIZED", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoSenhaIncorreta_DeveRetornarErro()
        {
            // Arrange
            var handler = new LoginCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _jwtTokenService);

            var command = new LoginCommand
            {
                Identificador = TestConstants.TEST_CPF_VALID,
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

            _mockContaCorrenteRepository
                .Setup(x => x.GetByCpfAsync(command.Identificador))
                .ReturnsAsync(contaCorrente);

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
        public async Task Handle_QuandoIdentificadorVazio_DeveRetornarErro()
        {
            // Arrange
            var handler = new LoginCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _jwtTokenService);

            var command = new LoginCommand
            {
                Identificador = "",
                Senha = TestConstants.TEST_PASSWORD
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("USER_UNAUTHORIZED", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoSenhaVazia_DeveRetornarErro()
        {
            // Arrange
            var handler = new LoginCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _jwtTokenService);

            var command = new LoginCommand
            {
                Identificador = TestConstants.TEST_CPF_VALID,
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

            _mockContaCorrenteRepository
                .Setup(x => x.GetByCpfAsync(command.Identificador))
                .ReturnsAsync(contaCorrente);

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
        public async Task Handle_QuandoIdentificadorNulo_DeveRetornarErro()
        {
            // Arrange
            var handler = new LoginCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _jwtTokenService);

            var command = new LoginCommand
            {
                Identificador = null,
                Senha = TestConstants.TEST_PASSWORD
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("USER_UNAUTHORIZED", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoSenhaNula_DeveRetornarErro()
        {
            // Arrange
            var handler = new LoginCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockPasswordHasherService.Object,
                _jwtTokenService);

            var command = new LoginCommand
            {
                Identificador = TestConstants.TEST_CPF_VALID,
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

            _mockContaCorrenteRepository
                .Setup(x => x.GetByCpfAsync(command.Identificador))
                .ReturnsAsync(contaCorrente);

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
