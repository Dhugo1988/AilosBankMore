using Xunit;
using APITransferencia.Application.Commands.EfetuarTransferencia;
using APITransferencia.Domain.Entities;
using APITransferencia.Infrastructure.Data;
using APITransferencia.Infrastructure.Messaging.Producers;
using APITransferencia.Infrastructure.Messaging.Messages;
using Moq;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace APITransferencia.Tests.Commands
{
    public class EfetuarTransferenciaCommandHandlerTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ITransferenciaDbContext> _mockDbContext;
        private readonly Mock<ITransferenciaProducer> _mockTransferenciaProducer;
        private readonly Mock<ILogger<EfetuarTransferenciaCommandHandler>> _mockLogger;

        public EfetuarTransferenciaCommandHandlerTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockDbContext = new Mock<ITransferenciaDbContext>();
            _mockTransferenciaProducer = new Mock<ITransferenciaProducer>();
            _mockLogger = new Mock<ILogger<EfetuarTransferenciaCommandHandler>>();

            // Setup do DbContext
            var mockTransferenciasDbSet = new Mock<DbSet<Transferencia>>();
            var mockIdempotenciasDbSet = new Mock<DbSet<Idempotencia>>();

            _mockDbContext.Setup(x => x.Transferencias).Returns(mockTransferenciasDbSet.Object);
            _mockDbContext.Setup(x => x.Idempotencias).Returns(mockIdempotenciasDbSet.Object);
            _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        }

        [Fact]
        public async Task Handle_QuandoValorZero_DeveRetornarErro()
        {
            // Arrange
            var handler = new EfetuarTransferenciaCommandHandler(
                _mockHttpClientFactory.Object,
                _mockDbContext.Object,
                _mockTransferenciaProducer.Object,
                _mockLogger.Object);

            var command = new EfetuarTransferenciaCommand
            {
                IdRequisicao = "transfer-test-002",
                ContaCorrenteIdOrigem = "12345",
                NumeroContaDestino = 67890,
                Valor = 0,
                BearerToken = "Bearer token123"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("BAD_REQUEST", result.ErrorType);
            Assert.Equal("Valor deve ser maior que zero", result.Message);
        }

        [Fact]
        public async Task Handle_QuandoValorNegativo_DeveRetornarErro()
        {
            // Arrange
            var handler = new EfetuarTransferenciaCommandHandler(
                _mockHttpClientFactory.Object,
                _mockDbContext.Object,
                _mockTransferenciaProducer.Object,
                _mockLogger.Object);

            var command = new EfetuarTransferenciaCommand
            {
                IdRequisicao = "transfer-test-003",
                ContaCorrenteIdOrigem = "12345",
                NumeroContaDestino = 67890,
                Valor = -100,
                BearerToken = "Bearer token123"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("BAD_REQUEST", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoContaOrigemVazia_DeveRetornarErro()
        {
            // Arrange
            var handler = new EfetuarTransferenciaCommandHandler(
                _mockHttpClientFactory.Object,
                _mockDbContext.Object,
                _mockTransferenciaProducer.Object,
                _mockLogger.Object);

            var command = new EfetuarTransferenciaCommand
            {
                IdRequisicao = "transfer-test-004",
                ContaCorrenteIdOrigem = "",
                NumeroContaDestino = 67890,
                Valor = 100,
                BearerToken = "Bearer token123"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("BAD_REQUEST", result.ErrorType);
        }

        [Fact]
        public async Task Handle_QuandoContaDestinoZero_DeveRetornarErro()
        {
            // Arrange
            var handler = new EfetuarTransferenciaCommandHandler(
                _mockHttpClientFactory.Object,
                _mockDbContext.Object,
                _mockTransferenciaProducer.Object,
                _mockLogger.Object);

            var command = new EfetuarTransferenciaCommand
            {
                IdRequisicao = "transfer-test-005",
                ContaCorrenteIdOrigem = "12345",
                NumeroContaDestino = 0,
                Valor = 100,
                BearerToken = "Bearer token123"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("BAD_REQUEST", result.ErrorType);
        }
    }
}
