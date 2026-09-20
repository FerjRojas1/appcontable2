using Microsoft.EntityFrameworkCore;
using ServiciosEC.Interfaces;
using ServiciosEC.Models;
using ServiciosEC.Utilidades;
using ServiciosEC.Utilidades.ModelosDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading; 
using System.Threading.Tasks;
using ServiciosEC.Interfaces.Managers;



namespace ServiciosEC.Managers
{
    public class CompraManager : ICompraManager
    {

        private IExcelDataHandler _excelDataHandler;
        private readonly ECContext _context;

        public CompraManager(ECContext context, IExcelDataHandler excelDataHandler)
        {
            _excelDataHandler = excelDataHandler;
            _context = context;
        }

        public async Task<(IEnumerable<Compra> comprasCorrectas, IEnumerable<Compra> comprasParaRevisar, IEnumerable<Compra> comprasFallidas, bool excelValido)> AgregarCompras(Stream stream, int idPersona, string cuit, List<string> tiposFacturaExcluir, CancellationToken cancellationToken)
        {
            var (list, primeraCelda) = _excelDataHandler.ImportarCompras(stream, idPersona);

            bool excelValido = true;
            List<Compra> comprasParaRevisar = new List<Compra>();
            List<Compra> comprasCorrectas = new List<Compra>();
            List<Compra> comprasFallidas = new List<Compra>();
            if (primeraCelda.Contains("Compras", StringComparison.OrdinalIgnoreCase) && primeraCelda.Contains($"{cuit}", StringComparison.OrdinalIgnoreCase))
            {
                foreach (Compra compra in list)
                {

                    //Comprueba que la factura no este ya registrada en la base de datos.
                    if (await facturaExistente(compra, cancellationToken))
                    {
                        comprasFallidas.Add(compra);
                        continue;
                    }

                    //excluir facturas
                    if (facturaExcluida(compra, tiposFacturaExcluir, cancellationToken))
                    {
                        comprasFallidas.Add(compra);
                        continue;
                    }

                    if (await IvaValido(compra, cancellationToken))
                    {
                      
                        await asignarMontoIva(compra, cancellationToken);
                        comprasCorrectas.Add(compra);
                        await Insertar(compra, cancellationToken);
                        continue;
                    }
                    else
                    {
                        comprasParaRevisar.Add(compra);
                        await Insertar(compra, cancellationToken);
                        continue;
                    }
                }

                return (comprasCorrectas, comprasParaRevisar, comprasFallidas, excelValido);
            }
            else
            {
                excelValido = false;
                return (comprasCorrectas, comprasParaRevisar, comprasFallidas, excelValido);
            }
        }
        /// <summary>
        /// recibe una lista con tipos de facturas para excluir y un objeto compra, retorna true si la propiedad tipoFactura de compra coincide con un elemento
        /// de la lista para excluir.
        /// </summary>
        /// <param name="compra"></param>
        /// <param name="tiposFacturaExcluir"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private bool facturaExcluida(Compra compra, List<string> tiposFacturaExcluir, CancellationToken cancellationToken)
        {
            bool excluir = false;

            foreach (var tipoFactura in tiposFacturaExcluir)
            {
                if (compra.TipoFact.Contains(tipoFactura))
                {
                    excluir = true;
                    break;
                }
            }

            return excluir;

        }



        /// <summary>
        /// Realiza una Busqueda en base de datos de una factura en particular. Los parametros de busqueda son, id , PuntoVenta y NumeroDesde
        /// Si existe una factura que repita esos 3 valores al mismo tiempo, retorna True y si no retorna False.
        /// </summary>
        /// <param name="compra"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> facturaExistente(Compra compra, CancellationToken cancellationToken)
        {
            bool existe = false;

            var factura = await _context.Compras
                .Where(c => c.EstadoId == (int)ECContext.EstadosEnum.Activo
                    && c.IdPersona == compra.IdPersona && c.PuntoVenta == compra.PuntoVenta && c.NroDesde == compra.NroDesde)
                .FirstOrDefaultAsync(cancellationToken);

            if (factura != null)
            {
                existe = true;
            }

            return existe;
        }

