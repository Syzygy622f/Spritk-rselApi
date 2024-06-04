using Businesslayer;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Models;
namespace SpritkørselApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly IRepository Repository;

        public CustomerController(IRepository customerRepository)
        {
            Repository = customerRepository;
        }


        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer customer)
        {
            // Validate the model state and return a bad request if it's not valid.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Attempt to create the customer asynchronously.
                int rowsAffected = await Repository.CreateCustomerAsync(customer);

                // Check if the operation was successful.
                if (rowsAffected > 0)
                {
                    // Return a successful response if the customer was created.
                    return Ok(new { message = "Customer created successfully." });
                }
                else
                {
                    // Handle cases where the operation did not affect any rows.
                    // This could indicate a failure or a partial success.
                    return BadRequest(new { message = "Failed to create customer." });
                }
            }
            catch
            {
                // Log the exception and return a server error response.
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }


        [HttpPost("CreateInvoice")]
        public async Task<IActionResult> CreateInvoice(List<int> productsId, string mail)
        {
            // Validate the model state and return a bad request if it's not valid.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Attempt to create the invoice asynchronously.
                int rowsAffected = await Repository.CreateInvoice(productsId, mail);

                // Check if the operation was successful.
                if (rowsAffected > 0)
                {
                    // Return a successful response if the invoice was created.
                    return Ok(new { message = "Invoice created successfully." });
                }
                else
                {
                    // Handle cases where the operation did not affect any rows.
                    // This could indicate a failure or a partial success.
                    return BadRequest(new { message = "Failed to create invoice." });
                }
            }
            catch
            {
                // Log the exception and return a server error response.
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {     
            return Ok(await Repository.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var customer = await Repository.GetProductByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }


        [HttpGet("costumerExist")]
        public async Task<IActionResult> GetCustomerAsync(string mail)
        {
            return Ok(await Repository.getCostumerAsynce(mail));
        }
    }
}
