using LW3.Dtos;

namespace LW3.Services;

public interface IVehicleService
{
    Task<IReadOnlyList<VehicleResponseDto>> GetAllAsync();
    Task<VehicleResponseDto> GetByIdAsync(string id);
    Task<VehicleResponseDto> CreateAsync(CreateVehicleDto dto);
    Task UpdateAsync(string id, UpdateVehicleDto dto);
    Task PatchAsync(string id, PatchVehicleDto dto);
    Task DeleteAsync(string id);
}
