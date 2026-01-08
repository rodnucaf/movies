namespace moviesMVC.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int PeliculaId { get; set; }
        public Pelicula? Pelicula { get; set; }
        public string UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int Rating { get; set; }
        public DateTime FechaReview { get; set; }
        

    }
}
