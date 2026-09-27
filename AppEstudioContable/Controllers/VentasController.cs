using AppEstudioContable.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiciosEC.Interfaces;
using ServiciosEC.Managers;
using ServiciosEC.Models;
using ServiciosEC.Utilidades;
using ServiciosEC.Utilidades.ModelosDTO;
using System.Diagnostics;
using System.Threading;
using System.Linq; 
using ServiciosEC.Interfaces.Managers;


namespace AppEstudioContable.Controllers
{

    public class VentasController : Controller
    {

        private readonly IVentaManager _ventaManager;
        private readonly IClienteManager _clienteManager;
        private readonly EstadoManager _estadoManager;
        private readonly PersonaManager _personaManager;

        public VentasController(IVentaManager ventaManager, IClienteManager clienteManager, EstadoManager estadoManager, PersonaManager personaManager)
        {
            _ventaManager = ventaManager;
            _clienteManager = clienteManager;
            _estadoManager = estadoManager;
            _personaManager = personaManager;
        }

        // GET: VentasController
        [HttpGet]
        [Route("{controller}/lista/{cuit?}/{ano?}/{mes?}")]
        public async Task<ActionResult> Index(string? cuit = null, int mes = 0, int ano = 0, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(cuit))
                return View("Error", new ErrorViewModel { Message = "Debe elegir un cliente para ver las ventas." });

            var cliente = await _clienteManager.ObtenerClientePorCuitAsync(cuit, cancellationToken);
            if (cliente == null)
                return View("Error", new ErrorViewModel { Message = $"No existe cliente con el CUIT: {cuit}" });


            ViewBag.Cuit = cuit;
            ViewBag.MesSeleccionado = mes;
            ViewBag.AnoSeleccionado = ano;
            ViewBag.Id = cliente.IdPersona;
            ViewBag.RazonSocial = cliente.RazonSocial;

            var meses = new[]
            {
                "Todos los meses", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            };

            ViewBag.Meses = meses
                .Select((m, i) => new SelectListItem
                {
                    Value = i.ToString(),
                    Text = m,
                    Selected = (i == mes)
                })
                .ToList();

            var anos = Enumerable.Range(DateTime.Now.Year - 5, 7)
                                 .OrderByDescending(x => x)
                                 .Select(y => new SelectListItem
                                 {
                                     Value = y.ToString(),
                                     Text = y.ToString(),
                                     Selected = (y == ano)
                                 }).ToList();

            anos.Insert(0, new SelectListItem { Value = "0", Text = "Todos los años", Selected = (ano == 0) });
            ViewBag.Anos = anos;

            if (ano != 0)
            {
                var listaVentas = await _ventaManager.ObtenerVentasPorClienteYPeriodoAsync(cliente.IdPersona, mes, ano, cancellationToken);

                return View(listaVentas);
            }
            else
            {
                return View();
            }
        }



        // POST: VentasController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string cuit, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(cuit))
            {
                ModelState.AddModelError("", "El CUIT del cliente es necesario para cargar las ventas.");
                return RedirectToAction(nameof(Index), new { cuit = cuit });
            }


