using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using csharpapi.Data;
using csharpapi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace csharpapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductoController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> Get()
        {
            return await _context.Productos.ToListAsync();
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<Producto>> GetProducto(int codigo)
        {
            var producto = await _context.Productos.FindAsync(codigo);
            if (producto == null)
            {
                return NotFound();
            }
            return producto;
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> Post([FromBody] Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProducto), new { codigo = producto.Codigo }, producto);
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> Put(int codigo, [FromBody] Producto productoActualizado)
        {
            if (codigo != productoActualizado.Codigo)
            {
                return BadRequest();
            }

            _context.Entry(productoActualizado).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Delete(int codigo)
        {
            var producto = await _context.Productos.FindAsync(codigo);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
