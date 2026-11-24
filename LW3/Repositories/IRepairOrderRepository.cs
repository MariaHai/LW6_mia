using LW3.Models;

namespace LW3.Repositories;

public interface IRepairOrderRepository : IRepository<RepairOrder>
{
    Task<IReadOnlyList<RepairOrder>> GetByVehicleIdAsync(string vehicleId);
    Task<long> CountCompletedByVehicleAsync(string vehicleId);
}
