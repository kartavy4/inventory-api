using System.Security.Claims;
using AutoMapper;
using InventoryApi.Data;
using InventoryApi.Dtos;
using InventoryApi.Enums;
using InventoryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public OrderController(AppDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _context.Orders.
            Include(o=>o.Items)
            .ThenInclude(i=>i.Product)
            .ToListAsync();
        // if (orders == null) return NotFound();
        var result = _mapper.Map<List<OrderDto>>(orders);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy()
    {
        var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        
        var orders = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i=>i.Product)
            .Where(o => o.UserId == userId)
            .ToListAsync();
        var dto = _mapper.Map<List<OrderDto>>(orders);
        return Ok(dto);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i=>i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
        if(order == null) return NotFound();
        var result = _mapper.Map<OrderDto>(order);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderDto orderDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var order = new Order
            {
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Paid,
                Items = orderDto.OrderItems.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                }).ToList()
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, _mapper.Map<OrderDto>(order));
        
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] OrderDto orderDto)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
        if(order == null) return NotFound();
        OrderStatus status = order.Status;
        if (orderDto.OrderItems != null && orderDto.OrderItems.Any())
        {
            _context.OrderItems.RemoveRange(order.Items);
            var ids = orderDto.OrderItems.Select(i => i.ProductId).Distinct().ToList();
            var priceById = await _context.Products
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Price);
            order.Items = orderDto.OrderItems.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                PriceAtPurchase = priceById.TryGetValue(i.ProductId, out var price) ? price : 0m,
            }).ToList();
        }

        _context.SaveChangesAsync();
        var result = _mapper.Map<OrderDto>(order);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
        if(order == null) return NotFound();
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}