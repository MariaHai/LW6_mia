using LW3.Dtos;

namespace LW3.Services;

public interface IPartService
{
    Task<IReadOnlyList<PartResponseDto>> GetAllAsync();
    Task<PartResponseDto> GetByIdAsync(string id);
    Task<PartResponseDto> CreateAsync(CreatePartDto dto);
    Task UpdateAsync(string id, UpdatePartDto dto);
    Task DeleteAsync(string id);
}
