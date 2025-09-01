using Xunit;
using APITarifa.Application.Commands.ConsultarTarifaPorId;
using APITarifa.Domain.Entities;
using APITarifa.Domain.Repositories;
using Moq;
using Microsoft.Extensions.Logging;

namespace APITarifa.Tests.Commands
{
    public class ConsultarTarifaPorIdCommandHandlerTests
    {
        private readonly Mock<ITarifaRepository> _mockTarifaRepository;
        private readonly Mock<ILogger<ConsultarTarifaPorIdCommandHandler>> _mockLogger;

        public ConsultarTarifaPorIdCommandHandlerTests()
        {
            _mockTarifaRepository = new Mock<ITarifaRepository>();
            _mockLogger = new Mock<ILogger<ConsultarTarifaPorIdCommandHandler>>();
        }

        [Fact]
        public async Task Handle_QuandoIdValido_DeveRetornarTarifa()
        {
            // Arrange
            var handler = new ConsultarTarifaPorIdCommandHandler(_mockTarifaRepository.Object, _mockLogger.Object);

            var command = new ConsultarTarifaPorIdCommand
            {
                IdTarifa = "tarifa-123"
            };

            var tarifa = new Tarifa
            {
                IdTarifa = "tarifa-123",
                IdContaCorrente = "conta-456",
                DataMovimento = "25/12/2024",
                Valor = 2.00m
            };

            _mockTarifaRepository
                .Setup(x => x.GetByIdAsync(command.IdTarifa))
                .ReturnsAsync(tarifa);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(tarifa.IdTarifa, result.Data.IdTarifa);
            Assert.Equal(tarifa.IdContaCorrente, result.Data.IdContaCorrente);
            Assert.Equal(tarifa.Valor, result.Data.Valor);
            _mockTarifaRepository.Verify(x => x.GetByIdAsync(command.IdTarifa), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoIdVazio_DeveRetornarErro()
        {
            // Arrange
            var handler = new ConsultarTarifaPorIdCommandHandler(_mockTarifaRepository.Object, _mockLogger.Object);

            var command = new ConsultarTarifaPorIdCommand
            {
                IdTarifa = ""
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_TARIFA_ID", result.ErrorType);
            Assert.Equal("ID da tarifa inválido", result.Message);
            _mockTarifaRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoIdNulo_DeveRetornarErro()
        {
            // Arrange
            var handler = new ConsultarTarifaPorIdCommandHandler(_mockTarifaRepository.Object, _mockLogger.Object);

            var command = new ConsultarTarifaPorIdCommand
            {
                IdTarifa = null
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_TARIFA_ID", result.ErrorType);
            Assert.Equal("ID da tarifa inválido", result.Message);
            _mockTarifaRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoTarifaNaoEncontrada_DeveRetornarErro()
        {
            // Arrange
            var handler = new ConsultarTarifaPorIdCommandHandler(_mockTarifaRepository.Object, _mockLogger.Object);

            var command = new ConsultarTarifaPorIdCommand
            {
                IdTarifa = "tarifa-inexistente"
            };

            _mockTarifaRepository
                .Setup(x => x.GetByIdAsync(command.IdTarifa))
                .ReturnsAsync((Tarifa)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("TARIFA_NOT_FOUND", result.ErrorType);
            Assert.Equal("Tarifa não encontrada", result.Message);
            _mockTarifaRepository.Verify(x => x.GetByIdAsync(command.IdTarifa), Times.Once);
        }
    }
}
