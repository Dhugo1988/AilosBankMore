using Xunit;
using APIContaCorrente.Application.Commands.CadastrarContaCorrente;
using APIContaCorrente.Domain.Entities;
using APIContaCorrente.Domain.Repositories;
using APIContaCorrente.Domain.Services;
using APIContaCorrente.Infrastructure.Security;
using APIContaCorrente.Tests.Common;
using Moq;

namespace APIContaCorrente.Tests.Commands
{
    public class CadastrarContaCorrenteCommandHandlerTests
    {
        private readonly Mock<IContaCorrenteRepository> _mockContaCorrenteRepository;
        private readonly Mock<ICpfValidationService> _mockCpfValidationService;
        private readonly Mock<IContaCorrenteService> _mockContaCorrenteService;
        private readonly Mock<IPasswordHasherService> _mockPasswordHasherService;

        public CadastrarContaCorrenteCommandHandlerTests()
        {
            _mockContaCorrenteRepository = new Mock<IContaCorrenteRepository>();
            _mockCpfValidationService = new Mock<ICpfValidationService>();
            _mockContaCorrenteService = new Mock<IContaCorrenteService>();
            _mockPasswordHasherService = new Mock<IPasswordHasherService>();
        }

        [Fact]
        public async Task Handle_QuandoDadosValidos_DeveCriarContaComSucesso()
        {
            // Arrange
            var handler = new CadastrarContaCorrenteCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockCpfValidationService.Object,
                _mockContaCorrenteService.Object,
                _mockPasswordHasherService.Object);

            var command = new CadastrarContaCorrenteCommand
            {
                Cpf = TestConstants.TEST_CPF_VALID,
                Nome = TestConstants.TEST_NAME,
                Senha = TestConstants.TEST_PASSWORD
            };

            _mockCpfValidationService
                .Setup(x => x.IsValid(command.Cpf))
                .Returns(true);

            _mockContaCorrenteRepository
                .Setup(x => x.ExistsByCpfAsync(command.Cpf))
                .ReturnsAsync(false);

            _mockContaCorrenteService
                .Setup(x => x.GerarNumeroConta())
                .Returns(TestConstants.TEST_ACCOUNT_NUMBER);

            _mockPasswordHasherService
                .Setup(x => x.HashPassword(It.IsAny<string>(), out It.Ref<string>.IsAny))
                .Returns("hashedPassword");

            _mockContaCorrenteRepository
                .Setup(x => x.AddAsync(It.IsAny<ContaCorrente>()))
                .ReturnsAsync(new ContaCorrente());

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(TestConstants.TEST_ACCOUNT_NUMBER, result.NumeroConta);
            _mockContaCorrenteRepository.Verify(x => x.AddAsync(It.IsAny<ContaCorrente>()), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoCpfInvalido_DeveRetornarErro()
        {
            // Arrange
            var handler = new CadastrarContaCorrenteCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockCpfValidationService.Object,
                _mockContaCorrenteService.Object,
                _mockPasswordHasherService.Object);

            var command = new CadastrarContaCorrenteCommand
            {
                Cpf = TestConstants.TEST_CPF_INVALID,
                Nome = TestConstants.TEST_NAME,
                Senha = TestConstants.TEST_PASSWORD
            };

            _mockCpfValidationService
                .Setup(x => x.IsValid(command.Cpf))
                .Returns(false);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_DOCUMENT", result.ErrorType);
            _mockContaCorrenteRepository.Verify(x => x.AddAsync(It.IsAny<ContaCorrente>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoCpfJaExiste_DeveRetornarErro()
        {
            // Arrange
            var handler = new CadastrarContaCorrenteCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockCpfValidationService.Object,
                _mockContaCorrenteService.Object,
                _mockPasswordHasherService.Object);

            var command = new CadastrarContaCorrenteCommand
            {
                Cpf = TestConstants.TEST_CPF_VALID,
                Nome = TestConstants.TEST_NAME,
                Senha = TestConstants.TEST_PASSWORD
            };

            _mockCpfValidationService
                .Setup(x => x.IsValid(command.Cpf))
                .Returns(true);

            _mockContaCorrenteRepository
                .Setup(x => x.ExistsByCpfAsync(command.Cpf))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_DOCUMENT", result.ErrorType);
            _mockContaCorrenteRepository.Verify(x => x.AddAsync(It.IsAny<ContaCorrente>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoNomeVazio_DeveRetornarErro()
        {
            // Arrange
            var handler = new CadastrarContaCorrenteCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockCpfValidationService.Object,
                _mockContaCorrenteService.Object,
                _mockPasswordHasherService.Object);

            var command = new CadastrarContaCorrenteCommand
            {
                Cpf = TestConstants.TEST_CPF_VALID,
                Nome = "",
                Senha = TestConstants.TEST_PASSWORD
            };

            _mockCpfValidationService
                .Setup(x => x.IsValid(command.Cpf))
                .Returns(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_DOCUMENT", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoSenhaVazia_DeveRetornarErro()
        {
            // Arrange
            var handler = new CadastrarContaCorrenteCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockCpfValidationService.Object,
                _mockContaCorrenteService.Object,
                _mockPasswordHasherService.Object);

            var command = new CadastrarContaCorrenteCommand
            {
                Cpf = TestConstants.TEST_CPF_VALID,
                Nome = TestConstants.TEST_NAME,
                Senha = ""
            };

            _mockCpfValidationService
                .Setup(x => x.IsValid(command.Cpf))
                .Returns(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_DOCUMENT", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoCpfNulo_DeveRetornarErro()
        {
            // Arrange
            var handler = new CadastrarContaCorrenteCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockCpfValidationService.Object,
                _mockContaCorrenteService.Object,
                _mockPasswordHasherService.Object);

            var command = new CadastrarContaCorrenteCommand
            {
                Cpf = null,
                Nome = TestConstants.TEST_NAME,
                Senha = TestConstants.TEST_PASSWORD
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_DOCUMENT", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoNomeNulo_DeveRetornarErro()
        {
            // Arrange
            var handler = new CadastrarContaCorrenteCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockCpfValidationService.Object,
                _mockContaCorrenteService.Object,
                _mockPasswordHasherService.Object);

            var command = new CadastrarContaCorrenteCommand
            {
                Cpf = TestConstants.TEST_CPF_VALID,
                Nome = null,
                Senha = TestConstants.TEST_PASSWORD
            };

            _mockCpfValidationService
                .Setup(x => x.IsValid(command.Cpf))
                .Returns(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_DOCUMENT", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoSenhaNula_DeveRetornarErro()
        {
            // Arrange
            var handler = new CadastrarContaCorrenteCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockCpfValidationService.Object,
                _mockContaCorrenteService.Object,
                _mockPasswordHasherService.Object);

            var command = new CadastrarContaCorrenteCommand
            {
                Cpf = TestConstants.TEST_CPF_VALID,
                Nome = TestConstants.TEST_NAME,
                Senha = null
            };

            _mockCpfValidationService
                .Setup(x => x.IsValid(command.Cpf))
                .Returns(true);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_DOCUMENT", result.ErrorType);
        }
    }
}
