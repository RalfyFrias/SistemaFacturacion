using SistemaFacturacion.Models;
using System.ComponentModel.DataAnnotations;

namespace SistemaFacturacion.Models
{
    public class Reparaciones
    {
        [Key]
        public int ReparacionesId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCreacion { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Debe ingresar un nombre para el cliente")]
        public string NombreCliente { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Descripcion")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debe ingresar un método de pago")]
        public string MetodoPago { get; set; }

        [Required(ErrorMessage = "Debe ingresar un método de pago")]
        public int Pago { get; set; }

        public decimal Total { get; set; }

        public ICollection<VentasDetalle> VentasDetalles { get; set; } = new List<VentasDetalle>();
    }
}
