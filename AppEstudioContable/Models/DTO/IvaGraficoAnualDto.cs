using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEstudioContable.Models.DTO
{
    public class IvaGraficoAnualDto
    {
        public int Anio { get; set; }
        public decimal TotalCreditoNeto { get; set; }
        public decimal TotalDebitoNeto { get; set; }
    }
}
