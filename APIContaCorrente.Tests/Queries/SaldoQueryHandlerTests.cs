using Xunit;
using APIContaCorrente.Application.Queries.Saldo;
using APIContaCorrente.Domain.Entities;
using APIContaCorrente.Domain.Repositories;
using APIContaCorrente.Application.Services;
using APIContaCorrente.Application.Common.Validators;
using APIContaCorrente.Tests.Common;
using Moq;

namespace APIContaCorrente.Tests.Queries
{
    public class SaldoQueryHandlerTests
    {
        private readonly Mock<IContaCorrenteRepository> _mockContaCorrenteRepository;
        private readonly Mock<IMovimentoRepository> _mockMovimentoRepository;
        private readonly Mock<IValidationService> _mockValidationService;

        public SaldoQueryHandlerTests()
        {
            _mockContaCorrenteRepository = new Mock<IContaCorrenteRepository>();
            _mockMovimentoRepository = new Mock<IMovimentoRepository>();
            _mockValidationService = new Mock<IValidationService>();
        }

        [Fact]
        public async Task Handle_QuandoContaValida_DeveRetornarSaldoCorreto()
        {
            // Arrange
            var handler = new SaldoQueryHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockValidationService.Object);

            var query = new SaldoQuery
            {
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID
            };

            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Numero = TestConstants.TEST_ACCOUNT_NUMBER,
                Nome = TestConstants.TEST_NAME,
                Ativo = true
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(query.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Valid(contaCorrente));

            _mockMovimentoRepository
                .Setup(x => x.GetSaldoAsync(query.ContaCorrenteId))
                .ReturnsAsync(150);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(150, result.Saldo);
            Assert.Equal(TestConstants.TEST_ACCOUNT_NUMBER, result.NumeroConta);
            Assert.Equal(TestConstants.TEST_NAME, result.NomeTitular);
            Assert.NotNull(result.Timestamp);
        }

        [Fact]
        public async Task Handle_QuandoContaInvalida_DeveRetornarErro()
        {
            // Arrange
            var handler = new SaldoQueryHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockValidationService.Object);

            var query = new SaldoQuery
            {
                ContaCorrenteId = "conta-inexistente"
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(query.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Invalid(
                    new SaldoResponse(false, "Conta não encontrada", "ACCOUNT_NOT_FOUND")));

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("ACCOUNT_NOT_FOUND", result.ErrorType);
            _mockMovimentoRepository.Verify(x => x.GetSaldoAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoContaInativa_DeveRetornarErro()
        {
            // Arrange
            var handler = new SaldoQueryHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockValidationService.Object);

            var query = new SaldoQuery
            {
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(query.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Invalid(
                    new SaldoResponse(false, "Conta inativa", "INACTIVE_ACCOUNT")));

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INACTIVE_ACCOUNT", result.ErrorType);
            _mockMovimentoRepository.Verify(x => x.GetSaldoAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoSaldoZero_DeveRetornarSucesso()
        {
            // Arrange
            var handler = new SaldoQueryHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockValidationService.Object);

            var query = new SaldoQuery
            {
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID
            };

            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Numero = TestConstants.TEST_ACCOUNT_NUMBER,
                Nome = TestConstants.TEST_NAME,
                Ativo = true
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(query.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Valid(contaCorrente));

            _mockMovimentoRepository
                .Setup(x => x.GetSaldoAsync(query.ContaCorrenteId))
                .ReturnsAsync(0);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(0, result.Saldo);
            Assert.Equal(TestConstants.TEST_ACCOUNT_NUMBER, result.NumeroConta);
            Assert.Equal(TestConstants.TEST_NAME, result.NomeTitular);
        }

        [Fact]
        public async Task Handle_QuandoSaldoNegativo_DeveRetornarSucesso()
        {
            // Arrange
            var handler = new SaldoQueryHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockValidationService.Object);

            var query = new SaldoQuery
            {
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID
            };

            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Numero = TestConstants.TEST_ACCOUNT_NUMBER,
                Nome = TestConstants.TEST_NAME,
                Ativo = true
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(query.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Valid(contaCorrente));

            _mockMovimentoRepository
                .Setup(x => x.GetSaldoAsync(query.ContaCorrenteId))
                .ReturnsAsync(-50);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(-50, result.Saldo);
            Assert.Equal(TestConstants.TEST_ACCOUNT_NUMBER, result.NumeroConta);
            Assert.Equal(TestConstants.TEST_NAME, result.NomeTitular);
        }

        [Fact]
        public async Task Handle_QuandoContaCorrenteIdVazio_DeveRetornarErro()
        {
            // Arrange
            var handler = new SaldoQueryHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockValidationService.Object);

            var query = new SaldoQuery
            {
                ContaCorrenteId = ""
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(query.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Invalid(
                    new SaldoResponse(false, "Conta inválida", "INVALID_ACCOUNT")));

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_ACCOUNT", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoContaCorrenteIdNulo_DeveRetornarErro()
        {
            // Arrange
            var handler = new SaldoQueryHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockValidationService.Object);

            var query = new SaldoQuery
            {
                ContaCorrenteId = null
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(query.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Invalid(
                    new SaldoResponse(false, "Conta inválida", "INVALID_ACCOUNT")));

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_ACCOUNT", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoContaComNomeVazio_DeveRetornarSucesso()
        {
            // Arrange
            var handler = new SaldoQueryHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockValidationService.Object);

            var query = new SaldoQuery
            {
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID
            };

            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Numero = TestConstants.TEST_ACCOUNT_NUMBER,
                Nome = "",
                Ativo = true
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(query.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Valid(contaCorrente));

            _mockMovimentoRepository
                .Setup(x => x.GetSaldoAsync(query.ContaCorrenteId))
                .ReturnsAsync(100);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(100, result.Saldo);
            Assert.Equal(TestConstants.TEST_ACCOUNT_NUMBER, result.NumeroConta);
            Assert.Equal("", result.NomeTitular);
        }

        [Fact]
        public async Task Handle_QuandoContaComNomeNulo_DeveRetornarSucesso()
        {
            // Arrange
            var handler = new SaldoQueryHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockValidationService.Object);

            var query = new SaldoQuery
            {
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID
            };

            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Numero = TestConstants.TEST_ACCOUNT_NUMBER,
                Nome = null,
                Ativo = true
            };

            _mockValidationService
                .Setup(x => x.ValidateAccountByIdConta(query.ContaCorrenteId))
                .ReturnsAsync(AccountValidationResult.Valid(contaCorrente));

            _mockMovimentoRepository
                .Setup(x => x.GetSaldoAsync(query.ContaCorrenteId))
                .ReturnsAsync(200);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.Saldo);
            Assert.Equal(TestConstants.TEST_ACCOUNT_NUMBER, result.NumeroConta);
            Assert.Null(result.NomeTitular);
        }
    }
}
