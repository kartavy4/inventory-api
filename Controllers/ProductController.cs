using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using InventoryApi.Data;
using InventoryApi.Dtos;
using InventoryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public ProductController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    //GET: /api/products
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }
    
    //GET: /api/products/id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }
    
    //POST: /api/products
    [HttpPost]
    public async Task<IActionResult> Create(ProductDTO dto)
    {
        var product = _mapper.Map<Product>(dto);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, _mapper.Map<ProductDTO>(product));
    }
    
    //PUT: /api/products/id
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductDTO dto)
    {
        var existing = await _context.Products.FindAsync(id);
        if (existing == null) return NotFound();
        _mapper.Map(dto, existing);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    //DELETE: /api/products/id
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}