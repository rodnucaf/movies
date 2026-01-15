using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace moviesMVC.Models
{
    public class Usuario : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(50)]
        public string Apellido { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        public string ImagenUrlPerfil { get; set; }
        public List<Favorito>? PeliculasFavoritas { get; set; }
        public List<Review>? ReviewsUsuario { get; set; }
    }

    public class UsuarioViewModel
    {
        [Required(ErrorMessage = "Ingrese un nombre")]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Ingrese un apellido")]
        [StringLength(50)]
        public string Apellido { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Ingrese el correo.")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Ingrese la clave.")]
        public string Clave { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Confirme la clave.")]
        [Compare("Clave", ErrorMessage = "Las claves no coinciden.")]
        public string ConfirmarClave { get; set; }
    }

    public class LoginViewModel
    {
        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        [Required(ErrorMessage = "Ingrese el correo.")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Ingrese la clave.")]
        public string Clave { get; set; }
        public bool Recordarme { get; set; }

    }

    public class PerfilViewModel
    {
        public string UsuarioId { get; set; }
        //[Required(ErrorMessage = "Tiene que haber un nombre.")]
        //[StringLength(50)]
        public string Nombre { get; set; }
        //[Required(ErrorMessage = "Tiene que haber un apellido.")]
        //[StringLength(50)]
        public string Apellido { get; set; }
        //[EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        //[Required(ErrorMessage = "Ingrese el correo.")]
        public string Email { get; set; }
        public IFormFile? ImagenPerfil { get; set; }
        public string? ImagenUrlPerfil { get; set; }

    }
}
