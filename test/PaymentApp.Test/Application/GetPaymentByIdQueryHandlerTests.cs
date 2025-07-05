using Moq; 
using PaymentApp.Application.Commands;
using PaymentApp.Application.Dtos;
using PaymentApp.Application.Interfaces.Repositories;
using PaymentApp.Domain.Entities;
using PaymentApp.Domain.ValueObjects;

namespace PaymentApp.Test.Application;

public class GetPaymentByIdQueryHandlerTests
{
	[Fact]
	public async Task HandleAsync_PaymentExists_ReturnsDto()
	{
		// Arrange  
		var repoMock = new Mock<IPaymentRepository>();
		repoMock
			.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		var handler = new CreatePaymentCommandHandler(repoMock.Object);
		var dto = new CreatePaymentDto(Guid.NewGuid(), 25m, "USD");
		var command = new CreatePaymentCommand(dto);

		// Act  
		var result = await handler.HandleAsync(command);

		// Assert  
		Assert.NotEqual(Guid.Empty, result.Id);  
		Assert.Equal(dto.CustomerId, result.CustomerId);
		Assert.Equal(dto.Amount, result.Amount);
		Assert.Equal(dto.Currency, result.Currency);
		Assert.Equal("Pending", result.Status);
	}

	[Fact]
	public async Task HandleAsync_PaymentNotFound_ReturnsNull()
	{
		// Arrange  
		var repoMock = new Mock<IPaymentRepository>();
		repoMock
			.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Payment)null!);

		var handler = new GetPaymentByIdQueryHandler(repoMock.Object);
		var result = await handler.HandleAsync(new GetPaymentByIdQuery(Guid.NewGuid()));

		Assert.Null(result);
	}
}
