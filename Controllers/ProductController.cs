using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using InventoryApi.Data;
using InventoryApi.Dtos;
using InventoryApi.Models;
using InventoryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Controllers;
[Authorize]
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
        var products = await _context.Products.Include(p => p.Category).ToListAsync();
        var productDtos = _mapper.Map<List<ProductDto>>(products);
        return Ok(productDtos);
    }
    
    //GET: /api/products/id
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p=> p.Id==id);
        if (product == null) return NotFound();
        var productDto = _mapper.Map<ProductDto>(product);
        
        return Ok(productDto);
    }
    
    //POST: /api/products
    
    [HttpPost]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> Create([FromBody]ProductCreateDto dto)
    {
        if (!await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId)) 
            return BadRequest($"Category Id {dto.CategoryId} does not exist");
        
        var product = _mapper.Map<Product>(dto);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        var productWithCategory = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == product.Id);
        
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, _mapper.Map<ProductDto>(productWithCategory));
    }
    
    //PUT: /api/products/id
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
    {
        var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p=> p.Id==id);
        
        if (product == null) return NotFound();
        if (!await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId)) return BadRequest("Category Id not found");
        
        _mapper.Map(dto, product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    //DELETE: /api/products/id
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpGet("filtered")]

    public async Task<IActionResult> GetFiltered(
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "Name",
        [FromQuery] bool isDescending = false)
    {
        var query = _context.Products.Include(p => p.Category).AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));
        }

        query = query.Include(p => p.Category);


        var sortValue = string.IsNullOrEmpty(sortBy) ? "name" : sortBy.ToLower();
        query = sortValue switch
        {
            "price" => isDescending 
                ? query.OrderByDescending(p => p.Price) 
                : query.OrderBy(p => p.Price),
            "category" => isDescending 
                ? query.OrderByDescending(p => p.Category.Name) 
                : query.OrderBy(p => p.Category.Name),
            _ => isDescending 
                ? query.OrderByDescending(p => p.Name) 
                : query.OrderBy(p => p.Name)
        };


        var totalItems = await query.CountAsync();
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        var productDtos = _mapper.Map<List<ProductDto>>(products);
        return Ok(new { totalItems, products = productDtos });
    }
}