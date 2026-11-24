using AutoMapper;
using LW3.Dtos;
using LW3.Exceptions;
using LW3.Models;
using LW3.Repositories;

namespace LW3.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _repository;
    private readonly IMapper _mapper;

    public VehicleService(IVehicleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<VehicleResponseDto>> GetAllAsync()
    {
        var vehicles = await _repository.GetAllAsync();
        return _mapper.Map<List<VehicleResponseDto>>(vehicles);
    }

    public async Task<VehicleResponseDto> GetByIdAsync(string id) =>
        _mapper.Map<VehicleResponseDto>(await GetEntityAsync(id));

    public async Task<VehicleResponseDto> CreateAsync(CreateVehicleDto dto)
    {
        var vehicle = _mapper.Map<Vehicle>(dto);
        var created = await _repository.CreateAsync(vehicle);
        return _mapper.Map<VehicleResponseDto>(created);
    }

    public async Task UpdateAsync(string id, UpdateVehicleDto dto)
    {
        var vehicle = await GetEntityAsync(id);
        _mapper.Map(dto, vehicle); // копіює поля DTO в існуючу модель (Id і VIN не чіпає)
        await _repository.UpdateAsync(id, vehicle);
    }

    public async Task PatchAsync(string id, PatchVehicleDto dto)
    {
        var vehicle = await GetEntityAsync(id);
        _mapper.Map(dto, vehicle); // лише передані (не null) поля
        await _repository.UpdateAsync(id, vehicle);
    }

    public async Task DeleteAsync(string id)
    {
        if (!await _repository.DeleteAsync(id))
            throw new NotFoundException($"Автомобіль з ID={id} не знайдено.");
    }

    private async Task<Vehicle> GetEntityAsync(string id) =>
        await _repository.GetByIdAsync(id)
        ?? throw new NotFoundException($"Автомобіль з ID={id} не знайдено.");
}
