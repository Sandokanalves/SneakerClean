using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SneakerClean.Application.DTOs;
using SneakerClean.Application.Interfaces;

namespace SneakerClean.Application.Services;

public class CacheOrderService : IOrderService
{
    private readonly IOrderService _innerService;
    private readonly IDistributedCache _cache;
    private const string CacheKey = "orders_all";

    public CacheOrderService(IOrderService innerService, IDistributedCache cache)
    {
        _innerService = innerService;
        _cache = cache;
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        var cachedOrders = await _cache.GetStringAsync(CacheKey);
        if (!string.IsNullOrEmpty(cachedOrders))
            return JsonSerializer.Deserialize<IEnumerable<OrderDto>>(cachedOrders)!;

        var orders = await _innerService.GetAllAsync();
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
        };

        await _cache.SetStringAsync(CacheKey, JsonSerializer.Serialize(orders), options);
        return orders;
    }

    public Task<OrderDto?> GetByIdAsync(Guid id) =>
        _innerService.GetByIdAsync(id);

    public Task<IEnumerable<OrderDto>> GetFilteredAsync(OrderFilterRequest filter) =>
        _innerService.GetFilteredAsync(filter);

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request)
    {
        await _cache.RemoveAsync(CacheKey);
        return await _innerService.CreateAsync(request);
    }

    public async Task<OrderDto?> UpdateAsync(Guid id, UpdateOrderRequest request)
    {
        await _cache.RemoveAsync(CacheKey);
        return await _innerService.UpdateAsync(id, request);
    }

    public async Task<bool> UpdateStatusAsync(Guid id, string status)
    {
        await _cache.RemoveAsync(CacheKey);
        return await _innerService.UpdateStatusAsync(id, status);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await _cache.RemoveAsync(CacheKey);
        return await _innerService.DeleteAsync(id);
    }
}