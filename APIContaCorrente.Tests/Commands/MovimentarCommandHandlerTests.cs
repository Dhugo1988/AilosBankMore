using Xunit;
using APIContaCorrente.Application.Commands.Movimentar;
using APIContaCorrente.Domain.Entities;
using APIContaCorrente.Domain.Repositories;
using APIContaCorrente.Application.Services;
using APIContaCorrente.Application.Common.Validators;
using APIContaCorrente.Tests.Common;
using Moq;
using Microsoft.Extensions.Logging;

namespace APIContaCorrente.Tests.Commands
{
    public class MovimentarCommandHandlerTests
    {
        private readonly Mock<IContaCorrenteRepository> _mockContaCorrenteRepository;
        private readonly Mock<IMovimentoRepository> _mockMovimentoRepository;
        private readonly Mock<IIdempotenciaRepository> _mockIdempotenciaRepository;
        private readonly Mock<IValidationService> _mockValidationService;
        private readonly Mock<ILogger<MovimentarCommandHandler>> _mockLogger;

        public MovimentarCommandHandlerTests()
        {
            _mockContaCorrenteRepository = new Mock<IContaCorrenteRepository>();
            _mockMovimentoRepository = new Mock<IMovimentoRepository>();
            _mockIdempotenciaRepository = new Mock<IIdempotenciaRepository>();
            _mockValidationService = new Mock<IValidationService>();
            _mockLogger = new Mock<ILogger<MovimentarCommandHandler>>();
        }

        [Fact]
        public async Task Handle_QuandoCreditoValido_DeveCreditarContaComSucesso()
        {
            // Arrange
            var handler = new MovimentarCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockIdempotenciaRepository.Object,
                _mockValidationService.Object,
                _mockLogger.Object);

            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Numero = TestConstants.TEST_ACCOUNT_NUMBER,
                Nome = TestConstants.TEST_NAME,
                Ativo = true
            };

            var request = new MovimentarCommand
            {
                IdRequisicao = "mov-test-001",
                Valor = TestConstants.TEST_VALID_VALUE,
                TipoMovimento = TestConstants.TEST_CREDIT_TYPE,
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID,
                NumeroConta = TestConstants.TEST_ACCOUNT_NUMBER
            };

            _mockIdempotenciaRepository
                .Setup(x => x.ExistsAsync(request.IdRequisicao))
                .ReturnsAsync(false);

            _mockValidationService
                .Setup(x => x.ValidateAccountByNumeroConta(request.NumeroConta))
                .ReturnsAsync(AccountValidationResult.Valid(contaCorrente));

            _mockContaCorrenteRepository
                .Setup(x => x.GetByIdAsync(request.ContaCorrenteId))
                .ReturnsAsync(contaCorrente);

            _mockMovimentoRepository
                .Setup(x => x.AddAsync(It.IsAny<Movimento>()))
                .ReturnsAsync(new Movimento());

            _mockIdempotenciaRepository
                .Setup(x => x.AddAsync(It.IsAny<Idempotencia>()))
                .ReturnsAsync(new Idempotencia());

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _mockMovimentoRepository.Verify(x => x.AddAsync(It.IsAny<Movimento>()), Times.Once);
            _mockIdempotenciaRepository.Verify(x => x.AddAsync(It.IsAny<Idempotencia>()), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoDebitoValido_DeveDebitarContaComSucesso()
        {
            // Arrange
            var handler = new MovimentarCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockIdempotenciaRepository.Object,
                _mockValidationService.Object,
                _mockLogger.Object);

            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Numero = TestConstants.TEST_ACCOUNT_NUMBER,
                Nome = TestConstants.TEST_NAME,
                Ativo = true
            };

            var request = new MovimentarCommand
            {
                IdRequisicao = "mov-test-002",
                Valor = 50,
                TipoMovimento = TestConstants.TEST_DEBIT_TYPE,
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID,
                NumeroConta = TestConstants.TEST_ACCOUNT_NUMBER
            };

            _mockIdempotenciaRepository
                .Setup(x => x.ExistsAsync(request.IdRequisicao))
                .ReturnsAsync(false);

            _mockValidationService
                .Setup(x => x.ValidateAccountByNumeroConta(request.NumeroConta))
                .ReturnsAsync(AccountValidationResult.Valid(contaCorrente));

            _mockContaCorrenteRepository
                .Setup(x => x.GetByIdAsync(request.ContaCorrenteId))
                .ReturnsAsync(contaCorrente);

            _mockMovimentoRepository
                .Setup(x => x.AddAsync(It.IsAny<Movimento>()))
                .ReturnsAsync(new Movimento());

