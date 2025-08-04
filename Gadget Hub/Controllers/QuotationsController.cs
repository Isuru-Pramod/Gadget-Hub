using GadgetHub.WebAPI.Models;
using GadgetHub.WebAPI.Models.Dtos;
using GadgetHub.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GadgetHub.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotationsController : ControllerBase
    {
        private readonly QuotationStore _store;

        public QuotationsController(QuotationStore store)
        {
            _store = store;
        }

        // Called by frontend when customer places an order
        [HttpPost("request")]
        public IActionResult RequestQuotations([FromBody] QuotationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CustomerUsername))
            {
                return BadRequest("Customer username is required.");
            }

            if (request.ProductOrders == null || request.ProductOrders.Count == 0)
            {
                return BadRequest("No product orders found.");
            }

            _store.CreateRequests(request.ProductOrders, request.Distributors, request.CustomerUsername);
            return Ok("Quotation requests created. Awaiting distributor responses.");
        }

        // Called by distributors to respond to a quotation
        [HttpPost("respond")]
        public IActionResult RespondToQuotation([FromBody] QuotationResponse response)
        {
            bool success = _store.AddResponse(response);

            if (!success)
            {
                return NotFound("Pending quotation not found for the specified distributor and product.");
            }

            return Ok("Quotation response saved successfully.");
        }

        [HttpPost("respond-batch")]
        public IActionResult RespondBatch([FromBody] BulkQuotationResponse bulk)
        {
            if (string.IsNullOrWhiteSpace(bulk.Distributor) || string.IsNullOrWhiteSpace(bulk.CustomerUsername))
            {
                return BadRequest("Distributor and CustomerUsername are required.");
            }

            var success = _store.HandleBulkResponse(bulk);

            return success
                ? Ok("Responses processed successfully.")
                : NotFound("Some quotations were not found or already processed.");
        }



        // For viewing quotation status per product (used by customer or admin)
        [HttpGet("status")]
        public IActionResult GetStatus([FromQuery] string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return BadRequest("Product ID is required.");

            var result = _store.GetQuotationsByProduct(productId);
            return Ok(result);
        }

        // View all quotations (for admin/debug)
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            return Ok(_store.GetAll());
        }
    }
}
