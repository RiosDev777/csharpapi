using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using csharpapi.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using csharpapi.Data;

namespace csharpapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly DataContext _context;

        public UsuarioController(DataContext context)
        {
            _context = context;
        }

        // Obtener todos los usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> Get()
        {
            return await _context.Usuarios.ToListAsync();
        }

        //  Obtener un usuario por email
        [HttpGet("{email}")]
        public async Task<ActionResult<Usuario>> GetUsuario(string email)
        {
            var usuario = await _context.Usuarios.FindAsync(email);
            if (usuario == null)
            {
                return NotFound();
            }
            return usuario;
        }

        // Crear un nuevo usuario
        [HttpPost]
        public async Task<ActionResult<Usuario>> Post([FromBody] Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUsuario), new { email = usuario.Email }, usuario);
        }

        // Actualizar un usuario existente
        [HttpPut("{email}")]
        public async Task<IActionResult> Put(string email, [FromBody] Usuario usuarioActualizado)
        {
            if (email != usuarioActualizado.Email)
            {
                return BadRequest();
            }

            _context.Entry(usuarioActualizado).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Eliminar un usuario
        [HttpDelete("{email}")]
        public async Task<IActionResult> Delete(string email)
        {
            var usuario = await _context.Usuarios.FindAsync(email);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
