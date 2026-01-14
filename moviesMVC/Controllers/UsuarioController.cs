using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using moviesMVC.Models;

namespace moviesMVC.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        public UsuarioController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UsuarioViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                var nuevoUsuario = new Usuario
                {
                    UserName = usuario.Email,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    ImagenUrlPerfil = "default-profile.png"
                };

                var resultado = await _userManager.CreateAsync(nuevoUsuario, usuario.Clave);

                if (resultado.Succeeded)
                {
                    await _signInManager.SignInAsync(nuevoUsuario, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in resultado.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(usuario);
        }
        
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel usuario)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(usuario.Email, usuario.Clave, usuario.Recordarme, lockoutOnFailure: false);
                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                }
            }
            return View(usuario);
        }

        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Perfil()
        {
            var usuarioActual = await ObtenerUsuarioActualAsync();

            if (usuarioActual == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            var usuarioVM = MapearAPerfilViewModel(usuarioActual);

            return View(usuarioVM);
        }

        [Authorize]
        public async Task<IActionResult> EditarPerfil(int usuarioId)
        {
           var usuarioActual = await ObtenerUsuarioActualAsync();
            
            var usuarioVM = MapearAPerfilViewModel(usuarioActual);

            return View(usuarioVM);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditarPerfil(PerfilViewModel perfilVM)
        {
            var usuarioActual = await ObtenerUsuarioActualAsync();

            if (ModelState.IsValid)
            {
                usuarioActual.Nombre = perfilVM.Nombre;
                usuarioActual.Apellido = perfilVM.Apellido;
                usuarioActual.Email = perfilVM.Email;
                usuarioActual.UserName = perfilVM.Email;

                var resultado = await _userManager.UpdateAsync(usuarioActual);

                if (resultado.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(usuarioActual);
                    return RedirectToAction("Perfil", "Usuario");
                }
                else
                {
                    foreach (var error in resultado.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View("EditarPerfil", perfilVM);
        }

        private async Task<Usuario> ObtenerUsuarioActualAsync()
        {

            return await _userManager.GetUserAsync(User);
        }

        private PerfilViewModel MapearAPerfilViewModel(Usuario usuario)
        {
            return new PerfilViewModel
            {
                UsuarioId = usuario.Id.ToString(),
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                ImagenUrlPerfil = usuario.ImagenUrlPerfil
            };
        }

        private bool EsUsuarioActual(Usuario usuario)
        {
            var usuarioActualId = _userManager.GetUserId(User);
            return usuario != null && usuario.Id == usuarioActualId;
        }
    }
}