        public async Task Insertar(Compra compra, CancellationToken cancellationToken)
        {
            _context.Compras.Add(compra);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Compra>> ObtenerTodos(CancellationToken cancellationToken)
        {
            return await _context.Compras
                .Where(c => c.EstadoId == (int)ECContext.EstadosEnum.Activo)
                .ToListAsync(cancellationToken);
        }

        public async Task<Compra> ObtenerPorId(int id, CancellationToken cancellationToken)
        {
            var compra = await _context.Compras
                .Where(c => c.EstadoId == (int)ECContext.EstadosEnum.Activo)
                .FirstOrDefaultAsync(c => c.IdCompra == id, cancellationToken);
            if (compra == null)
                throw new KeyNotFoundException($"Compra con ID {id} no encontrada.");
            return compra;
        }

        public Task Editar(Compra entidad, CancellationToken cancellationToken)
        {
            _context.Entry(entidad).State = EntityState.Modified;
            return _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Borrar(int id, CancellationToken cancellationToken)
        {
            var compra = await ObtenerPorId(id, cancellationToken);
            if (compra != null)
            {
                _context.Compras.Remove(compra);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }


    
        public async Task<IEnumerable<Compra>> ObtenerTodasLasComprasPorFechas(DateOnly fechaDesde, DateOnly fechaHasta, CancellationToken cancellationToken)
        {
            return await _context.Compras
                .Where(c => c.Fecha >= fechaDesde && 
                            c.Fecha <= fechaHasta && 
                            c.EstadoId == (int)ECContext.EstadosEnum.Activo)
                .ToListAsync(cancellationToken);
        }

        
        public async Task<bool> ExisteCompraEnFecha(DateOnly periodo, CancellationToken cancellationToken)
        {
            return await _context.Compras
                .AnyAsync(c => c.Fecha == periodo && 
                               c.EstadoId == (int)ECContext.EstadosEnum.Activo,
                               cancellationToken);

        }

        public async Task<(TotalesIVA, TotalesIVA)> ObtenerIVAPorClienteYPeriodo(Cliente cliente, int mes, int ano)
        {
            IQueryable<Compra> query = _context.Compras
                .Where(v => v.IdPersona == cliente.IdPersona
                    && v.Fecha.Year == ano
                    && v.EstadoId == (int)ECContext.EstadosEnum.Activo
                );


      
            if (mes != 0)
                query = query.Where(v => v.Fecha.Month == mes);


            var compras = await query.ToListAsync();

            TotalesIVA TotalCreditoFiscal = Calculadora
                .CalcularTotales(compras
                    .Where(v => !v.TipoFact.ToLower().Contains("crédito")));

            TotalesIVA TotalRestitucionCreditoFiscal = Calculadora
                .CalcularTotales(compras
                    .Where(v => v.TipoFact.ToLower().Contains("crédito")));

            return (TotalCreditoFiscal, TotalRestitucionCreditoFiscal);
        }

        public async Task<ResumenIIGGMensual> ObtenerIVAMensualPorCliente(Cliente cliente, int año)
        {
            var compras = await _context.Compras
                .Where(v => v.IdPersona == cliente.IdPersona &&
                            v.Fecha.Year == año &&
                            v.EstadoId == (int)ECContext.EstadosEnum.Activo)
                .ToListAsync();

          
            var totalesPorMes = Enumerable.Range(1, 12)
                .Select(mes =>
                {
                    var comprasDelMes = compras.Where(v => v.Fecha.Month == mes);
                    return Calculadora.CalcularTotales(comprasDelMes);
                })
                .ToList();

  
            var totalAnualCalculado = totalesPorMes.Aggregate(new TotalesIVA(), (acc, t) => acc + t);

         
            var resumen = new ResumenIIGGMensual
            {
                PorMes = totalesPorMes,
                TotalAnual = totalAnualCalculado
            };

            return resumen;
        }


        public async Task<List<Compra>> ObtenerComprasPorClientePeriodoAsync(
            int idPersona, int mes, int ano, CancellationToken cancellationToken)
        {
            IQueryable<Compra> query = _context.Compras
                .Where(c => c.IdPersona == idPersona && c.EstadoId == (int)ECContext.EstadosEnum.Activo);

            if (mes > 0)
            {
                query = query.Where(c => c.Fecha.Month == mes);
            }

            if (ano > 0)
            {
                query = query.Where(c => c.Fecha.Year == ano);
            }

            query = query.OrderByDescending(c => c.Fecha);


            List<Compra> dbCompras = await query.ToListAsync(cancellationToken);

            return dbCompras;
        }

        public async Task<int> ObtenerCantidad(CancellationToken cancellationToken)
        {
            return await _context.Compras.CountAsync(cancellationToken);
        }


        public async Task<bool> IvaValido(Compra compra, CancellationToken cancellationToken)
        {
            bool ivaValido = false;
            const decimal tolerancia = 0.1m;

            var ivasAceptables = await _context.Ivas.ToListAsync(cancellationToken);

           
            if (compra.Iva.HasValue && compra.NetoGravado.HasValue && compra.NetoGravado.Value != 0)
            {
                decimal porcentajeCalculado = (compra.Iva.Value / compra.NetoGravado.Value) * 100;

                foreach (var iva in ivasAceptables)
                {
                    if (decimal.Abs(porcentajeCalculado - iva.Porcentaje) < tolerancia)
                    {
                        ivaValido = true;
                        break;
                    }
                }
            }
            return ivaValido;
        }

  
        public async Task<Compra> asignarMontoIva(Compra compra, CancellationToken cancellationToken)
        {
            
            if (compra.Iva.HasValue && compra.NetoGravado.HasValue && compra.NetoGravado.Value != 0)
            {
                decimal porcentajeCalculado = (compra.Iva.Value / compra.NetoGravado.Value) * 100;
                var ivasAceptables = await _context.Ivas.ToListAsync(cancellationToken);

                foreach (var iva in ivasAceptables)
                {
                    if (decimal.Abs(porcentajeCalculado - iva.Porcentaje) < 0.1m)
                    {
                        
                        /*
                        compra.Iva0 = 0m; compra.Grav0 = 0m;
                        compra.Iva25 = 0m; compra.Grav25 = 0m; 
                        compra.Iva5 = 0m; compra.Grav5 = 0m;
                        compra.Iva105 = 0m; compra.Grav105 = 0m;
                        compra.Iva21 = 0m; compra.Grav21 = 0m;
                        compra.Iva27 = 0m; compra.Grav27 = 0m;
                        */
                        

                        switch (iva.Porcentaje)
                        {
                            case 0m:
                                compra.Iva0 = compra.Iva;
                                compra.Grav0 = compra.NetoGravado;
                                break;
                            case 25m: 
                                compra.Iva25 = compra.Iva;
                                compra.Grav25 = compra.NetoGravado;
                                break;
                            case 5m:
                                compra.Iva5 = compra.Iva;
                                compra.Grav5 = compra.NetoGravado;
                                break;
                            case 10.5m:
                                compra.Iva105 = compra.Iva;
                                compra.Grav105 = compra.NetoGravado;
                                break;
                            case 21m:
                                compra.Iva21 = compra.Iva;
                                compra.Grav21 = compra.NetoGravado;
                                break;
                            case 27m:
                                compra.Iva27 = compra.Iva;
                                compra.Grav27 = compra.NetoGravado;
                                break;
                        }
                        break;
                    }
                }
            }
            return compra;
        }


        public async Task<bool> ValidarTotales(Compra compra, CancellationToken cancellationToken)
        {
          
            if (compra.Total.HasValue && compra.NetoGravado.HasValue && compra.Iva.HasValue && compra.Exento.HasValue && compra.NoGravado.HasValue)
            {
                const decimal tolerancia = 0.01m; 
                bool totalValido = Math.Abs((compra.Iva.Value + compra.NetoGravado.Value + compra.Exento.Value + compra.NoGravado.Value) - compra.Total.Value) < tolerancia;

             
                return totalValido;
            }
            return false;
        }


        public async Task<bool> ValidacionIvaGravadoDesglosado(Compra compra, CancellationToken cancellationToken)
        {
          
            if (compra.Iva0.HasValue && compra.Iva25.HasValue && compra.Iva5.HasValue && compra.Iva105.HasValue && compra.Iva21.HasValue && compra.Iva27.HasValue &&
                compra.Grav0.HasValue && compra.Grav25.HasValue && compra.Grav5.HasValue && compra.Grav105.HasValue && compra.Grav21.HasValue && compra.Grav27.HasValue &&
                compra.Iva.HasValue && compra.NetoGravado.HasValue)
            {
                const decimal tolerancia = 0.01m; 

                bool IvaValido = Math.Abs((compra.Iva0.Value + compra.Iva25.Value + compra.Iva5.Value + compra.Iva105.Value + compra.Iva21.Value + compra.Iva27.Value) - compra.Iva.Value) < tolerancia;
                bool NetoValido = Math.Abs((compra.Grav0.Value + compra.Grav25.Value + compra.Grav5.Value + compra.Grav105.Value + compra.Grav21.Value + compra.Grav27.Value) - compra.NetoGravado.Value) < tolerancia;

               
                return IvaValido && NetoValido;
            }
            return false;
        }

        /// <summary>
        /// Obtiene los totales de IVA agrupados por tipo de comprobante para un cliente y período.
        /// Si mes = 0, se consideran todas las compras del año.
        /// </summary>
        public async Task<Dictionary<string, TotalesIVA>> ObtenerTotalesPorTipoComprobante(Cliente cliente, int mes, int ano)
        {
            IQueryable<Compra> query = _context.Compras
                .Where(c => c.IdPersona == cliente.IdPersona
                         && c.Fecha.Year == ano
                         && c.EstadoId == (int)ECContext.EstadosEnum.Activo);

            if (mes != 0)
                query = query.Where(c => c.Fecha.Month == mes);

            var compras = await query.ToListAsync();

            // Agrupar por tipo de comprobante y calcular totales de cada grupo
            var totales = compras
                .GroupBy(c => c.TipoFact)
                .ToDictionary(
                    g => g.Key,
                    g => Calculadora.CalcularTotales(g)
                );

            return totales;
        }

    }
}