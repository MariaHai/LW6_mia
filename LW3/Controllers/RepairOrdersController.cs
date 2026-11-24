using LW3.Dtos;
using LW3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LW3.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RepairOrdersController : ControllerBase
{
    private readonly IRepairOrderService _service;

    public RepairOrdersController(IRepairOrderService service)
    {
        _service = service;
    }

    // ===================== CRUD =====================

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RepairOrderResponseDto>>> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<RepairOrderResponseDto>> GetById(string id) => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<RepairOrderResponseDto>> Create(CreateRepairOrderDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateRepairOrderDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(string id, PatchRepairOrderDto dto)
    {
        await _service.PatchAsync(id, dto);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    // ===================== Бізнес-задачі =====================

    // 1. Прийом автомобіля та формування наряду-замовлення
    [HttpPost("reception")]
    public async Task<ActionResult<ReceiveVehicleResponseDto>> ReceiveVehicle(ReceiveVehicleDto dto)
    {
        var result = await _service.ReceiveVehicleAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.OrderId }, result);
    }

    // 2. Затвердження кошторису та резервування автозапчастин
    [HttpPost("{id}/estimate")]
    public async Task<ActionResult<EstimateResponseDto>> ApproveEstimate(string id, ApproveEstimateDto dto) =>
        Ok(await _service.ApproveEstimateAsync(id, dto));

    // 3. Контроль якості, видача авто та відкриття гарантії
    [HttpPost("{id}/complete")]
    public async Task<ActionResult<CompleteOrderResponseDto>> CompleteOrder(string id, CompleteOrderDto dto) =>
        Ok(await _service.CompleteOrderAsync(id, dto));
}