            //buscar cliente por id
            Cliente cliente = await _clienteManager.ObtenerClientePorCuitAsync(cuit, cancellationToken);
            if (cliente == null)
            {
                ModelState.AddModelError("", $"No se encontró cliente con CUIT: {cuit}");
                return RedirectToAction(nameof(Index), new { cuit = cuit });
            }
            return RedirectToAction(nameof(Index), new { cuit = cuit });

        }


        

        // GET: VentasController/Details/5
        public async Task<IActionResult> Details(int id, string cuit)
        {
            var venta = await _ventaManager.ObtenerPorId(id, default);
            if (venta == null)
            {
                return NotFound();
            }
            VentaModel ventaModel = new VentaModel
            {
                IdVenta = venta.IdVenta,
                Fecha = venta.Fecha,
                TipoFact = venta.TipoFact,
                PuntoVenta = venta.PuntoVenta,
                NroDesde = venta.NroDesde,
                NroHasta = venta.NroHasta,
                TipoDocComprador = venta.TipoDocComprador,
                NroDocComprador = venta.NroDocComprador,
                DenomComprador = venta.DenomComprador,
                TipoCambio = venta.TipoCambio,
                Moneda = venta.Moneda,
                NetoGravado = venta.NetoGravado,
                NoGravado = venta.NoGravado,
                Exento = venta.Exento,
                Iva = venta.Iva,
                Grav0 = venta.Grav0,
                Grav25 = venta.Grav25,
                Grav5 = venta.Grav5,
                Grav105 = venta.Grav105,
                Grav21 = venta.Grav21,
                Grav27 = venta.Grav27,
                Iva0 = venta.Iva0,
                Iva25 = venta.Iva25,
                Iva5 = venta.Iva5,
                Iva105 = venta.Iva105,
                Iva21 = venta.Iva21,
                Iva27 = venta.Iva27,
                Total = venta.Total,
                id = venta.IdPersona
            };
            ViewBag.Cuit = cuit;


            return View(ventaModel);
        }

        // GET: VentasController/Create
        public async Task<IActionResult> Create(int id, CancellationToken cancellationToken)
        {
            ViewBag.id = id;
            var model = new VentaModel
            {
                id = id,
            };
            return View(model);
        }

        // POST: VentasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( VentaModel ventaModel, int id, CancellationToken cancellationToken) // <-- CAMBIADO DE Venta A VentaModel y añadido EstadoId en Bind
        {
            try
            {   
                var clienteAsignado = await _clienteManager.ObtenerPorId(id, cancellationToken);
                if (clienteAsignado == null)
                {
                    ModelState.AddModelError("id", "Cliente no encontrado. Por favor, verifique el id.");

                    ViewData["id"] = new SelectList(await _clienteManager.ObtenerTodosAsync(cancellationToken), "id", "Denominacion", ventaModel.id);
                    ViewData["EstadoId"] = new SelectList(await _estadoManager.ObtenerTodosAsync(cancellationToken), "IdEstado", "Descripcion", ventaModel.EstadoId);
                    return View(ventaModel);
                }

                // Mapea VentaModel a la entidad Venta antes de insertar
                var venta = new Venta
                {
                    Fecha = ventaModel.Fecha,
                    TipoFact = ventaModel.TipoFact,
                    PuntoVenta = ventaModel.PuntoVenta,
                    NroDesde = ventaModel.NroDesde,
                    NroHasta = ventaModel.NroHasta,
                    TipoDocComprador = ventaModel.TipoDocComprador,
                    NroDocComprador = ventaModel.NroDocComprador,
                    DenomComprador = ventaModel.DenomComprador,
                    TipoCambio = ventaModel.TipoCambio,
                    Moneda = ventaModel.Moneda,
                    NetoGravado = ventaModel.NetoGravado,
                    NoGravado = ventaModel.NoGravado,
                    Exento = ventaModel.Exento,
                    Iva = ventaModel.Iva,
                    Total = ventaModel.Total,
                    IdPersona = clienteAsignado.IdPersona,
                    EstadoId = ventaModel.EstadoId,
                    Iva0 = ventaModel.Iva0,
                    Iva25=ventaModel.Iva25,
                    Iva5=ventaModel.Iva5,
                    Iva105=ventaModel.Iva105,
                    Iva21=ventaModel.Iva21,
                    Iva27=ventaModel.Iva27,
                    Grav0 = ventaModel.Grav0,
                    Grav25 = ventaModel.Grav25,
                    Grav5 = ventaModel.Grav5,
                    Grav105 = ventaModel.Grav105,
                    Grav21 = ventaModel.Grav21,
                    Grav27 = ventaModel.Grav27

                };

                // 👇 CÁLCULO AUTOMÁTICO EN EL SERVIDOR
                venta.Iva0 = (venta.Grav0 ?? 0) * 0m;
                venta.Iva25 = (venta.Grav25 ?? 0) * 0.025m;
                venta.Iva5 = (venta.Grav5 ?? 0) * 0.05m;
                venta.Iva105 = (venta.Grav105 ?? 0) * 0.105m;
                venta.Iva21 = (venta.Grav21 ?? 0) * 0.21m;
                venta.Iva27 = (venta.Grav27 ?? 0) * 0.27m;

                venta.NetoGravado = (venta.Grav0 ?? 0) + (venta.Grav25 ?? 0)
                                  + (venta.Grav5 ?? 0) + (venta.Grav105 ?? 0)
                                  + (venta.Grav21 ?? 0) + (venta.Grav27 ?? 0);

                venta.Iva = (venta.Iva0 ?? 0) + (venta.Iva25 ?? 0)
                          + (venta.Iva5 ?? 0) + (venta.Iva105 ?? 0)
                          + (venta.Iva21 ?? 0) + (venta.Iva27 ?? 0);

                venta.Total = (venta.NetoGravado ?? 0) + (venta.NoGravado ?? 0)
                            + (venta.Exento ?? 0) + (venta.Iva ?? 0);
                // 👆 FIN DEL CÁLCULO

                ViewBag.id = id;
                if (!await _ventaManager.ValidarTotales(venta, cancellationToken))
                {
                    ModelState.AddModelError(string.Empty, "Los totales de la venta no son válidos. Verifique los montos ingresados.");
                    return View(ventaModel);
                }

                if (!await _ventaManager.ValidacionIvaGravadoDeglosado(venta, cancellationToken))
                {
                    ModelState.AddModelError(string.Empty, "Los montos de Gravado y de IVA no coinciden. Verifique los montos ingresados.");
                    return View(ventaModel);
                }

                await _ventaManager.Insertar(venta, cancellationToken);

                TempData["SuccessMessage"] = "¡Comprobante creado!";
                TempData["SuccessDetail"] = "Se guardó correctamente.";
                return RedirectToAction("Altas", new { id = id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error inesperado: {ex.Message}");
                ViewBag.id = id;

                ViewData["id"] = new SelectList(await _clienteManager.ObtenerTodosAsync(cancellationToken), "id", "Denominacion", ventaModel.id);
                ViewData["EstadoId"] = new SelectList(await _estadoManager.ObtenerTodosAsync(cancellationToken), "IdEstado", "Descripcion", ventaModel.EstadoId);
                return View(ventaModel);
            }
        }

        // GET: VentasController/Edit/5
        public async Task<IActionResult> Edit(int id, string cuit, CancellationToken cancellationToken)
        {
            var venta = await _ventaManager.ObtenerPorId(id, cancellationToken);
            if (venta == null)
            {
                return NotFound();
            }

            VentaModel ventaModel = new VentaModel
            {
                IdVenta = venta.IdVenta,
                Fecha = venta.Fecha,
                TipoFact = venta.TipoFact,
                PuntoVenta = venta.PuntoVenta,
                NroDesde = venta.NroDesde,
                NroHasta = venta.NroHasta,
                TipoDocComprador = venta.TipoDocComprador,
                NroDocComprador = venta.NroDocComprador,
                DenomComprador = venta.DenomComprador,
                TipoCambio = venta.TipoCambio,
                Moneda = venta.Moneda,
                Grav0 = Math.Round(venta.Grav0 ?? 0, 2),
                Grav25 = Math.Round(venta.Grav25 ?? 0, 2),
                Grav5 = Math.Round(venta.Grav5 ?? 0, 2),
                Grav105 = Math.Round(venta.Grav105 ?? 0, 2),
                Grav21 = Math.Round(venta.Grav21 ?? 0, 2),
                Grav27 = Math.Round(venta.Grav27 ?? 0, 2),
                Iva0 = Math.Round(venta.Iva0 ?? 0, 2),
                Iva25 = Math.Round(venta.Iva25 ?? 0, 2),
                Iva5 = Math.Round(venta.Iva5 ?? 0, 2),
                Iva105 = Math.Round(venta.Iva105 ?? 0, 2),
                Iva21 = Math.Round(venta.Iva21 ?? 0, 2),
                Iva27 = Math.Round(venta.Iva27 ?? 0, 2),
                NetoGravado = Math.Round(venta.NetoGravado ?? 0, 2),
                NoGravado = Math.Round(venta.NoGravado ?? 0, 2),
                Exento = Math.Round(venta.Exento ?? 0, 2),
                Iva = Math.Round(venta.Iva ?? 0, 2),
                Total = Math.Round(venta.Total ?? 0, 2),
                id = venta.IdPersona,
                EstadoId = venta.EstadoId
            };



            ViewBag.Cuit = cuit;
            ViewBag.id = ventaModel.id;
            return View(ventaModel);
        }

        // POST: VentasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VentaModel model, int IdCliente, string cuit, CancellationToken cancellationToken)
        {
            try
            {
                var venta = await _ventaManager.ObtenerPorId(model.IdVenta, cancellationToken);
                if (venta == null)
                {
                    return NotFound();
                }
                
                venta.IdVenta = model.IdVenta;
                venta.Fecha = model.Fecha;
                venta.TipoFact = model.TipoFact;
                venta.PuntoVenta = model.PuntoVenta;
                venta.NroDesde = model.NroDesde;
                venta.NroHasta = model.NroHasta;
                venta.TipoDocComprador = model.TipoDocComprador;
                venta.NroDocComprador = model.NroDocComprador;
                venta.DenomComprador = model.DenomComprador;
                venta.TipoCambio = model.TipoCambio;
                venta.Moneda = model.Moneda;
                venta.NetoGravado = model.NetoGravado;
                venta.NoGravado = model.NoGravado;
                venta.Exento = model.Exento;
                venta.Iva = model.Iva;
                venta.Iva0 = model.Iva0;
                venta.Iva25 = model.Iva25;
                venta.Iva5 = model.Iva5;
                venta.Iva105 = model.Iva105;
                venta.Iva21 = model.Iva21;
                venta.Iva27 = model.Iva27;
                venta.Grav0 = model.Grav0;
                venta.Grav25 = model.Grav25;
                venta.Grav5 = model.Grav5;
                venta.Grav105 = model.Grav105;
                venta.Grav21 = model.Grav21;
                venta.Grav27 = model.Grav27;
                venta.Total = model.Total;
                venta.IdPersona = IdCliente;

                // 👇 CÁLCULO AUTOMÁTICO EN EL SERVIDOR
                venta.Iva0 = (venta.Grav0 ?? 0) * 0m;
                venta.Iva25 = (venta.Grav25 ?? 0) * 0.025m;
                venta.Iva5 = (venta.Grav5 ?? 0) * 0.05m;
                venta.Iva105 = (venta.Grav105 ?? 0) * 0.105m;
                venta.Iva21 = (venta.Grav21 ?? 0) * 0.21m;
                venta.Iva27 = (venta.Grav27 ?? 0) * 0.27m;

                venta.NetoGravado = (venta.Grav0 ?? 0) + (venta.Grav25 ?? 0)
                                  + (venta.Grav5 ?? 0) + (venta.Grav105 ?? 0)
                                  + (venta.Grav21 ?? 0) + (venta.Grav27 ?? 0);

                venta.Iva = (venta.Iva0 ?? 0) + (venta.Iva25 ?? 0)
                          + (venta.Iva5 ?? 0) + (venta.Iva105 ?? 0)
                          + (venta.Iva21 ?? 0) + (venta.Iva27 ?? 0);

                venta.Total = (venta.NetoGravado ?? 0) + (venta.NoGravado ?? 0)
                            + (venta.Exento ?? 0) + (venta.Iva ?? 0);
                // 👆 FIN DEL CÁLCULO


                if (!await _ventaManager.ValidarTotales(venta, cancellationToken))
                {
                    ModelState.AddModelError("Error", "Los totales de la venta no son válidos. Verifique los montos ingresados.");
                    ViewBag.IdCliente = IdCliente;
                    return View(model);
                }

                if (!await _ventaManager.ValidacionIvaGravadoDeglosado(venta, cancellationToken))
                {
                    ModelState.AddModelError("Error", "Los montos de IVA gravado y de IVA desglosado no coinciden. Verifique los montos ingresados.");
                    ViewBag.IdCliente = IdCliente;
                    return View(model);
                }

                ViewBag.Cuit = cuit;
                await _ventaManager.Editar(venta, cancellationToken);

                TempData["SuccessMessage"] = "¡Comprobante actualizado!";
                TempData["SuccessDetail"] = "Los cambios se guardaron correctamente.";

                return RedirectToAction(nameof(Altas), new { id = IdCliente ,cuit = cuit });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al editar la venta: {ex.Message}");
                ViewBag.id = IdCliente;
                ViewBag.Cuit = cuit;
                return View(model);
            }
        }



        [HttpPost]
        public async Task<IActionResult> Edit2(VentaModel model, CancellationToken cancellationToken)
        {
            try
            {
                var venta = await _ventaManager.ObtenerPorId(model.IdVenta, cancellationToken);
                if (venta == null)
                {
                    return NotFound();
                }

                venta.IdVenta = model.IdVenta;
                venta.Fecha = model.Fecha;
                venta.TipoFact = model.TipoFact;
                venta.PuntoVenta = model.PuntoVenta;
                venta.NroDesde = model.NroDesde;
                venta.NroHasta = model.NroHasta;
                venta.TipoDocComprador = model.TipoDocComprador;
                venta.NroDocComprador = model.NroDocComprador;
                venta.DenomComprador = model.DenomComprador;
                venta.TipoCambio = model.TipoCambio;
                venta.Moneda = model.Moneda;
                venta.NetoGravado = model.NetoGravado;
                venta.NoGravado = model.NoGravado;
                venta.Exento = model.Exento;
                venta.Iva = model.Iva;
                venta.Iva0 = model.Iva0;
                venta.Iva25 = model.Iva25;
                venta.Iva5 = model.Iva5;
                venta.Iva105 = model.Iva105;
                venta.Iva21 = model.Iva21;
                venta.Iva27 = model.Iva27;
                venta.Grav0 = model.Grav0;
                venta.Grav25 = model.Grav25;
                venta.Grav5 = model.Grav5;
                venta.Grav105 = model.Grav105;
                venta.Grav21 = model.Grav21;
                venta.Grav27 = model.Grav27;
                venta.Total = model.Total;

                await _ventaManager.Editar(venta, cancellationToken);

                TempData["MensajeExito"] = "Venta editada correctamente.";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al editar la venta: {ex.Message}" });
            }

        }




        // GET: VentasController/Delete/5
        public async Task<IActionResult> Delete(int id, string cuit, CancellationToken cancellationToken)
        {
            var venta = await _ventaManager.ObtenerPorId(id, cancellationToken);
            if (venta == null)
            {
                return NotFound();
            }
            ViewBag.Cuit = cuit;
            return View(venta);
        }

        // POST: VentasController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string cuit, CancellationToken cancellationToken)
        {
            try
            {
                await _ventaManager.Borrar(id, cancellationToken);
                TempData["SuccessMessage"] = "¡Comprobante eliminado!";
                TempData["SuccessDetail"] = "Se borró correctamente.";
                return RedirectToAction(nameof(Index), new { cuit = cuit });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al eliminar la venta: {ex.Message}");
                ViewBag.Cuit = cuit;
                var venta = await _ventaManager.ObtenerPorId(id, cancellationToken);
                return View("Delete", venta);
            }
        }

        /// <summary>
        /// Metodo en desuso.
        /// Muestra totales de IVA por cliente y un unico tipo de factura.
        /// </summary>
        /// <param name="cuit"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        //[Route("{controller}/{action}/{id}")]
        public async Task<IActionResult> VerTotales2(string cuit, string tipo = "")
        {
            var cliente = await _clienteManager.ObtenerClientePorCuitAsync(cuit, default);

            if (cliente == null)
                throw new Exception("Cliente no encontrado");

            var ventas = _ventaManager
                .ObtenerPorClienteYTipoFact(cliente, tipo);

            var totalesIVA = Calculadora.CalcularTotales(ventas);

            ViewBag.Cuit = cuit;
            return View(totalesIVA);
        }

        /// <summary>
        /// Permite ver los totales de IVA por cliente, mes y año.
        /// Si el mes es 0, se consideran todas las ventas del año.
        /// </summary>
        /// <param name="cuit"></param>
        /// <param name="mes">Si el mes es 0, se consideran todas las ventas del año.</param>
        /// <param name="año"></param>
        /// <returns></returns>
        [Route("{controller}/{action}/{cuit?}/{año?}/{mes?}")]
        public async Task<IActionResult> VerTotales(string cuit, int mes = 0, int año = 0)
        {
            try
            {
                var cliente = await _clienteManager.ObtenerClientePorCuitAsync(cuit, default);

                if (cliente == null)
                    throw new Exception($"No se encontró ningún cliente con el id {cuit}");

                var ventas = _ventaManager.ObtenerPorClienteAgrupadas(cliente, mes, año);

                var totalesIVA = Calculadora.CalcularTotales(ventas);

                ViewBag.Cuit = cuit;
                ViewBag.MesSeleccionado = mes;
                ViewBag.AnoSeleccionado = año;
                return View(totalesIVA);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return View("Error", new ErrorViewModel { Message = ex.Message });
            }
        }

        [Route("{controller}/{action}/{cuit?}/{año?}/{mes?}")]
        public async Task<IActionResult> VerNeto(string cuit, int mes = 0, int año = 0, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(cuit))
                {
                    ModelState.AddModelError("", "El CUIT es obligatorio para ver el neto gravado.");

                    return View(new VerNetoViewModel { Cuit = "", Mes = DateTime.Now.Month, Año = DateTime.Now.Year, NetoGravado = 0 });
                }

                var cliente = await _clienteManager.ObtenerClientePorCuitAsync(cuit, cancellationToken);

                if (cliente == null)
                {
                    ModelState.AddModelError("", $"No se encontró ningún cliente con el id {cuit}.");

                    return View(new VerNetoViewModel { Cuit = cuit, Mes = DateTime.Now.Month, Año = DateTime.Now.Year, NetoGravado = 0 });
                }


                int mesParaCalculo = mes;
                int añoParaCalculo = año;


                if (mesParaCalculo < 1 || mesParaCalculo > 12)
                {
                    mesParaCalculo = DateTime.Now.Month;
                }

                if (añoParaCalculo < 1900 || añoParaCalculo > 2200)
                {
                    añoParaCalculo = DateTime.Now.Year;
                }


                var neto = _ventaManager.ObtenerNetoGravadoVentas(cliente, mesParaCalculo, añoParaCalculo);

                var model = new VerNetoViewModel
                {
                    Cuit = cuit,
                    Mes = mesParaCalculo,
                    Año = añoParaCalculo,
                    NetoGravado = neto
                };

                ViewBag.Cuit = cuit;
                ViewBag.MesSeleccionado = mesParaCalculo;
                ViewBag.AnoSeleccionado = añoParaCalculo;

                return View(model);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ModelState.AddModelError(string.Empty, $"Ocurrió un error al obtener el neto gravado: {ex.Message}");

                return View(new VerNetoViewModel { Cuit = cuit, Mes = DateTime.Now.Month, Año = DateTime.Now.Year, NetoGravado = 0 });
            }
        }


        [HttpGet]
        public async Task<IActionResult> VerTotalNeto(string cuit, int? mes, int? ano, CancellationToken cancellationToken)
        {
            TempData["InfoMessage"] = $"Total Neto solicitado para CUIT: {cuit}, Mes: {mes ?? 0}, Año: {ano ?? 0}. (Lógica de cálculo pendiente)";
            return RedirectToAction(nameof(Index), new { cuit = cuit, mes = mes, ano = ano });
        }


        [HttpGet]
        [Route("{controller}/{action}/{id?}/{ano?}/{mes?}")]
        public async Task<IActionResult> VerTotalesComprobante(string cuit, int? ano, int? mes, CancellationToken cancellationToken)
        {

            if (string.IsNullOrEmpty(cuit))
            {
                ModelState.AddModelError("", "El CUIT es obligatorio para ver los totales de comprobantes.");
                return RedirectToAction(nameof(Index), new { cuit = cuit });
            }


            int añoParaCalculo = ano ?? DateTime.Now.Year;
            int mesParaCalculo = mes ?? DateTime.Now.Month;


            if (mesParaCalculo < 1 || mesParaCalculo > 12)
            {
                mesParaCalculo = DateTime.Now.Month;
            }
            if (añoParaCalculo < 1900 || añoParaCalculo > 2200)
            {
                añoParaCalculo = DateTime.Now.Year;
            }

            TempData["InfoMessage"] = $"Totales por Comprobante solicitados para CUIT: {cuit}, Mes: {mesParaCalculo}, Año: {añoParaCalculo}. (Lógica de cálculo pendiente)";


            return RedirectToAction(nameof(Index), new { cuit = cuit, mes = mesParaCalculo, ano = añoParaCalculo });
        }


        [HttpGet]
        public async Task<ActionResult> Altas(int id, CancellationToken cancellationToken = default)
        {
            
            VentaViewModel model = new VentaViewModel
            {
                id = id,
            };

            var cliente = await _clienteManager.ObtenerPorId(id, cancellationToken);
            if (cliente == null)
            {
                TempData["ErrorMessage"] = $"No se encontró cliente con id: {id}";
                return View(model);
            }

            ViewBag.RazonSocial = cliente.RazonSocial;


            return View(model);
        }



        // POST: VentasController
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Altas(VentaViewModel model, IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment, CancellationToken cancellationToken)
        {

            var id = model.id;

            var cliente = await _clienteManager.ObtenerPorId(id, cancellationToken);

            if (cliente == null)
            {
                TempData["ErrorMessage"] = $"No se encontró cliente con id: {id}";
                return View(model);
            }

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Por favor, selecciona un archivo para cargar.");
                return View(model);
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var extensionesPermitidas = new[] { ".xlsx", ".xls", ".csv" };

            if (!extensionesPermitidas.Contains(extension))
            {
                ModelState.AddModelError("", "Solo se permiten archivos con extensión .xlsx, .xls o .csv");
                return View(model);
            }

            try
            {
                
                using var stream = file.OpenReadStream();

                
                var (ventasCorrectas, ventasParaRevisar, ventasFallidas, excelValido) = await _ventaManager.AgregarVentas(stream, id, cliente.Cuit, cancellationToken);

                model.ventasCorrectas = ventasCorrectas.ToList();
                model.ventasParaRevisar = ventasParaRevisar.ToList();
                model.ventasFallidas = ventasFallidas.ToList();

                if (!excelValido)
                {
                    TempData["ErrorMessage"] = "Error: Cuit incorrecto o excel no corresponde a Ventas";
                    return View(model);
                }

                if (!ventasCorrectas.Any() && !ventasParaRevisar.Any())
                {
                    TempData["ErrorMessage"] = "No hay Ventas para agregar.";
                    return View(model);
                }

                TempData["SuccessMessage"] = "Ventas cargadas exitosamente.";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ocurrió un error al procesar el archivo: {ex.Message}";
                return View(model);
            }

            

        }



    }
}