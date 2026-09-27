// =========================================================
// IVA / Index — Lógica de la vista
// =========================================================

$(document).ready(function () {

    // ===================== Datos del gráfico =====================
    // vienen desde la vista como variable global
    // (window.datosGraficoIva)
    const datosGrafico = window.datosGraficoIva || [];
    let chartIva = null;

    // ===================== Formateo moneda ARS =====================
    function formatCurrency(valor) {
        if (valor === null || valor === undefined) return '-';
        return valor.toLocaleString('es-AR', { style: 'currency', currency: 'ARS' });
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

    // ===================== Gráfico IVA Anual =====================
    function renderGrafico(anio) {
        const dato = datosGrafico.find(d => d.anio === parseInt(anio));
        if (!dato) return;

        const canvas = document.getElementById('graficoIvaAnual');
        if (!canvas) return;

        // Destruir gráfico previo para evitar duplicados
        if (chartIva) {
            chartIva.destroy();
        }

        chartIva = new Chart(canvas, {
            type: 'bar',
            data: {
                labels: ['Totales Netos'],
                datasets: [
                    {
                        label: 'Crédito Fiscal (Neto)',
                        data: [dato.totalCreditoNeto],
                        backgroundColor: '#0891b2',
                        borderColor: '#0e7490',
                        borderWidth: 1
                    },
                    {
                        label: 'Débito Fiscal (Neto)',
                        data: [dato.totalDebitoNeto],
                        backgroundColor: '#64748b',
                        borderColor: '#475569',
                        borderWidth: 1
                    }
                ]
            },
            options: {
                indexAxis: 'y',   // Barras horizontales
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'top' },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                const valor = context.parsed.x || 0;
                                return context.dataset.label + ': ' +
                                    valor.toLocaleString('es-AR', {
                                        style: 'currency',
                                        currency: 'ARS'
                                    });
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        beginAtZero: true,
                        ticks: {
                            callback: function (value) {
                                return value.toLocaleString('es-AR', {
                                    style: 'currency',
                                    currency: 'ARS',
                                    maximumFractionDigits: 0
                                });
                            }
                        }
                    }
                }
            }
        });

        // Actualizar cards
        document.getElementById('totalCreditoNeto').textContent =
            (dato.totalCreditoNeto || 0).toLocaleString('es-AR', {
                style: 'currency',
                currency: 'ARS'
            });

        document.getElementById('totalDebitoNeto').textContent =
            (dato.totalDebitoNeto || 0).toLocaleString('es-AR', {
                style: 'currency',
                currency: 'ARS'
            });

        document.getElementById('anioCreditoLabel').textContent = `Año ${dato.anio}`;
        document.getElementById('anioDebitoLabel').textContent = `Año ${dato.anio}`;
    }

    // Render inicial + listener del dropdown
    const selectorAnio = document.getElementById('anioGrafico');
    if (selectorAnio && datosGrafico.length > 0) {
        renderGrafico(selectorAnio.value);

        selectorAnio.addEventListener('change', function () {
            renderGrafico(this.value);
        });
    }

});