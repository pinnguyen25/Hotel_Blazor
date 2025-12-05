using HotelBooking.infrastructure.Models;

public interface IPolicyTypeRepository : IRepository<PolicyType>
{
    // Add custom methods for PolicyType here if needed
}

public class PolicyTypeRepository : Repository<PolicyType>, IPolicyTypeRepository
{
    public PolicyTypeRepository(HotelBookingContext context) : base(context)
    {
    }
}