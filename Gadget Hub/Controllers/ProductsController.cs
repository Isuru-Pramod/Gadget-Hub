using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GadgetHub.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    // Get all products
    [HttpGet]
    public IActionResult GetAll() => Ok(_service.GetAll());

    // Get a product by ID
    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var result = _service.GetById(id);
        return result == null ? NotFound("Product not found.") : Ok(result);
    }

    // Get image for a product
    [HttpGet("{id}/image")]
    public IActionResult GetImage(string id)
    {
        var product = _service.GetById(id);
        if (product == null || product.ImageData == null)
            return NotFound("Image not found");

        return File(product.ImageData, product.ImageType ?? "application/octet-stream");
    }

    // Upload product with image
    [HttpPost("upload")]
    public async Task<IActionResult> UploadProductWithImage([FromForm] ProductUploadDto dto)
    {
        if (dto.Image == null || dto.Image.Length == 0)
            return BadRequest("Image file is required.");

        var product = new Product
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category
        };

        using (var ms = new MemoryStream())
        {
            await dto.Image.CopyToAsync(ms);
            product.ImageData = ms.ToArray();
            product.ImageType = dto.Image.ContentType;
        }

        var created = _service.Add(product);
        return Ok(created);
    }

    // Update a product (excluding image)
    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] Product updated)
    {
        var result = _service.Update(id, updated);
        return result == null ? NotFound("Product not found.") : Ok(result);
    }

    // Delete a product
    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var result = _service.Delete(id);
        return result ? Ok("Product deleted.") : NotFound("Product not found.");
    }
}