            _mockIdempotenciaRepository
                .Setup(x => x.AddAsync(It.IsAny<Idempotencia>()))
                .ReturnsAsync(new Idempotencia());

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _mockMovimentoRepository.Verify(x => x.AddAsync(It.IsAny<Movimento>()), Times.Once);
            _mockIdempotenciaRepository.Verify(x => x.AddAsync(It.IsAny<Idempotencia>()), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoValorInvalido_DeveRetornarErro()
        {
            // Arrange
            var handler = new MovimentarCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockIdempotenciaRepository.Object,
                _mockValidationService.Object,
                _mockLogger.Object);

            var request = new MovimentarCommand
            {
                IdRequisicao = "mov-test-003",
                Valor = -100,
                TipoMovimento = TestConstants.TEST_CREDIT_TYPE,
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID,
                NumeroConta = TestConstants.TEST_ACCOUNT_NUMBER
            };

            _mockIdempotenciaRepository
                .Setup(x => x.ExistsAsync(request.IdRequisicao))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_VALUE", result.ErrorType);
            _mockMovimentoRepository.Verify(x => x.AddAsync(It.IsAny<Movimento>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoValorZero_DeveRetornarErro()
        {
            // Arrange
            var handler = new MovimentarCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockIdempotenciaRepository.Object,
                _mockValidationService.Object,
                _mockLogger.Object);

            var request = new MovimentarCommand
            {
                IdRequisicao = "mov-test-004",
                Valor = 0,
                TipoMovimento = TestConstants.TEST_CREDIT_TYPE,
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID,
                NumeroConta = TestConstants.TEST_ACCOUNT_NUMBER
            };

            _mockIdempotenciaRepository
                .Setup(x => x.ExistsAsync(request.IdRequisicao))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_VALUE", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoTipoMovimentoInvalido_DeveRetornarErro()
        {
            // Arrange
            var handler = new MovimentarCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockIdempotenciaRepository.Object,
                _mockValidationService.Object,
                _mockLogger.Object);

            var request = new MovimentarCommand
            {
                IdRequisicao = "mov-test-005",
                Valor = TestConstants.TEST_VALID_VALUE,
                TipoMovimento = TestConstants.TEST_INVALID_TYPE,
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID,
                NumeroConta = TestConstants.TEST_ACCOUNT_NUMBER
            };

            _mockIdempotenciaRepository
                .Setup(x => x.ExistsAsync(request.IdRequisicao))
                .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_TYPE", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoContaInativa_DeveRetornarErro()
        {
            // Arrange
            var handler = new MovimentarCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockIdempotenciaRepository.Object,
                _mockValidationService.Object,
                _mockLogger.Object);

            var request = new MovimentarCommand
            {
                IdRequisicao = "mov-test-006",
                Valor = TestConstants.TEST_VALID_VALUE,
                TipoMovimento = TestConstants.TEST_CREDIT_TYPE,
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID,
                NumeroConta = TestConstants.TEST_ACCOUNT_NUMBER
            };

            _mockIdempotenciaRepository
                .Setup(x => x.ExistsAsync(request.IdRequisicao))
                .ReturnsAsync(false);

            _mockValidationService
                .Setup(x => x.ValidateAccountByNumeroConta(request.NumeroConta))
                .ReturnsAsync(AccountValidationResult.Invalid(
                    new MovimentarResponse(false, "Conta inativa", "INACTIVE_ACCOUNT")));

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INACTIVE_ACCOUNT", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoRequisicaoJaProcessada_DeveRetornarSucesso()
        {
            // Arrange
            var handler = new MovimentarCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockIdempotenciaRepository.Object,
                _mockValidationService.Object,
                _mockLogger.Object);

            var request = new MovimentarCommand
            {
                IdRequisicao = "mov-test-007",
                Valor = TestConstants.TEST_VALID_VALUE,
                TipoMovimento = TestConstants.TEST_CREDIT_TYPE,
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID,
                NumeroConta = TestConstants.TEST_ACCOUNT_NUMBER
            };

            _mockIdempotenciaRepository
                .Setup(x => x.ExistsAsync(request.IdRequisicao))
                .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            _mockValidationService.Verify(x => x.ValidateAccountByNumeroConta(It.IsAny<int>()), Times.Never);
            _mockMovimentoRepository.Verify(x => x.AddAsync(It.IsAny<Movimento>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoTentativaDebitoContaDiferente_DeveRetornarErro()
        {
            // Arrange
            var handler = new MovimentarCommandHandler(
                _mockContaCorrenteRepository.Object,
                _mockMovimentoRepository.Object,
                _mockIdempotenciaRepository.Object,
                _mockValidationService.Object,
                _mockLogger.Object);

            var contaUsuario = new ContaCorrente
            {
                IdContaCorrente = TestConstants.TEST_ACCOUNT_ID,
                Numero = TestConstants.TEST_ACCOUNT_NUMBER,
                Nome = TestConstants.TEST_NAME,
                Ativo = true
            };

            var request = new MovimentarCommand
            {
                IdRequisicao = "mov-test-008",
                Valor = TestConstants.TEST_VALID_VALUE,
                TipoMovimento = TestConstants.TEST_DEBIT_TYPE,
                ContaCorrenteId = TestConstants.TEST_ACCOUNT_ID_2,
                NumeroConta = TestConstants.TEST_ACCOUNT_NUMBER_2 // Conta diferente
            };

            _mockIdempotenciaRepository
                .Setup(x => x.ExistsAsync(request.IdRequisicao))
                .ReturnsAsync(false);

            _mockContaCorrenteRepository
                .Setup(x => x.GetByIdAsync(request.ContaCorrenteId))
                .ReturnsAsync(contaUsuario);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_TYPE", result.ErrorType);
        }
    }
}
