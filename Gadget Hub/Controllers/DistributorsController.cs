using GadgetHub.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GadgetHub.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DistributorsController : ControllerBase
{
    [HttpPost("{distributor}/quote")]
    public IActionResult SimulateDistributor(string distributor, [FromBody] ProductOrder order)
    {
        var rand = new Random();

        var response = new QuotationResponse
        {
            Distributor = distributor,
            ProductId = order.ProductId,
            PricePerUnit = rand.Next(100, 500),
            AvailableUnits = rand.Next(1, 20),
            EstimatedDeliveryDays = rand.Next(1, 10)
        };

        return Ok(response);
    }
}
