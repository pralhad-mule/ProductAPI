
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
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }


        // GET ALL PRODUCTS
        // GET: api/Product

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products =
                await _context.Products.ToListAsync();

            return Ok(products);
        }


        // GET PRODUCT BY ID
        // GET: api/Product/1

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product =
                await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound(
                    "Product not found");
            }

            return Ok(product);
        }


        // CREATE PRODUCT
        // POST: api/Product

        [HttpPost]
        public async Task<IActionResult> CreateProduct(
            Product product)
        {
            product.CreatedOn = DateTime.Now;

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return Ok(product);
        }


        // UPDATE PRODUCT
        // PUT: api/Product/1

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(
            int id,
            Product product)
        {
            var existingProduct =
                await _context.Products
                .FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound(
                    "Product not found");
            }


            existingProduct.ProductName =
                product.ProductName;

            existingProduct.ModifiedBy =
                product.ModifiedBy;

            existingProduct.ModifiedOn =
                DateTime.Now;


            await _context.SaveChangesAsync();

            return Ok(existingProduct);
        }


        // DELETE PRODUCT
        // DELETE: api/Product/1

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(
            int id)
        {
            var product =
                await _context.Products
                .FindAsync(id);

            if (product == null)
            {
                return NotFound(
                    "Product not found");
            }


            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return Ok(
                "Product deleted successfully");
        }
    }
}

