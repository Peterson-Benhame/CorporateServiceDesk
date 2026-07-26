using CorporateServiceDesk.Application.Common.Abstractions.Persistence;
using CorporateServiceDesk.Application.Common.Abstractions.Notifications;
using CorporateServiceDesk.Application.Tickets.Abstractions;
using CorporateServiceDesk.Application.Tickets.Create;
using CorporateServiceDesk.Domain.Tickets.Entities;
using CorporateServiceDesk.Domain.Tickets.Enums;
using CorporateServiceDesk.Application.UnitTests.Support;
using Moq;

namespace CorporateServiceDesk.Application.UnitTests.Tickets
{
    public sealed class CreateTicketUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_ShouldPersistNewTicket()
        {
            var repository = new Mock<ITicketRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var now = new DateTimeOffset(2026, 7, 22, 12, 0, 0, TimeSpan.Zero);
            var timeProvider = new TestTimeProvider(now);
            var requesterId = Guid.NewGuid();
            var command = new CreateTicketCommand("VPN unavailable", "Cannot connect.", requesterId, TicketPriority.High);
            var useCase = new CreateTicketUseCase(repository.Object, unitOfWork.Object, timeProvider);

            var result = await useCase.ExecuteAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Null(result.Error);
            Assert.Equal(EnumErrorType.Created, result.ErrorType);
            Assert.Equal(TicketStatus.Open, result.Value.Status);
            Assert.Equal(now, result.Value.OpenedAtUtc);
            Assert.Equal("VPN unavailable", result.Value.Title);

            repository.Verify(x => x.AddAsync(
                It.Is<Ticket>(ticket =>
                    ticket.RequesterId == requesterId &&
                    ticket.Title == "VPN unavailable" &&
                    ticket.Status == TicketStatus.Open),
                It.IsAny<CancellationToken>()),
                Times.Once);

            unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldNormalizeTitleBeforeCheckingDuplicate()
        {
            var repository = new Mock<ITicketRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var timeProvider = new TestTimeProvider(DateTimeOffset.UtcNow);
            var requesterId = Guid.NewGuid();
            var command = new CreateTicketCommand("  VPN unavailable  ", "Cannot connect.", requesterId, TicketPriority.High);
            var useCase = new CreateTicketUseCase(repository.Object, unitOfWork.Object, timeProvider);

            var result = await useCase.ExecuteAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("VPN unavailable", result.Value.Title);

            repository.Verify(x => x.ExistsByTitleForRequesterAsync(
                requesterId,
                "VPN unavailable",
                It.IsAny<CancellationToken>()),
                Times.Once);

            repository.Verify(x => x.AddAsync(
                It.Is<Ticket>(ticket => ticket.Title == "VPN unavailable"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnConflictFailure_WhenDuplicateTicketExists()
        {
            var repository = new Mock<ITicketRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var timeProvider = new TestTimeProvider(DateTimeOffset.UtcNow);
            var requesterId = Guid.NewGuid();
            var command = new CreateTicketCommand("VPN unavailable", "Cannot connect.", requesterId, TicketPriority.High);

            repository
                .Setup(x => x.ExistsByTitleForRequesterAsync(
                    requesterId,
                    command.Title,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var useCase = new CreateTicketUseCase(repository.Object, unitOfWork.Object, timeProvider);

            var result = await useCase.ExecuteAsync(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal(EnumErrorType.Conflict, result.ErrorType);
            Assert.Equal("A similar open ticket already exists for this requester.", result.Error);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldNotPersistTicket_WhenDuplicateTicketExists()
        {
            var repository = new Mock<ITicketRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var timeProvider = new TestTimeProvider(DateTimeOffset.UtcNow);
            var requesterId = Guid.NewGuid();
            var command = new CreateTicketCommand("VPN unavailable", "Cannot connect.", requesterId, TicketPriority.High);

            repository
                .Setup(x => x.ExistsByTitleForRequesterAsync(
                    requesterId,
                    command.Title,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var useCase = new CreateTicketUseCase(repository.Object, unitOfWork.Object, timeProvider);

            var result = await useCase.ExecuteAsync(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(EnumErrorType.Conflict, result.ErrorType);

            repository.Verify(x => x.AddAsync(
                It.IsAny<Ticket>(),
                It.IsAny<CancellationToken>()),
                Times.Never);

            unitOfWork.Verify(x => x.CommitAsync(
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnCreatedTicketData()
        {
            var repository = new Mock<ITicketRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var now = new DateTimeOffset(2026, 7, 22, 12, 0, 0, TimeSpan.Zero);
            var timeProvider = new TestTimeProvider(now);
            var requesterId = Guid.NewGuid();
            var command = new CreateTicketCommand("Printer unavailable", "Printer is offline.", requesterId, TicketPriority.Medium);
            var useCase = new CreateTicketUseCase(repository.Object, unitOfWork.Object, timeProvider);

            var result = await useCase.ExecuteAsync(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.NotEqual(Guid.Empty, result.Value.Id);
            Assert.Equal("Printer unavailable", result.Value.Title);
            Assert.Equal(TicketStatus.Open, result.Value.Status);
            Assert.Equal(now, result.Value.OpenedAtUtc);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldPassCancellationTokenToDependencies()
        {
            var repository = new Mock<ITicketRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var timeProvider = new TestTimeProvider(DateTimeOffset.UtcNow);
            var requesterId = Guid.NewGuid();
            var cancellationToken = new CancellationTokenSource().Token;
            var command = new CreateTicketCommand("VPN unavailable", "Cannot connect.", requesterId, TicketPriority.High);
            var useCase = new CreateTicketUseCase(repository.Object, unitOfWork.Object, timeProvider);

            await useCase.ExecuteAsync(command, cancellationToken);

            repository.Verify(x => x.ExistsByTitleForRequesterAsync(
                requesterId,
                command.Title,
                cancellationToken),
                Times.Once);

            repository.Verify(x => x.AddAsync(
                It.IsAny<Ticket>(),
                cancellationToken),
                Times.Once);

            unitOfWork.Verify(x => x.CommitAsync(cancellationToken), Times.Once);
        }
    }
}

