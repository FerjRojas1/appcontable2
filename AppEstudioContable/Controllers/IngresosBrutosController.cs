using AppEstudioContable.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiciosEC.Interfaces.Managers;
using ServiciosEC.Managers;
using ServiciosEC.Models;
using System.Diagnostics;
using System.Threading;


namespace AppEstudioContable.Controllers
{
    public class IngresosBrutosController : Controller
    {

        private readonly IVentaManager _ventaManager;
        private readonly IClienteManager _clienteManager;

        public IngresosBrutosController(IVentaManager ventaManager, IClienteManager clienteManager)
        {
            _ventaManager = ventaManager;
            _clienteManager = clienteManager;
        }

        [HttpGet]
        public async Task<IActionResult> IngresosBrutos(int id, CancellationToken cancellationToken)
        {
            var meses = new[]
            {
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            };
            ViewBag.Periodos = meses
                .Select((mes, index) => new SelectListItem
                {
                    Value = (index + 1).ToString(), // 1 a 12
                    Text = mes
                })
                .ToList();

            var jurisdicciones = new[]
                {
                "Ciudad Autónoma de Buenos Aires", "Buenos Aires", "Catamarca", "Chaco", "Chubut",
                "Córdoba", "Corrientes", "Entre Ríos", "Formosa", "Jujuy", "La Pampa", "La Rioja",
                "Mendoza", "Misiones", "Neuquén", "Río Negro", "Salta", "San Juan", "San Luis",
                "Santa Cruz", "Santa Fe", "Santiago del Estero",
                "Tierra del Fuego, Antártida e Islas del Atlántico Sur", "Tucumán"
                };

            ViewBag.jurisdicciones = jurisdicciones
                .Select((jurisdiccion, index) => new SelectListItem
                {
                    Value = (index + 1).ToString(), // 1 a 24
                    Text = jurisdiccion
                })
                .ToList();

            


            var model = new IngresosBrutosModel { id = id };

            var cliente = await _clienteManager.ObtenerPorId(model.id, cancellationToken);

            ViewBag.Cuit = cliente.Cuit;

            ViewBag.RazonSocial = cliente.RazonSocial;

            if (cliente == null)
            {
                TempData["ErrorMessage"] = $"Error: No se encontró cliente con id: {id}";
                return View(model);
            }
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IngresosBrutos(IngresosBrutosModel model, CancellationToken cancellationToken)
        {
            try
            {
                var meses = new[]
                    {
                        "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
                };

                ViewBag.Periodos = meses
                    .Select((mes, index) => new SelectListItem
                    {
                        Value = (index + 1).ToString(),
                        Text = mes
                    })
                    .ToList();

                var jurisdicciones = new[]
                    {
                        "Ciudad Autónoma de Buenos Aires", "Buenos Aires", "Catamarca", "Chaco", "Chubut",
                        "Córdoba", "Corrientes", "Entre Ríos", "Formosa", "Jujuy", "La Pampa", "La Rioja",
                        "Mendoza", "Misiones", "Neuquén", "Río Negro", "Salta", "San Juan", "San Luis",
                        "Santa Cruz", "Santa Fe", "Santiago del Estero",
                        "Tierra del Fuego, Antártida e Islas del Atlántico Sur", "Tucumán"
                };

                ViewBag.jurisdicciones = jurisdicciones
                    .Select((jurisdiccion, index) => new SelectListItem
                    {
                        Value = (index + 1).ToString(), // 1 a 24
                        Text = jurisdiccion
                    })
                    .ToList();


                if (!ModelState.IsValid)
                {
                    return View("IngresosBrutos", model);
                }
                

                Ingresosbrutos ingresosBrutos = new Ingresosbrutos
                {
                    IdPersona = model.id,
                    Periodo = model.Periodo,
                    Anio = model.Anio,
                    JurisdiccionId = model.JurisdiccionId,
                    Coeficiente = model.Coeficiente,
                    Alicuota = model.Alicuota,
                    Retenciones = model.Retenciones,
                    RetencionesBancarias = model.RetencionesBancarias,
                    Percepciones = model.Percepciones,
                    Aduaneras = model.Aduaneras,

                };

                var cliente = await _clienteManager.ObtenerPorId(model.id, cancellationToken);

                if (cliente == null)
                {
                    TempData["ErrorMessage"] = $"Error: No se encontró cliente con id: {model.id}";
                    return View(model);
                }

                //ingresosBrutos.GravadoPais = _ventaManager.ObtenerNetoGravadoVentas(cliente, model.Periodo, model.Anio);
                var netoVentas = _ventaManager.ObtenerNetoGravadoVentas(cliente, model.Periodo, model.Anio);
                //Valida que haya ventas en el periodo dado
                if (netoVentas == 0)
                {
                    TempData["ErrorMessage"] = $"Error: No hay ventas cargadas para el periodo {model.Periodo}/{model.Anio}.";
                    return View("IngresosBrutos", model);
                }

                //obtener el gravado pais
                var (TotalDebitoFiscal, TotalRestitucionDebitoFiscal) = await _ventaManager.ObtenerIVAPorClienteYPeriodo(cliente, model.Periodo, model.Anio);

                var netoDebitoFiscal = 
                        TotalDebitoFiscal.Neto0 + TotalDebitoFiscal.Neto27 + 
                        TotalDebitoFiscal.Neto21 +  TotalDebitoFiscal.Neto105 + 
                        TotalDebitoFiscal.Neto25 + TotalDebitoFiscal.Neto5;
                
                var netoRestitucionDebitoFiscal =
                        TotalRestitucionDebitoFiscal.Neto0 + TotalRestitucionDebitoFiscal.Neto27 + 
                        TotalRestitucionDebitoFiscal.Neto21 + TotalRestitucionDebitoFiscal.Neto105 + 
                        TotalRestitucionDebitoFiscal.Neto25 + TotalRestitucionDebitoFiscal.Neto5;

                ingresosBrutos.GravadoPais = netoDebitoFiscal - netoRestitucionDebitoFiscal;

                Debug.WriteLine(ingresosBrutos.GravadoPais);

                var coeficienteAcumulado = await _ventaManager.ObtenerCoeficienteAcumulado(ingresosBrutos,cancellationToken);

                //validar que el acumulado de coeficientes mas el coef ingresado no pueda ser mayor a 1.
                if (coeficienteAcumulado + ingresosBrutos.Coeficiente > 1)
                {

                    TempData["ErrorMessage"] = $"Error: Coeficiente mensual excedido";
                    return View("IngresosBrutos", model);
                }

                await _ventaManager.GenerarIngresosBrutosMensual(ingresosBrutos, cliente, cancellationToken);
                TempData["SuccessMessage"] = "Ingresos Brutos generados/actualizados correctamente.";

                //return View("IngresosBrutos", model);
                return RedirectToAction("IngresosBrutos", model);


            }
            catch (Exception ex)
            {
                
                TempData["ErrorMessage"] = "Ocurrió un error al procesar la solicitud. Intenta nuevamente. "+ex.Message;

                var meses = new[]
                {
                    "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
                };

                ViewBag.Periodos = meses
                    .Select((mes, index) => new SelectListItem
                    {
                        Value = (index + 1).ToString(),
                        Text = mes
                    })
                    .ToList();

                var jurisdicciones = new[]
                        {
                        "Ciudad Autónoma de Buenos Aires", "Buenos Aires", "Catamarca", "Chaco", "Chubut",
                        "Córdoba", "Corrientes", "Entre Ríos", "Formosa", "Jujuy", "La Pampa", "La Rioja",
                        "Mendoza", "Misiones", "Neuquén", "Río Negro", "Salta", "San Juan", "San Luis",
                        "Santa Cruz", "Santa Fe", "Santiago del Estero",
                        "Tierra del Fuego, Antártida e Islas del Atlántico Sur", "Tucumán"
                        };

                ViewBag.jurisdicciones = jurisdicciones
                    .Select((jurisdiccion, index) => new SelectListItem
                    {
                        Value = (index + 1).ToString(), // 1 a 24
                        Text = jurisdiccion
                    })
                    .ToList();

                return View("IngresosBrutos", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> IngresosBrutosMensuales(int id, CancellationToken cancellationToken)
        {
            var meses = new[]
            {
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            };

            ViewBag.Periodos = meses
                .Select((mes, index) => new SelectListItem
                {
                    Value = (index + 1).ToString(), // 1 a 12
                    Text = mes,

                })
                .ToList();

            var model = new IngresosBrutosMensualModel
            {
                id = id,
            };

            var cliente = await _clienteManager.ObtenerPorId(model.id, cancellationToken);

            if (cliente == null)
            {
                TempData["ErrorMessage"] = $"Error: No se encontró cliente con id: {id}";
                return View(model);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IngresosBrutosMensuales(IngresosBrutosMensualModel model, CancellationToken cancellationToken)
        {
            try
            {

                var meses = new[]
                {
                    "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
                };

                ViewBag.Periodos = meses
                    .Select((mes, index) => new SelectListItem
                    {
                        Value = (index + 1).ToString(),
                        Text = mes,
                        Selected = (model.Periodo == index + 1)
                    })
                    .ToList();

                if (!ModelState.IsValid)
                {
                    return View(model); // volver con errores
                }

                var cliente = await _clienteManager.ObtenerPorId(model.id, cancellationToken);

                if (cliente == null)
                {
                    TempData["ErrorMessage"] = $"Error: No se encontró cliente con id: {model.id}";
                    return View(model);
                }

                //lista completa de ib mensual
                var _listaIbMensual = await _ventaManager.GetAllIngresosbrutosMensual(model.id, model.Periodo.Value, model.Anio.Value, cancellationToken);
                model.listaIbMensual = _listaIbMensual.ToList();

                if (!_listaIbMensual.Any())
                {
                    ModelState.AddModelError(string.Empty, "No se encontraron datos de Ingresos Brutos para el período y año especificados.");

                    return View(model);
                }

                //resumen con el total del mes
                var _totalIbMensual = await _ventaManager.TotalIngresosBrutosMensual(model.id, model.Periodo.Value, model.Anio.Value, cancellationToken);
                model.ResumenMensual=_totalIbMensual;

                return View(model);


            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al procesar la solicitud. Intenta nuevamente.");


                var meses = new[]
                {
                    "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                    "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
                };

                ViewBag.Periodos = meses
                    .Select((mes, index) => new SelectListItem
                    {
                        Value = (index + 1).ToString(),
                        Text = mes
                    })
                    .ToList();

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> IngresosBrutosAnuales(int id, CancellationToken cancellationToken)
        {

            var model = new IngresosBrutosAnualesModel
            {
                id = id,
            };

            var cliente = await _clienteManager.ObtenerPorId(model.id, cancellationToken);

            if (cliente == null)
            {
                TempData["ErrorMessage"] = $"Error: No se encontró cliente con id: {id}";
                return View(model);
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IngresosBrutosAnuales(IngresosBrutosAnualesModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model); // volver con errores
                }

                var cliente = await _clienteManager.ObtenerPorId(model.id, cancellationToken);

                if (cliente == null)
                {
                    TempData["ErrorMessage"] = $"Error: No se encontró cliente con id: {model.id}";
                    return View(model);
                }

                // obtener la lista detalle mensual de todo el año
                //obtener los meses que registra ib
                var periodos = await _ventaManager.ObtenerPeriodos(model.id, model.Anio.Value, cancellationToken);

                if (model.Anio == null || !periodos.Any())
                {
                    ModelState.AddModelError(string.Empty, "No se encontraron datos de Ingresos Brutos para año especificado.");

                    return View(model);
                }


                //lista con todos los registros del año
                var _listaAnual = await _ventaManager.GetIngresosbrutosAnual(model.id, model.Anio.Value, cancellationToken);
                model.listaAnual = _listaAnual.ToList();

                //resumen lista de totales mensuales
                var _listaTotalIbMensual = await _ventaManager.GetTotalesIbMensual(model.id, model.Anio.Value, cancellationToken);
                model.listaIbTotalesPorMes = _listaTotalIbMensual.ToList();


                //resumen con el total del año
                var _totalIbAnual = await _ventaManager.TotalIngresosBrutosAnual(model.id, model.Anio.Value, cancellationToken);
                model.ResumenAnual = _totalIbAnual;

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al procesar la solicitud. Intenta nuevamente.");

                Debug.WriteLine(ex.Message);

                return View(model);
            }
        }

    }
}
