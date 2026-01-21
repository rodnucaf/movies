using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using moviesMVC.Models;
using moviesMVC.Services;

namespace moviesMVC.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ImageStorage _imageStorage;
        private readonly ILogger<UsuarioController> _logger;
        public UsuarioController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, ImageStorage imageStorage, ILogger<UsuarioController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _imageStorage = imageStorage;
            _logger = logger;
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
                    ImagenUrlPerfil = "/images/default-avatar.png"
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

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
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
        public async Task<IActionResult> EditarPerfil()
        {
            var usuarioActual = await ObtenerUsuarioActualAsync();

            var usuarioVM = MapearAPerfilViewModel(usuarioActual);

            return View(usuarioVM);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfil(PerfilViewModel perfilVM)
        {

            if (!ModelState.IsValid)
            {
                return View(perfilVM);

            }

            var usuarioActual = await ObtenerUsuarioActualAsync();

            try
            {
                if (perfilVM.ImagenPerfil is not null && perfilVM.ImagenPerfil.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(usuarioActual.ImagenUrlPerfil))
                    {

                        await _imageStorage.DeleteAsync(usuarioActual.ImagenUrlPerfil);
                    }


                    var nuevaRuta = await _imageStorage.SaveAsync(usuarioActual.Id, perfilVM.ImagenPerfil);
                    usuarioActual.ImagenUrlPerfil = nuevaRuta;
                    perfilVM.ImagenUrlPerfil = nuevaRuta;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(perfilVM);
            }

            usuarioActual.Nombre = perfilVM.Nombre;
            usuarioActual.Apellido = perfilVM.Apellido;
            usuarioActual.Email = perfilVM.Email;
            usuarioActual.UserName = perfilVM.Email;

            var resultado = await _userManager.UpdateAsync(usuarioActual);

            if (resultado.Succeeded)
            {
                //await _signInManager.RefreshSignInAsync(usuarioActual);
                return RedirectToAction("Perfil", "Usuario");
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogWarning("Error updating user {UserId}: {Error}", usuarioActual.Id, error.Description);
                }
            }



            return View(perfilVM);


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

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
