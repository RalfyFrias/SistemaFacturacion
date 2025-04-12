using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SistemaFacturacion.Models;

namespace SistemaFacturacion.Models
{   
    public class Financiamiento
    {
        [Key]
        public int FinanciamientoId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCreacion { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Debe ingresar un nombre para el cliente")]
        public string NombreCliente { get; set; }

        [Required(ErrorMessage = "Debe ingresar una descripción para el producto")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debe ingresar una dirección para el cliente")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Debe ingresar un teléfono para el cliente")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Debe ingresar un email para el cliente")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe elegir una cantidad ")]
        public string FormaPago { get; set; }

        [Range(1, 1000000000000000, ErrorMessage = "Debe elegir una cantidad ")]
        public decimal CantidadPago { get; set; }

        [Range(1, 1000000000000000, ErrorMessage = "Debe elegir una cantidad ")]
        public decimal TotalPorPago { get; set; }


        [Required(ErrorMessage = "Debe elegir una cantidad ")]
        public decimal ganancia { get; set; }

        [Required(ErrorMessage = "Debe elegir una cantidad ")]
        public decimal Inicial { get; set; }
        [Required(ErrorMessage = "Debe ingresar una cédula")]
        [Column(TypeName = "nvarchar(11)")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "Debe ingresar un método de pago")]
        public string MetodoPago { get; set; }

        public ICollection<VentasDetalle> VentasDetalles { get; set; } = new List<VentasDetalle>();
    }
}
