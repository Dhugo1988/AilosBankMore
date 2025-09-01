using Xunit;
using APITarifa.Application.Commands.ConsultarTarifasPorConta;
using APITarifa.Application.DTOs;
using APITarifa.Domain.Entities;
using APITarifa.Domain.Repositories;
using Moq;
using Microsoft.Extensions.Logging;

namespace APITarifa.Tests.Commands
{
    public class ConsultarTarifasPorContaCommandHandlerTests
    {
        private readonly Mock<ITarifaRepository> _mockTarifaRepository;
        private readonly Mock<ILogger<ConsultarTarifasPorContaCommandHandler>> _mockLogger;

        public ConsultarTarifasPorContaCommandHandlerTests()
        {
            _mockTarifaRepository = new Mock<ITarifaRepository>();
            _mockLogger = new Mock<ILogger<ConsultarTarifasPorContaCommandHandler>>();
        }

        [Fact]
        public async Task Handle_QuandoContaValida_DeveRetornarTarifas()
        {
            // Arrange
            var handler = new ConsultarTarifasPorContaCommandHandler(_mockTarifaRepository.Object, _mockLogger.Object);

            var command = new ConsultarTarifasPorContaCommand
            {
                IdContaCorrente = "conta-123"
            };

            var tarifas = new List<Tarifa>
            {
                new Tarifa
                {
                    IdTarifa = "tarifa-1",
                    IdContaCorrente = "conta-123",
                    DataMovimento = "25/12/2024 10:00:00",
                    Valor = 5.50m
                },
                new Tarifa
                {
                    IdTarifa = "tarifa-2",
                    IdContaCorrente = "conta-123",
                    DataMovimento = "25/12/2024 15:30:00",
                    Valor = 3.25m
                }
            };

            _mockTarifaRepository
                .Setup(x => x.GetByContaCorrenteAsync(command.IdContaCorrente))
                .ReturnsAsync(tarifas);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(command.IdContaCorrente, result.Data.IdContaCorrente);
            Assert.Equal(2, result.Data.TotalTarifas);
            Assert.Equal(8.75m, result.Data.ValorTotalTarifado);
            Assert.Equal(2, result.Data.Tarifas.Count);
            _mockTarifaRepository.Verify(x => x.GetByContaCorrenteAsync(command.IdContaCorrente), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoContaSemTarifas_DeveRetornarListaVazia()
        {
            // Arrange
            var handler = new ConsultarTarifasPorContaCommandHandler(_mockTarifaRepository.Object, _mockLogger.Object);

            var command = new ConsultarTarifasPorContaCommand
            {
                IdContaCorrente = "conta-sem-tarifas"
            };

            var tarifas = new List<Tarifa>();

            _mockTarifaRepository
                .Setup(x => x.GetByContaCorrenteAsync(command.IdContaCorrente))
                .ReturnsAsync(tarifas);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(command.IdContaCorrente, result.Data.IdContaCorrente);
            Assert.Equal(0, result.Data.TotalTarifas);
            Assert.Equal(0, result.Data.ValorTotalTarifado);
            Assert.Empty(result.Data.Tarifas);
            _mockTarifaRepository.Verify(x => x.GetByContaCorrenteAsync(command.IdContaCorrente), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoIdContaVazio_DeveRetornarErro()
        {
            // Arrange
            var handler = new ConsultarTarifasPorContaCommandHandler(_mockTarifaRepository.Object, _mockLogger.Object);

            var command = new ConsultarTarifasPorContaCommand
            {
                IdContaCorrente = ""
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_ACCOUNT_ID", result.ErrorType);
            Assert.Equal("ID da conta inválido", result.Message);
            _mockTarifaRepository.Verify(x => x.GetByContaCorrenteAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoIdContaNulo_DeveRetornarErro()
        {
            // Arrange
            var handler = new ConsultarTarifasPorContaCommandHandler(_mockTarifaRepository.Object, _mockLogger.Object);

            var command = new ConsultarTarifasPorContaCommand
            {
                IdContaCorrente = null
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("INVALID_ACCOUNT_ID", result.ErrorType);
            Assert.Equal("ID da conta inválido", result.Message);
            _mockTarifaRepository.Verify(x => x.GetByContaCorrenteAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
