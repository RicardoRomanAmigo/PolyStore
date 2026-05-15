using Microsoft.EntityFrameworkCore;
using PolyStore.Domain.Entities;
using PolyStore.Domain.Enums;
using PolyStore.Application.Abstractions.Persistence;
using PolyStore.Infrastructure.Persistence.Context;

namespace PolyStore.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StoreDbContext _context;

    //Inyector del contexto 
    public ProductRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetLiveProductAsync()
    {
        //Busca el producto marcado como Live
        return await _context.Products
            .AsNoTracking() //Mejora el rendimiento (solo lectura)
            .OrderByDescending(p => p.PublishedAt)
            .FirstOrDefaultAsync(p => p.Status == ProductStatus.Live);
    }

    public async Task<IEnumerable<Product>> GetArchivedProductsAsync()
    {
        //Devuelve la lista de los productos archivados
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Status == ProductStatus.Archived)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        //Devuelve lista completa de los productos de live y archived
        return await _context.Products
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();
    }

    public async Task AddProductAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public void Update(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
    }

    public async Task<bool> SaveChangesAsync()
    {
        //Devuleve true si se guardo al menos un cambio
        return await _context.SaveChangesAsync() > 0;
    }

    public void Delete(Product product)
    {
        _context.Products.Remove(product);
    } 
}