using AppEstudioContable.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiciosEC.Managers;
using System.Diagnostics;
using System.Threading;
using ServiciosEC.Interfaces.Managers;


namespace AppEstudioContable.Controllers
{
    public class GananciasController(IClienteManager clienteManager, IVentaManager ventaManager, ICompraManager compraManager) : Controller
    {
        private readonly IClienteManager _clienteManager = clienteManager;
        private readonly IVentaManager _ventaManager = ventaManager;
        private readonly ICompraManager _compraManager = compraManager;



        // GET: GananciasController
        public async Task<ActionResult> IndexAsync(string cuit, int? año, CancellationToken cancellationToken)
        {
            ViewBag.Cuit = cuit;
            //si es nulo, se usa el año actual
            año??=DateTime.Today.Year;

            // Validaciones o valores por defecto
            if (cuit == null)
                return BadRequest("Faltan parámetros.");

            try
            {
                var cliente = await _clienteManager.ObtenerClientePorCuitAsync(cuit, cancellationToken);

                if (cliente == null)
                    throw new Exception($"No se encontró ningún cliente con el cuit {cuit}");

                //calculo para ventas
                var TotalDebitoFiscal = await _ventaManager.ObtenerIVAMensualPorCliente(cliente, (int)año);

                //lo mismo para compras
                var TotalCreditoFiscal = await _compraManager.ObtenerIVAMensualPorCliente(cliente, (int)año);

                ViewBag.Id = cliente.IdPersona;
                ViewBag.RazonSocial = cliente.RazonSocial;

                var model = new GananciasModel
                {
                    Ventas = TotalDebitoFiscal,
                    Compras = TotalCreditoFiscal,
                };

                return View(model);
                //return Json(model);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return View("Error", new ErrorViewModel { Message = ex.Message });
            }
        }

        // GET: GananciasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: GananciasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GananciasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET: GananciasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GananciasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET: GananciasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GananciasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }
    }
}
