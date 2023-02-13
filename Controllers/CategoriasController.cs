using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context) => _context = context;
        
        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get(string? nome, bool? situacao)
        {
            var query = _context.Categorias.AsNoTracking();

            if (!string.IsNullOrEmpty(nome))
                query = query.Where(c => c.Nome.Contains(nome));

            if (situacao != null)
                query = query.Where(c => c.Situacao == situacao);

            if (!query.Any())
                return NotFound("Não foi encontrado nenhum registro");

            return query.ToList();
        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            _context.Categorias.Add(categoria);
            _context.SaveChanges();

            return Ok(categoria);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, [FromBody] Categoria categoria)
        {
            if (id != categoria.CategoriaId)
                return BadRequest();

            Categoria categoriaBanco = _context.Categorias.Find(id);

            if (categoriaBanco == null)
                return NotFound();

            categoriaBanco.Nome = categoria.Nome;
            categoriaBanco.Situacao = categoria.Situacao;

            _context.Categorias.Entry(categoriaBanco).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(categoriaBanco);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);
            if (categoria == null)
                return NotFound("Categoria não encontrado...");

            _context.Categorias.Remove(categoria);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
