using ServiciosEC.Models;
using ServiciosEC.Utilidades.ModelosDTO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiciosEC.Interfaces.Managers
{
    public interface ICompraManager : IManager<Compra>
    {
        
        Task<(IEnumerable<Compra> comprasCorrectas, IEnumerable<Compra> comprasParaRevisar, IEnumerable<Compra> comprasFallidas, bool excelValido)> AgregarCompras(Stream stream, int idPersona, string cuit, List<string> tiposFacturaExcluir, CancellationToken cancellationToken);
        Task<IEnumerable<Compra>> ObtenerTodasLasComprasPorFechas(DateOnly fechaDesde, DateOnly fechaHasta, CancellationToken cancellationToken);
        Task<bool> ExisteCompraEnFecha(DateOnly periodo, CancellationToken cancellationToken);
        Task<(TotalesIVA, TotalesIVA)> ObtenerIVAPorClienteYPeriodo(Cliente cliente, int mes, int ano);
        Task<ResumenIIGGMensual> ObtenerIVAMensualPorCliente(Cliente cliente, int año);
        Task<List<Compra>> ObtenerComprasPorClientePeriodoAsync(int idPersona, int mes, int ano, CancellationToken cancellationToken);
        Task<bool> ValidarTotales(Compra compraNueva, CancellationToken cancellationToken);
        Task<bool> ValidacionIvaGravadoDesglosado(Compra compraNueva, CancellationToken cancellationToken);
        Task<Dictionary<string, TotalesIVA>> ObtenerTotalesPorTipoComprobante(Cliente cliente, int mes, int ano);
    }
}
