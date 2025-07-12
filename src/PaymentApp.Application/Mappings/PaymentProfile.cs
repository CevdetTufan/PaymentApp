using AutoMapper;
using PaymentApp.Application.Dtos;
using PaymentApp.Domain.Entities;

namespace PaymentApp.Application.Mappings;

public class PaymentProfile : Profile
{
	public PaymentProfile()
	{
		//mappinf to record with constructor parameters
		CreateMap<Payment, PaymentDto>()
			.ForCtorParam("Id", opt => opt.MapFrom(s => s.Id))
			.ForCtorParam("CustomerId", opt => opt.MapFrom(s => s.CustomerId))
			.ForCtorParam("Amount", opt => opt.MapFrom(s => s.Amount.Amount))
			.ForCtorParam("Currency", opt => opt.MapFrom(s => s.Amount.Currency))
			.ForCtorParam("Status", opt => opt.MapFrom(s => s.Status.ToString()))
			.ForCtorParam("CreatedAt", opt => opt.MapFrom(s => s.CreatedAt))
			.ForCtorParam("ProcessedAt", opt => opt.MapFrom(s => s.ProcessedAt));
	}
}
