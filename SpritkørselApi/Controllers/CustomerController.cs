using Businesslayer;
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int rowsAffected = await Repository.CreateCustomerAsync(customer);

            if (rowsAffected > 0)
            {
                return Ok(new { message = "Customer created successfully." });
            }

            return BadRequest(new { message = "Failed to create customer." });
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

    }
}
