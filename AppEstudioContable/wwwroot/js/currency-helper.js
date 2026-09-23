
// Formateo de moneda para inputs (formato es-AR: 1.234.567,89)

(function () {
    'use strict';

    // ---------- Parsear: "1.234,56" → 1234.56 ----------
    function parseCurrency(str) {
        if (str === null || str === undefined) return 0;
        str = String(str).trim();
        if (str === '') return 0;

        // Sacar todo excepto dígitos, coma, punto y signo
        str = str.replace(/[^\d,.-]/g, '');

        const tieneComa = str.includes(',');
        const tienePunto = str.includes('.');

        if (tieneComa && tienePunto) {
            // Formato es-AR: 1.234,56 → el punto es miles, la coma decimal
            str = str.replace(/\./g, '').replace(',', '.');
        } else if (tieneComa) {
            // Solo coma: 1234,56 → decimal
            str = str.replace(',', '.');
        } else if (tienePunto) {
            // Solo punto: decidir si es miles o decimal
            const partes = str.split('.');
            const ultima = partes[partes.length - 1];
            // Múltiples puntos O último grupo de exactamente 3 dígitos → miles
            if (partes.length > 2 || ultima.length === 3) {
                str = str.replace(/\./g, '');
            }
            // Si no, se deja el punto como decimal (ej: "1.5" → 1.5)
        }

        const num = parseFloat(str);
        return isNaN(num) ? 0 : num;
    }

    // ---------- Formatear: 1234.56 → "1.234,56" ----------
    function formatCurrency(num) {
        if (num === null || num === undefined || num === '') return '';
        const n = typeof num === 'string' ? parseCurrency(num) : num;
        if (isNaN(n)) return '';

        return n.toLocaleString('es-AR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    }

    // ---------- Aplicar formato a un input ----------
    function formatInput(input) {
        if (!input || input.readOnly) return;
        const raw = parseCurrency(input.value);
        input.value = raw === 0 && input.value === '' ? '' : formatCurrency(raw);
    }

    // ---------- Inicializar todos los inputs de moneda ----------
    function initCurrencyInputs(container) {
        const scope = container || document;

        scope.querySelectorAll('input.currency-input').forEach(input => {
            // Al perder el foco, formatear
            input.addEventListener('blur', function () {
                formatInput(this);
            });

            // Al escribir, permitir solo números, coma y punto
            input.addEventListener('input', function () {
                this.value = this.value.replace(/[^\d,.-]/g, '');
            });

            // Al enfocar, mostrar el valor plano (sin formato) para editar cómodo
            input.addEventListener('focus', function () {
                const raw = parseCurrency(this.value);
                this.value = raw === 0 ? '' : raw.toString().replace('.', ',');
            });

            // Formatear al cargar
            formatInput(input);
        });
    }

    // ---------- Exponer API global ----------
    window.CurrencyHelper = {
        parse: parseCurrency,
        format: formatCurrency,
        init: initCurrencyInputs,
        formatInput: formatInput
    };

    // Auto-inicializar cuando el DOM esté listo
    document.addEventListener('DOMContentLoaded', function () {
        initCurrencyInputs();
    });
})();