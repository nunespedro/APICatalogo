using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context) =>  _context = context;

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get(int? categoriaId, string? descricao, bool? situacao)
        {
            var query = _context.Produtos.AsNoTracking();

            if (categoriaId != null && categoriaId != 0)
                query = query.Where(p => p.CategoriaId == categoriaId);

            if (!string.IsNullOrEmpty(descricao))
                query = query.Where(p => p.Descricao.Contains(descricao));

            if (situacao != null)
                query = query.Where(p => p.Situacao == situacao);

            if (!query.Any())
                return NotFound("Nenhum produto foi encontrado.");

            return query.ToList();
        }

        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var categoria = _context.Categorias.Find(produto.CategoriaId);

            if (categoria == null)
                return BadRequest();

            produto.Categoria = categoria;

            _context.Produtos.Add(produto);
            _context.SaveChanges();

            return Ok(produto);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return BadRequest();
            }

            _context.Entry(produto).State = EntityState.Modified;
            _context.SaveChanges();
            
            return Ok(produto);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            if (produto == null)
                return NotFound("Produto não encontrado...");

            _context.Produtos.Remove(produto);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
