// =========================================================
// IVA / PeriodosCargados — Lógica del modal Libro IVA
// =========================================================

$(document).ready(function () {

    // ===================== Formateo moneda ARS =====================
    function formatCurrency(valor) {
        if (valor === null || valor === undefined) return '-';
        return Number(valor).toLocaleString('es-AR', {
            style: 'currency',
            currency: 'ARS'
        });
    }

    // ===================== Formateo fecha (dd/mm/yyyy) =====================
    function formatDate(fechaStr) {
        if (!fechaStr) return '-';
        const fecha = new Date(fechaStr);
        return fecha.toLocaleDateString('es-AR');
    }

    // ===================== Modal Libro IVA =====================
    $('#libroIvaModal').on('show.bs.modal', function (event) {
        const button = $(event.relatedTarget);
        const libro = button.data('libro');
        console.log("Libro:", libro);

        if (!libro) return;

        // Débito Fiscal
        $('#campoDebitoNeto27').text(formatCurrency(libro.debitoNeto27));
        $('#campoDebitoNeto21').text(formatCurrency(libro.debitoNeto21));
        $('#campoDebitoNeto105').text(formatCurrency(libro.debitoNeto105));
        $('#campoDebitoIva27').text(formatCurrency(libro.debitoIva27));
        $('#campoDebitoIva21').text(formatCurrency(libro.debitoIva21));
        $('#campoDebitoIva105').text(formatCurrency(libro.debitoIva105));
        $('#campoDebitoNoGravado').text(formatCurrency(libro.debitoNoGravado));
        $('#campoDebitoExento').text(formatCurrency(libro.debitoExento));
        $('#campoDebitoNetoOtros').text(formatCurrency(libro.debitoNetoOtros));
        $('#campoDebitoIvaOtros').text(formatCurrency(libro.debitoIvaOtros));

        // Restitución Débito Fiscal
        $('#campoRestDebitoNeto27').text(formatCurrency(libro.restDebitoNeto27));
        $('#campoRestDebitoNeto21').text(formatCurrency(libro.restDebitoNeto21));
        $('#campoRestDebitoNeto105').text(formatCurrency(libro.restDebitoNeto105));
        $('#campoRestDebitoIva27').text(formatCurrency(libro.restDebitoIva27));
        $('#campoRestDebitoIva21').text(formatCurrency(libro.restDebitoIva21));
        $('#campoRestDebitoIva105').text(formatCurrency(libro.restDebitoIva105));
        $('#campoRestDebitoNoGravado').text(formatCurrency(libro.restDebitoNoGravado));
        $('#campoRestDebitoExento').text(formatCurrency(libro.restDebitoExento));
        $('#campoRestDebitoNetoOtros').text(formatCurrency(libro.restDebitoNetoOtros));
        $('#campoRestDebitoIvaOtros').text(formatCurrency(libro.restDebitoIvaOtros));

        // Crédito Fiscal
        $('#campoCreditoNeto27').text(formatCurrency(libro.creditoNeto27));
        $('#campoCreditoNeto21').text(formatCurrency(libro.creditoNeto21));
        $('#campoCreditoNeto105').text(formatCurrency(libro.creditoNeto105));
        $('#campoCreditoIva27').text(formatCurrency(libro.creditoIva27));
        $('#campoCreditoIva21').text(formatCurrency(libro.creditoIva21));
        $('#campoCreditoIva105').text(formatCurrency(libro.creditoIva105));
        $('#campoCreditoNoGravado').text(formatCurrency(libro.creditoNoGravado));
        $('#campoCreditoExento').text(formatCurrency(libro.creditoExento));
        $('#campoCreditoNetoOtros').text(formatCurrency(libro.creditoNetoOtros));
        $('#campoCreditoIvaOtros').text(formatCurrency(libro.creditoIvaOtros));

        // Restitución Crédito Fiscal
        $('#campoRestCreditoNeto27').text(formatCurrency(libro.restCreditoNeto27));
        $('#campoRestCreditoNeto21').text(formatCurrency(libro.restCreditoNeto21));
        $('#campoRestCreditoNeto105').text(formatCurrency(libro.restCreditoNeto105));
        $('#campoRestCreditoIva27').text(formatCurrency(libro.restCreditoIva27));
        $('#campoRestCreditoIva21').text(formatCurrency(libro.restCreditoIva21));
        $('#campoRestCreditoIva105').text(formatCurrency(libro.restCreditoIva105));
        $('#campoRestCreditoNoGravado').text(formatCurrency(libro.restCreditoNoGravado));
        $('#campoRestCreditoExento').text(formatCurrency(libro.restCreditoExento));
        $('#campoRestCreditoNetoOtros').text(formatCurrency(libro.restCreditoNetoOtros));
        $('#campoRestCreditoIvaOtros').text(formatCurrency(libro.restCreditoIvaOtros));

        // Resumen Técnico
        $('#campoFechaDeclaracion').text(formatDate(libro.fechaDeclaracion));
        $('#campoGravadoDebitoFical').text(formatCurrency(libro.gravadoDebitoFical));
        $('#campoIvaDebitoFiscal').text(formatCurrency(libro.ivaDebitoFiscal));
        $('#campoGravadoCreditoFiscal').text(formatCurrency(libro.gravadoCreditoFiscal));
        $('#campoIvaCreditoFiscal').text(formatCurrency(libro.ivaCreditoFiscal));
        $('#campoSaldoTecnico').text(formatCurrency(libro.saldoTecnico));
        $('#campoRetencionesIVA').text(formatCurrency(libro.retencionesIva));
        $('#campoPercepcionesIVA').text(formatCurrency(libro.percepcionesIva));
        $('#campoSaldoTecnicoNeto').text(formatCurrency(libro.saldoTecnicoNeto));
    });

});