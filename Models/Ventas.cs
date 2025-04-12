using SistemaFacturacion.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaFacturacion.Models
{
    public class Ventas
    {
        [Key]
        public int VentasId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCreacion { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Debe ingresar un nombre para el cliente")]
        public string NombreCliente { get; set; }

        [Required(ErrorMessage = "Debe ingresar un método de pago")]
        public string MetodoPago { get; set; }

        public ICollection<VentasDetalle> FacturaDetalle { get; set; } = new List<VentasDetalle>();
    }
}
