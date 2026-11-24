using LW3.Dtos;

namespace LW3.Services;

public interface IMechanicService
{
    Task<IReadOnlyList<MechanicResponseDto>> GetAllAsync();
    Task<MechanicResponseDto> GetByIdAsync(string id);
    Task<MechanicResponseDto> CreateAsync(CreateMechanicDto dto);
    Task UpdateAsync(string id, UpdateMechanicDto dto);
    Task PatchAsync(string id, PatchMechanicDto dto);
    Task DeleteAsync(string id);
}
