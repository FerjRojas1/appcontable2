using AppEstudioContable.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiciosEC.Managers;
using ServiciosEC.Models;
using System.Diagnostics;
using System.Threading;
using ServiciosEC.Interfaces.Managers;
using ServiciosEC.Utilidades.ModelosDTO;
using AppEstudioContable.Models.DTO;    


namespace AppEstudioContable.Controllers
{
    public class IVAController : Controller
    {
        private readonly IIvaManager _ivaManager;
        private IVentaManager _ventaManager;
        private readonly IClienteManager _clienteManager;
        private readonly ICompraManager _compraManager;
        private readonly ILibroIvaManager _libroIvaManager;

        public IVAController(IIvaManager ivaManager, IVentaManager ventaManager, IClienteManager clienteManager, ICompraManager compraManager, ILibroIvaManager libroIvaManager)
        {
            _ivaManager = ivaManager;
            _compraManager = compraManager;
            _ventaManager = ventaManager;
            _clienteManager = clienteManager;
            _libroIvaManager = libroIvaManager;
        }



        // GET: IVAController
        [Route("{controller}/{cuit?}")]
        public async Task<ActionResult> IndexAsync(string cuit, CancellationToken cancellationToken)
        {
            // Validaciones o valores por defecto
            if (cuit == null)
                return BadRequest("Debe ingresar un cuit.");

            try
            {
                var cliente = await _clienteManager.ObtenerClientePorCuitAsync(cuit, cancellationToken);

                if (cliente == null)
                    throw new Exception($"No se encontró ningún cliente con el cuit {cuit}");

                var periodosCargados = await _ivaManager.ObtenerPeriodosPorClienteAsync(cliente, cancellationToken);
                
                var model = periodosCargados.Select(p => { 

                    //extraigo elementos de la tupla
                    var (fecha, librosIva) = p;

                    return new PeriodosModel
                    {
                        Cuit = cuit,
                        Ano = fecha.Year,
                        Mes = fecha.Month,
                        Libros = librosIva,//lista de libros
                    };
                }).ToList();

                ViewBag.GraficoIva = ArmarDatosGrafico(model);
                ViewBag.Cuit = cuit;
                ViewBag.Id = cliente.IdPersona;
                ViewBag.RazonSocial = cliente.RazonSocial;

                return View(model);
                //return Json(model);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return View("Error", new ErrorViewModel { Message = ex.Message });
            }
        }

        //ejemplo https://localhost:7112/IVA/detalle/123/2025/03
        [HttpGet]
        [Route("{controller}/detalle/{cuit?}/{año?}/{mes?}")]
        public async Task<ActionResult> DetailsPeriodoAsync(string? cuit, int año, int mes, CancellationToken cancellationToken)
        {
            // Validaciones o valores por defecto
            if (cuit == null || año == null || mes == null)
                return BadRequest("Faltan parámetros.");

            try
            {
                var model = await ObtenerLibroIVA(cuit, año, mes, cancellationToken);
                return base.View(model);

            }catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return View("Error", new ErrorViewModel { Message = ex.Message });
            }
        }


        //ejemplo https://localhost:7112/IVA/resumen/123/2025/03
        [HttpGet]
        [Route("{controller}/resumen/{cuit?}/{año?}/{mes?}")]
        public async Task<ActionResult> ResumenPeriodoAsync(string? cuit, int año, int mes, CancellationToken cancellationToken)
        {
            // Validaciones o valores por defecto
            if (cuit == null || año == null || mes == null)
                return BadRequest("Faltan parámetros.");

            try
            {
                var model = await ObtenerLibroIVA(cuit, año, mes, cancellationToken);
                return base.View(model);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return View("Error", new ErrorViewModel { Message = ex.Message });
            }
        }


        [HttpPost]
        [Route("{controller}/resumen/guardar")]
        public async Task<ActionResult> GuardarPeriodo(IFormCollection coll, CancellationToken cancellationToken)
        {
            decimal.TryParse(coll["SaldoTecnicoAnterior"], out decimal SaldoTecnicoAnterior);
            decimal.TryParse(coll["SaldoLibreDisponibilidad"], out decimal SaldoLibreDisponibilidad);
            decimal.TryParse(coll["RetencionesPercepciones"], out decimal RetencionesPercepciones);

            var cuit = coll["Cuit"];
            int.TryParse(coll["Mes"], out int mes);
            int.TryParse(coll["Año"], out int año);

            try
            {
                var model = await ObtenerLibroIVA(cuit, año, mes, cancellationToken);

                model.Cliente = null;
                model.SaldoTecnicoAnterior = SaldoTecnicoAnterior;
                model.SaldoLibreDisponibilidad = SaldoLibreDisponibilidad;
                model.RetencionesIVA = RetencionesPercepciones;
                model.PercepcionesIVA = 0; //para evitar que se duplique


                // Guardar el resumen en la base de datos
                var libroIva = ServiciosEC.Utilidades.Mappers.MapearLibroIvaModelAEntidad(model, model.idPersona);
                await _libroIvaManager.Insertar(libroIva, cancellationToken);

                //return View(model);
                return Json(libroIva);
                //return Json(model);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return View("Error", new ErrorViewModel { Message = ex.Message });
            }

        }



