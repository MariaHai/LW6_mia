using AutoMapper;
using LW3.Models;
using LW3.Repositories;
using LW3.Services;
using Moq;

namespace MathApp.Tests;

public class RepairOrderServiceTests
{
    private readonly Mock<IRepairOrderRepository> _mockOrders = new();
    private readonly Mock<IVehicleRepository> _mockVehicles = new();
    private readonly Mock<IMechanicRepository> _mockMechanics = new();
    private readonly Mock<IPartRepository> _mockParts = new();
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly RepairOrderService _service;

    public RepairOrderServiceTests()
    {
        _service = new RepairOrderService(
            _mockOrders.Object,
            _mockVehicles.Object,
            _mockMechanics.Object,
            _mockParts.Object,
            _mockMapper.Object);
    }

    private static List<RepairOrder> SampleOrders() => new()
    {
        new RepairOrder { Id = "1", TotalCost = 1000m, Status = OrderStatus.Completed,  CompletedAt = new DateTime(2026, 10, 5) },
        new RepairOrder { Id = "2", TotalCost = 3000m, Status = OrderStatus.Completed,  CompletedAt = new DateTime(2026, 10, 20) },
        new RepairOrder { Id = "3", TotalCost = 2000m, Status = OrderStatus.Completed,  CompletedAt = new DateTime(2026, 9, 1) },
        new RepairOrder { Id = "4", TotalCost = 2000m, Status = OrderStatus.InProgress, CompletedAt = null },
    };

    // ---------- GetAverageTotalCostAsync ----------
    [Fact]
    public async Task GetAverageTotalCostAsync_StandardData_ReturnsAverage()
    {
        _mockOrders.Setup(r => r.GetAllAsync()).ReturnsAsync(SampleOrders());

        var result = await _service.GetAverageTotalCostAsync();

        Assert.Equal(2000m, result);
        _mockOrders.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAverageTotalCostAsync_EmptyCollection_ReturnsZero()
    {
        _mockOrders.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<RepairOrder>());

        var result = await _service.GetAverageTotalCostAsync();

        Assert.Equal(0m, result);
    }

    // ---------- GetMonthlyTotalCostAsync ----------
    [Fact]
    public async Task GetMonthlyTotalCostAsync_October2026_SumsOnlyCompletedInThatMonth()
    {
        _mockOrders.Setup(r => r.GetAllAsync()).ReturnsAsync(SampleOrders());

        var result = await _service.GetMonthlyTotalCostAsync(2026, 10);

        Assert.Equal(4000m, result);
        _mockOrders.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetMonthlyTotalCostAsync_MonthWithoutOrders_ReturnsZero()
    {
        _mockOrders.Setup(r => r.GetAllAsync()).ReturnsAsync(SampleOrders());

        var result = await _service.GetMonthlyTotalCostAsync(2026, 1);

        Assert.Equal(0m, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    [InlineData(-1)]
    public async Task GetMonthlyTotalCostAsync_InvalidMonth_ThrowsAndDoesNotCallRepository(int month)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _service.GetMonthlyTotalCostAsync(2026, month));

        _mockOrders.Verify(r => r.GetAllAsync(), Times.Never);
    }

    // ---------- GetMostExpensiveOrderAsync ----------
    [Fact]
    public async Task GetMostExpensiveOrderAsync_StandardData_ReturnsMaxTotalCostOrder()
    {
        _mockOrders.Setup(r => r.GetAllAsync()).ReturnsAsync(SampleOrders());

        var result = await _service.GetMostExpensiveOrderAsync();

        Assert.Equal("2", result.Id);
        Assert.Equal(3000m, result.TotalCost);
    }

    [Fact]
    public async Task GetMostExpensiveOrderAsync_EmptyCollection_ThrowsInvalidOperationException()
    {
        _mockOrders.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<RepairOrder>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetMostExpensiveOrderAsync());
    }

    // ---------- CountByStatusAsync ----------
    [Theory]
    [InlineData(OrderStatus.Completed, 3)]
    [InlineData(OrderStatus.InProgress, 1)]
    [InlineData(OrderStatus.Diagnostics, 0)]
    public async Task CountByStatusAsync_VariousStatuses_ReturnsExpectedCount(OrderStatus status, int expected)
    {
        _mockOrders.Setup(r => r.GetAllAsync()).ReturnsAsync(SampleOrders());

        var result = await _service.CountByStatusAsync(status);

        Assert.Equal(expected, result);
    }

    // ---------- GetOrderOrThrowAsync ----------
    [Fact]
    public async Task GetOrderOrThrowAsync_ExistingId_ReturnsOrder()
    {
        var order = new RepairOrder { Id = "1", TotalCost = 1000m };
        _mockOrders.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(order);

        var result = await _service.GetOrderOrThrowAsync("1");

        Assert.Same(order, result);
        _mockOrders.Verify(r => r.GetByIdAsync("1"), Times.Once);
    }

    [Fact]
    public async Task GetOrderOrThrowAsync_UnknownId_ThrowsKeyNotFoundException()
    {
        _mockOrders.Setup(r => r.GetByIdAsync("999")).ReturnsAsync((RepairOrder?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetOrderOrThrowAsync("999"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetOrderOrThrowAsync_EmptyId_ThrowsArgumentException(string? id)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetOrderOrThrowAsync(id!));

        _mockOrders.Verify(r => r.GetByIdAsync(It.IsAny<string>()), Times.Never);
    }

    // ---------- GetCompletedCountForVehicleAsync ----------
    [Fact]
    public async Task GetCompletedCountForVehicleAsync_ValidVehicle_ReturnsCountFromRepository()
    {
        _mockOrders.Setup(r => r.CountCompletedByVehicleAsync("v1")).ReturnsAsync(5L);

        var result = await _service.GetCompletedCountForVehicleAsync("v1");

        Assert.Equal(5L, result);
        _mockOrders.Verify(r => r.CountCompletedByVehicleAsync("v1"), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetCompletedCountForVehicleAsync_EmptyVehicleId_ThrowsAndDoesNotCallRepository(string vehicleId)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetCompletedCountForVehicleAsync(vehicleId));

        _mockOrders.Verify(r => r.CountCompletedByVehicleAsync(It.IsAny<string>()), Times.Never);
    }

    // ---------- GetVehicleTotalSpentAsync ----------
    [Fact]
    public async Task GetVehicleTotalSpentAsync_MixedStatuses_SumsOnlyCompleted()
    {
        _mockOrders.Setup(r => r.GetByVehicleIdAsync("v1")).ReturnsAsync(SampleOrders());

        var result = await _service.GetVehicleTotalSpentAsync("v1");

        Assert.Equal(6000m, result);
        _mockOrders.Verify(r => r.GetByVehicleIdAsync("v1"), Times.Once);
    }

    [Fact]
    public async Task GetVehicleTotalSpentAsync_NoOrders_ReturnsZero()
    {
        _mockOrders.Setup(r => r.GetByVehicleIdAsync("v1")).ReturnsAsync(new List<RepairOrder>());

        var result = await _service.GetVehicleTotalSpentAsync("v1");

        Assert.Equal(0m, result);
    }
}