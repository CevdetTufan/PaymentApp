using AutoMapper;
using Moq;
using PaymentApp.Application.Commands;
using PaymentApp.Application.Dtos;
using PaymentApp.Application.Interfaces.Repositories;
using PaymentApp.Domain.Entities;

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

		// Map mock to record with constructor parameters
		var mapperMock = new Mock<IMapper>();
		mapperMock
			.Setup(m => m.Map<PaymentDto>(It.IsAny<Payment>()))
			.Returns<Payment>(p => new PaymentDto(
				p.Id,
				p.CustomerId,
				p.Amount.Amount,
				p.Amount.Currency,
				p.Status.ToString(),
				p.CreatedAt,
				p.ProcessedAt));

		var handler = new CreatePaymentCommandHandler(repoMock.Object, mapperMock.Object);
		var dto = new CreatePaymentDto(Guid.NewGuid(), 25m, "USD");
		var command = new CreatePaymentCommand(dto);

		// Act  
		var result = await handler.HandleAsync(command);

		// Assert  
		Assert.NotNull(result);
		Assert.NotEqual(Guid.Empty, result.Id);
		Assert.Equal(dto.CustomerId, result.CustomerId);
		Assert.Equal(dto.Amount, result.Amount);
		Assert.Equal(dto.Currency, result.Currency);
		Assert.Equal("Pending", result.Status);
	}


	[Fact]
	public async Task HandleAsync_PaymentNotFound_ThrowsKeyNotFoundException()
	{
		// Arrange
		var id = Guid.NewGuid();
		var repoMock = new Mock<IPaymentRepository>();
		repoMock
			.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
			.ReturnsAsync((Payment?)null);

		var mapperMock = new Mock<IMapper>();
		var handler = new GetPaymentByIdQueryHandler(repoMock.Object, mapperMock.Object);

		// Act & Assert
		var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
			handler.HandleAsync(new GetPaymentByIdQuery(id)));

		Assert.Equal($"Payment with ID {id} not found.", ex.Message);
	}
}
