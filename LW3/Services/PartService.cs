using AutoMapper;
using LW3.Dtos;
using LW3.Exceptions;
using LW3.Models;
using LW3.Repositories;

namespace LW3.Services;

public class PartService : IPartService
{
    private readonly IPartRepository _repository;
    private readonly IMapper _mapper;

    public PartService(IPartRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PartResponseDto>> GetAllAsync()
    {
        var parts = await _repository.GetAllAsync();
        return _mapper.Map<List<PartResponseDto>>(parts);
    }

    public async Task<PartResponseDto> GetByIdAsync(string id) =>
        _mapper.Map<PartResponseDto>(await GetEntityAsync(id));

    public async Task<PartResponseDto> CreateAsync(CreatePartDto dto)
    {
        var part = _mapper.Map<Part>(dto);
        var created = await _repository.CreateAsync(part);
        return _mapper.Map<PartResponseDto>(created);
    }

    public async Task UpdateAsync(string id, UpdatePartDto dto)
    {
        var part = await GetEntityAsync(id);
        _mapper.Map(dto, part);
        await _repository.UpdateAsync(id, part);
    }

    public async Task DeleteAsync(string id)
    {
        if (!await _repository.DeleteAsync(id))
            throw new NotFoundException($"Запчастину з ID={id} не знайдено.");
    }

    private async Task<Part> GetEntityAsync(string id) =>
        await _repository.GetByIdAsync(id)
        ?? throw new NotFoundException($"Запчастину з ID={id} не знайдено.");
}
