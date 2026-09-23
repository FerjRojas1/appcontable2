

document.addEventListener('DOMContentLoaded', function () {

    // ---------- Utilidades ----------
    const getVal = (id) => {
        const el = document.getElementById(id);
        if (!el) return 0;
        return window.CurrencyHelper
            ? window.CurrencyHelper.parse(el.value)
            : parseFloat(el.value) || 0;
    };

    const setVal = (id, valor) => {
        const el = document.getElementById(id);
        if (!el) return;
        el.value = window.CurrencyHelper
            ? window.CurrencyHelper.format(valor)
            : valor.toFixed(2);
    };

    // ---------- Alícuotas ----------
    const alicuotas = [
        { grav: 'Grav0', iva: 'Iva0', tasa: 0 },
        { grav: 'Grav25', iva: 'Iva25', tasa: 0.025 },
        { grav: 'Grav5', iva: 'Iva5', tasa: 0.05 },
        { grav: 'Grav105', iva: 'Iva105', tasa: 0.105 },
        { grav: 'Grav21', iva: 'Iva21', tasa: 0.21 },
        { grav: 'Grav27', iva: 'Iva27', tasa: 0.27 }
    ];

    // ---------- Cálculos ----------
    function calcularIvaPorAlicuota() {
        alicuotas.forEach(a => {
            const gravado = getVal(a.grav);
            const iva = gravado * a.tasa;
            setVal(a.iva, iva);
        });
    }

    function calcularNetoGravado() {
        const neto = alicuotas.reduce((acc, a) => acc + getVal(a.grav), 0);
        setVal('NetoGravado', neto);
    }

    function calcularIvaTotal() {
        const iva = alicuotas.reduce((acc, a) => acc + getVal(a.iva), 0);
        setVal('Iva', iva);
    }

    function calcularTotal() {
        const total = getVal('NetoGravado') + getVal('NoGravado')
            + getVal('Exento') + getVal('Iva');
        setVal('Total', total);
    }

    function recalcular() {
        calcularIvaPorAlicuota();
        calcularNetoGravado();
        calcularIvaTotal();
        calcularTotal();
    }

    // ---------- Listeners ----------
    const camposDisparadores = [
        ...alicuotas.map(a => a.grav),
        'NoGravado', 'Exento'
    ];

    camposDisparadores.forEach(id => {
        const el = document.getElementById(id);
        if (el) el.addEventListener('input', recalcular);
    });

    // ---------- Fecha por defecto ----------
    const fechaInput = document.getElementById('Fecha');
    if (fechaInput && !fechaInput.value) {
        const hoy = new Date();
        const yyyy = hoy.getFullYear();
        const mm = String(hoy.getMonth() + 1).padStart(2, '0');
        const dd = String(hoy.getDate()).padStart(2, '0');
        fechaInput.value = `${yyyy}-${mm}-${dd}`;
    }

    // ---------- Al enviar ----------
    const form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function () {
            // 👇 Convertir currency-input a formato plano antes de enviar
            form.querySelectorAll('input.currency-input').forEach(function (input) {
                if (window.CurrencyHelper) {
                    const raw = window.CurrencyHelper.parse(input.value);
                    // Formato plano con punto decimal, sin separador de miles
                    input.value = raw.toString();
                }
            });

            const nroDesde = document.getElementById('NroDesde')?.value;
            const nroHasta = document.getElementById('NroHasta');
            if (nroHasta && !nroHasta.value && nroDesde) {
                nroHasta.value = nroDesde;
            }
            form.querySelectorAll('input[type="number"]').forEach(input => {
                if (input.value === '') input.value = 0;
            });
        });
    }

    // ---------- Calcular al cargar ----------
    recalcular();
});