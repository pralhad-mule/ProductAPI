
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemController(AppDbContext context)
        {
            _context = context;
        }


        // GET ALL ITEMS
        // GET: api/Item

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var items =
                await _context.Items
                .Include(x => x.Product)
                .ToListAsync();

            return Ok(items);
        }


        // GET ITEM BY ID
        // GET: api/Item/1

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            var item =
                await _context.Items
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                return NotFound(
                    "Item not found");
            }

            return Ok(item);
        }


        // CREATE ITEM
        // POST: api/Item

        [HttpPost]
        public async Task<IActionResult> CreateItem(
            Item item)
        {
            var product =
                await _context.Products
                .FindAsync(item.ProductId);


            if (product == null)
            {
                return BadRequest(
                    "Product does not exist");
            }


            _context.Items.Add(item);

            await _context.SaveChangesAsync();

            return Ok(item);
        }


        // UPDATE ITEM
        // PUT: api/Item/1

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(
            int id,
            Item item)
        {
            var existingItem =
                await _context.Items
                .FindAsync(id);


            if (existingItem == null)
            {
                return NotFound(
                    "Item not found");
            }


            var product =
                await _context.Products
                .FindAsync(item.ProductId);


            if (product == null)
            {
                return BadRequest(
                    "Product does not exist");
            }


            existingItem.ProductId =
                item.ProductId;

            existingItem.Quantity =
                item.Quantity;


            await _context.SaveChangesAsync();

            return Ok(existingItem);
        }


        // DELETE ITEM
        // DELETE: api/Item/1

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(
            int id)
        {
            var item =
                await _context.Items
                .FindAsync(id);


            if (item == null)
            {
                return NotFound(
                    "Item not found");
            }


            _context.Items.Remove(item);

            await _context.SaveChangesAsync();

            return Ok(
                "Item deleted successfully");
        }
    }
}