        private async Task<LibroIvaModel> ObtenerLibroIVA(string? cuit, int año, int mes, CancellationToken cancellationToken)
        {
            var cliente = await _clienteManager.ObtenerClientePorCuitAsync(cuit, cancellationToken);

            if (cliente == null)
                throw new Exception($"No se encontró ningún cliente con el cuit {cuit}");

            //calculo para ventas
            (var TotalDebitoFiscal, var TotalRestitucionDebitoFiscal) = await _ventaManager.ObtenerIVAPorClienteYPeriodo(cliente, mes, año);

            //lo mismo para compras
            (var TotalCreditoFiscal, var TotalRestitucionCreditoFiscal) = await _compraManager.ObtenerIVAPorClienteYPeriodo(cliente, mes, año);

            return new LibroIvaModel
            {
                Cliente = cliente,
                idPersona = cliente.IdPersona,
                Cuit = cuit,
                Mes = mes,
                Año = año,

                TotalDebitoFiscal = TotalDebitoFiscal,
                TotalRestitucionDebitoFiscal = TotalRestitucionDebitoFiscal,

                TotalCreditoFiscal = TotalCreditoFiscal,
                TotalRestitucionCreditoFiscal = TotalRestitucionCreditoFiscal,

            };
        }

        private List<IvaGraficoAnualDto> ArmarDatosGrafico(List<PeriodosModel> periodos)
        {
            return periodos
                .GroupBy(p => p.Ano)
                .Select(g => new IvaGraficoAnualDto
                {
                    Anio = g.Key,

                    TotalCreditoIva = g.Sum(p => (p.Libros ?? new List<LibroIva>())
                        .Sum(l => (l.CreditoIva27 ?? 0) + (l.CreditoIva21 ?? 0) +
                                  (l.CreditoIva105 ?? 0) + (l.CreditoIva5 ?? 0) +
                                  (l.CreditoIva25 ?? 0) + (l.CreditoIva0 ?? 0) +
                                  (l.CreditoIvaOtros ?? 0))),

                    TotalDebitoIva = g.Sum(p => (p.Libros ?? new List<LibroIva>())
                        .Sum(l => (l.DebitoIva27 ?? 0) + (l.DebitoIva21 ?? 0) +
                                  (l.DebitoIva105 ?? 0) + (l.DebitoIva5 ?? 0) +
                                  (l.DebitoIva25 ?? 0) + (l.DebitoIva0 ?? 0) +
                                  (l.DebitoIvaOtros ?? 0)))
                })
                .OrderBy(d => d.Anio)
                .ToList();
        }

        [HttpGet]
        [Route("{controller}/periodos/{cuit?}")]
        public async Task<ActionResult> PeriodosCargados(string cuit, CancellationToken cancellationToken)
        {
            if (cuit == null)
                return BadRequest("Debe ingresar un cuit.");

            try
            {
                var cliente = await _clienteManager.ObtenerClientePorCuitAsync(cuit, cancellationToken);

                if (cliente == null)
                    throw new Exception($"No se encontró ningún cliente con el cuit {cuit}");

                var periodosCargados = await _ivaManager.ObtenerPeriodosPorClienteAsync(cliente, cancellationToken);

                var model = periodosCargados.Select(p =>
                {
                    var (fecha, librosIva) = p;
                    return new PeriodosModel
                    {
                        Cuit = cuit,
                        Ano = fecha.Year,
                        Mes = fecha.Month,
                        Libros = librosIva,
                    };
                }).ToList();

                ViewBag.Cuit = cuit;
                ViewBag.Id = cliente.IdPersona;
                ViewBag.RazonSocial = cliente.RazonSocial;

                return View(model);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return View("Error", new ErrorViewModel { Message = ex.Message });
            }
        }


        //// GET: IVAController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: IVAController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(IvaModel model, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        Iva iva = new Iva
        //        {
        //            Descripcion = model.Descripcion,
        //            Porcentaje = model.Porcentaje,
        //        };

        //        await _ivaManager.Insertar(iva, cancellationToken);
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        ModelState.AddModelError("", "Error al crear el IVA. Verifique los datos e intente nuevamente.");
        //        return View();
        //    }
        //}

        //// GET: IVAController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: IVAController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: IVAController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: IVAController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}




    }
    }
