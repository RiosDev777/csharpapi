using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using csharpapi.Data.Entities;
using csharpapi.Data;
using Microsoft.EntityFrameworkCore;

namespace csharpapi.Controllers
{

[ApiController]
[Route("api/[controller]")]
    public class EmpresaController : ControllerBase
    {
        private readonly DataContext _context;

        public EmpresaController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Empresa>>> Get()
        {
            return await _context.Empresas.ToListAsync();
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<Empresa>> GetEmpresa(int codigo)
        {
            var empresa = await _context.Empresas.FindAsync(codigo);
            if (empresa == null)
            {
                return NotFound();
            }
            return empresa;
        }

        [HttpPost]
        public async Task<ActionResult<Empresa>> Post([FromBody] Empresa empresa)
        {
            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmpresa), new { codigo = empresa.Codigo }, empresa);
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> Put(int codigo, [FromBody] Empresa empresaActualizada)
        {
            if (codigo != empresaActualizada.Codigo)
            {
                return BadRequest();
            }

            _context.Entry(empresaActualizada).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Delete(int codigo)
        {
            var empresa = await _context.Empresas.FindAsync(codigo);
            if (empresa == null)
            {
                return NotFound();
            }

            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
