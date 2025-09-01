using Xunit;
using APITarifa.Application.Commands.CadastrarTarifa;
using APITarifa.Domain.Entities;
using APITarifa.Domain.Repositories;
using Moq;
using Microsoft.Extensions.Logging;

namespace APITarifa.Tests.Commands
{
    public class TarifaCommandHandlerTests
    {
        private readonly Mock<ITarifaRepository> _mockTarifaRepository;
        private readonly Mock<ILogger<CadastrarTarifaCommandHandler>> _mockLogger;

        public TarifaCommandHandlerTests()
        {
            _mockTarifaRepository = new Mock<ITarifaRepository>();
            _mockLogger = new Mock<ILogger<CadastrarTarifaCommandHandler>>();
        }

        [Fact]
        public async Task Handle_QuandoDadosValidos_DeveCadastrarTarifaComSucesso()
        {
            // Arrange
            var handler = new CadastrarTarifaCommandHandler(_mockTarifaRepository.Object, _mockLogger.Object);

            var command = new CadastrarTarifaCommand
            {
                IdContaCorrente = "conta-123",
                DataMovimento = "25/12/2024",
                Valor = 5.50m
            };

            var tarifa = new Tarifa
            {
                IdTarifa = "tarifa-456",
                IdContaCorrente = command.IdContaCorrente,
                DataMovimento = command.DataMovimento,
                Valor = command.Valor
            };

            _mockTarifaRepository
                .Setup(x => x.AddAsync(It.IsAny<Tarifa>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.TarifaId);
            Assert.Equal(command.IdContaCorrente, result.IdContaCorrente);
            Assert.Equal(command.DataMovimento, result.DataMovimento);
            Assert.Equal(command.Valor, result.Valor);
            _mockTarifaRepository.Verify(x => x.AddAsync(It.IsAny<Tarifa>()), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoIdContaCorrenteVazio_DeveRetornarErro()
        {
            // Arrange
            var handler = new CadastrarTarifaCommandHandler(_mockTarifaRepository.Object, _mockLogger.Object);

            var command = new CadastrarTarifaCommand
            {
                IdContaCorrente = "",
                DataMovimento = "25/12/2024",
                Valor = 5.50m
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("BAD_REQUEST", result.ErrorType);
            Assert.Equal("ID da conta corrente é obrigatório", result.Message);
            _mockTarifaRepository.Verify(x => x.AddAsync(It.IsAny<Tarifa>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoDataMovimentoVazia_DeveRetornarErro()
        {
            // Arrange
            var handler = new CadastrarTarifaCommandHandler(_mockTarifaRepository.Object, _mockLogger.Object);

            var command = new CadastrarTarifaCommand
            {
                IdContaCorrente = "conta-123",
                DataMovimento = "",
                Valor = 5.50m
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("BAD_REQUEST", result.ErrorType);
            Assert.Equal("Data do movimento é obrigatória", result.Message);
            _mockTarifaRepository.Verify(x => x.AddAsync(It.IsAny<Tarifa>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoValorZero_DeveRetornarErro()
        {
            // Arrange
            var handler = new CadastrarTarifaCommandHandler(_mockTarifaRepository.Object, _mockLogger.Object);

            var command = new CadastrarTarifaCommand
            {
                IdContaCorrente = "conta-123",
                DataMovimento = "25/12/2024",
                Valor = 0
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("BAD_REQUEST", result.ErrorType);
            Assert.Equal("Valor deve ser maior que zero", result.Message);
            _mockTarifaRepository.Verify(x => x.AddAsync(It.IsAny<Tarifa>()), Times.Never);
        }

        [Fact]
        public async Task Handle_QuandoValorNegativo_DeveRetornarErro()
        {
            // Arrange
            var handler = new CadastrarTarifaCommandHandler(_mockTarifaRepository.Object, _mockLogger.Object);

            var command = new CadastrarTarifaCommand
            {
                IdContaCorrente = "conta-123",
                DataMovimento = "25/12/2024",
                Valor = -5.50m
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("BAD_REQUEST", result.ErrorType);
            Assert.Equal("Valor deve ser maior que zero", result.Message);
            _mockTarifaRepository.Verify(x => x.AddAsync(It.IsAny<Tarifa>()), Times.Never);
        }
    }
}
