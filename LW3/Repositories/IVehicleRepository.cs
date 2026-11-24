using LW3.Models;

namespace LW3.Repositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<Vehicle?> GetByVinAsync(string vin);
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate);
}
