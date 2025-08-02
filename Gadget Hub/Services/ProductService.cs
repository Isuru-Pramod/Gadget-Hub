using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Data;

namespace GadgetHub.WebAPI.Services;

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public List<Product> GetAll() => _context.Products.ToList();

    public Product Add(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
        return product;
    }

    public Product GetById(string id)
    {
        return _context.Products.FirstOrDefault(p => p.Id == id);
    }

    public bool Delete(string id)
    {
        var product = GetById(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        _context.SaveChanges();
        return true;
    }

    public Product Update(string id, Product updated)
    {
        var product = GetById(id);
        if (product == null) return null;

        product.Name = updated.Name;
        product.Description = updated.Description;
        product.Category = updated.Category;
        _context.SaveChanges();
        return product;
    }
}
