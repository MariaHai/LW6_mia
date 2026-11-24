using AutoMapper;
using LW3.Dtos;
using LW3.Exceptions;
using LW3.Models;
using LW3.Repositories;

namespace LW3.Services;

public class MechanicService : IMechanicService
{
    private readonly IMechanicRepository _repository;
    private readonly IMapper _mapper;

    public MechanicService(IMechanicRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<MechanicResponseDto>> GetAllAsync()
    {
        var mechanics = await _repository.GetAllAsync();
        return _mapper.Map<List<MechanicResponseDto>>(mechanics);
    }

    public async Task<MechanicResponseDto> GetByIdAsync(string id) =>
        _mapper.Map<MechanicResponseDto>(await GetEntityAsync(id));

    public async Task<MechanicResponseDto> CreateAsync(CreateMechanicDto dto)
    {
        var mechanic = _mapper.Map<Mechanic>(dto);
        var created = await _repository.CreateAsync(mechanic);
        return _mapper.Map<MechanicResponseDto>(created);
    }

    public async Task UpdateAsync(string id, UpdateMechanicDto dto)
    {
        var mechanic = await GetEntityAsync(id);
        _mapper.Map(dto, mechanic); // графік (Schedule) не змінюється
        await _repository.UpdateAsync(id, mechanic);
    }

    public async Task PatchAsync(string id, PatchMechanicDto dto)
    {
        var mechanic = await GetEntityAsync(id);
        _mapper.Map(dto, mechanic);
        await _repository.UpdateAsync(id, mechanic);
    }

    public async Task DeleteAsync(string id)
    {
        if (!await _repository.DeleteAsync(id))
            throw new NotFoundException($"Майстра з ID={id} не знайдено.");
    }

    private async Task<Mechanic> GetEntityAsync(string id) =>
        await _repository.GetByIdAsync(id)
        ?? throw new NotFoundException($"Майстра з ID={id} не знайдено.");
}
