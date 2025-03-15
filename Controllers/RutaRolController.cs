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
    public class RutaRolController : ControllerBase
    {
        private readonly DataContext _context;

        public RutaRolController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RutaRol>>> Get()
        {
            return await _context.RutaRoles.ToListAsync();
        }

        [HttpGet("{ruta}")]
        public async Task<ActionResult<RutaRol>> GetRutaRol(string ruta)
        {
            var rutaRol = await _context.RutaRoles.FindAsync(ruta);
            if (rutaRol == null)
            {
                return NotFound();
            }
            return rutaRol;
        }

        [HttpPost]
        public async Task<ActionResult<RutaRol>> Post([FromBody] RutaRol rutaRol)
        {
            _context.RutaRoles.Add(rutaRol);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRutaRol), new { ruta = rutaRol.Ruta }, rutaRol);
        }

        [HttpPut("{ruta}")]
        public async Task<IActionResult> Put(string ruta, [FromBody] RutaRol rutaRolActualizado)
        {
            if (ruta != rutaRolActualizado.Ruta)
            {
                return BadRequest();
            }

            _context.Entry(rutaRolActualizado).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{ruta}")]
        public async Task<IActionResult> Delete(string ruta)
        {
            var rutaRol = await _context.RutaRoles.FindAsync(ruta);
            if (rutaRol == null)
            {
                return NotFound();
            }

            _context.RutaRoles.Remove(rutaRol);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}