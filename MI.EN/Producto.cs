using System.ComponentModel.DataAnnotations;

namespace MI.EN
{
	public class Producto
	{
        public int IdProducto { get; set; }
        [Required, StringLength(2, MinimumLength = 1)]
        public string ClaveProducto { get; set; }
        [Required, StringLength(100, MinimumLength = 3)]
        public string ProductoDescripcion { get; set; }
        [Required, StringLength(100, MinimumLength = 3)]
        public string Comentario { get; set; }
    }
}
