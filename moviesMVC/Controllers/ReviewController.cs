using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using moviesMVC.Data;
using moviesMVC.Models;

namespace moviesMVC.Controllers
{
    public class ReviewController : Controller
    {

        private readonly UserManager<Usuario> _userManager;
        private readonly MovieDbContext _context;

        public ReviewController(UserManager<Usuario> userManager, MovieDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        // GET: ReviewController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ReviewController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReviewController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReviewController/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReviewCreateViewModel review)
        {
            try
            {
                review.UsuarioId = _userManager.GetUserId(User);

                if (ModelState.IsValid)
                {
                    var nuevaReview = new Review
                    {
                        PeliculaId = review.PeliculaId,
                        UsuarioId = review.UsuarioId,
                        Rating = review.Rating,
                        Comentario = review.Comentario,
                        FechaReview = DateTime.Now
                    };
                    _context.Reviews.Add(nuevaReview);
                    _context.SaveChanges();
                    return RedirectToAction("Details", "Home", new { id = review.PeliculaId });
                }


                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // GET: ReviewController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReviewController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReviewController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReviewController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
