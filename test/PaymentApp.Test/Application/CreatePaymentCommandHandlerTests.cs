using AutoMapper;
using Moq;
using PaymentApp.Application.Commands;
using PaymentApp.Application.Dtos;
using PaymentApp.Application.Interfaces.Repositories;
using PaymentApp.Domain.Entities;

namespace PaymentApp.Test.Application;
public class CreatePaymentCommandHandlerTests
{
	[Fact]
	public async Task HandleAsync_ValidCommand_CreatesAndReturnsDto()
	{
		// Arrange
		var repoMock = new Mock<IPaymentRepository>();
		repoMock
			.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask)
			.Verifiable();

		var mapperMock = new Mock<IMapper>();
		var handler = new CreatePaymentCommandHandler(repoMock.Object, mapperMock.Object);

		var dto = new CreatePaymentDto(Guid.NewGuid(), 25m, "USD");
		var command = new CreatePaymentCommand(dto);

		// Act
		var result = await handler.HandleAsync(command);

		// Assert
		repoMock.Verify();
		Assert.Equal(dto.CustomerId, result.CustomerId);
		Assert.Equal(dto.Amount, result.Amount);
		Assert.Equal(dto.Currency, result.Currency);
		Assert.Equal("Pending", result.Status);
		Assert.NotEqual(default, result.CreatedAt);
	}
}
