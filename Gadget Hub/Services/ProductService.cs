using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace GadgetHub.WebAPI.Services;

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAll() => await _context.Products.ToListAsync();

    public async Task<Product> Add(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> GetById(string id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> Update(string id, Product updated)
    {
        var product = await GetById(id);
        if (product == null) return null;

        product.Name = updated.Name ?? product.Name;
        product.Description = updated.Description ?? product.Description;
        product.Category = updated.Category ?? product.Category;

        if (updated.ImageData != null)
        {
            product.ImageData = updated.ImageData;
            product.ImageType = updated.ImageType;
        }

        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> Delete(string id)
    {
        var product = await GetById(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}