using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using moviesMVC.Data;
using moviesMVC.Models;

namespace moviesMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MovieDbContext _context;

        public HomeController(ILogger<HomeController> logger, MovieDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, string txtBusqueda = "", int generoId = 0)
        {
            const int pageSize = 8;
            if (page < 1) page = 1;

            var consulta = _context.Peliculas.AsQueryable();

            if (!string.IsNullOrEmpty(txtBusqueda))
            {
               consulta = consulta.Where(p => p.Titulo.Contains(txtBusqueda));            
            }

            if (generoId != 0)
            {
                consulta = consulta.Where(p => p.GeneroId == generoId);
            }

            var totalPeliculas = await consulta.CountAsync();
            var totalPaginas = (int)Math.Ceiling(totalPeliculas / (double)pageSize);
            if (totalPaginas == 0) totalPaginas = 1;
            if (page > totalPaginas) page = totalPaginas;

            var peliculas = await consulta
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPaginas;
            ViewBag.PageSize = pageSize;
            ViewBag.TxtBusqueda = txtBusqueda;

            var generos = await _context.Generos.OrderBy(g => g.Descripcion).ToListAsync();

            generos.Insert(0, new Genero {Id = 0,  Descripcion = "Todos"});

            ViewBag.GeneroId = new SelectList(
                generos,
                "Id",
                "Descripcion",
                generoId);

            return View(peliculas);
        }

        public async Task<IActionResult> Details(int id)
        {
            var pelicula = await _context.Peliculas
                .Include(p => p.Genero)
                .Include(p => p.ListaReviews)
                    .ThenInclude(r => r.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            ViewBag.UserReview = false;

            if (User?.Identity?.IsAuthenticated == true && pelicula.ListaReviews != null)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.UserReview = !(pelicula.ListaReviews.Any(r => r.UsuarioId == userId) == null);
            }

            return View(pelicula);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
