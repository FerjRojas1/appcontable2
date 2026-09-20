
// Formateo de moneda para inputs (formato es-AR: 1.234.567,89)

(function () {
    'use strict';

    // ---------- Parsear: "1.234,56" → 1234.56 ----------
    function parseCurrency(str) {
        if (str === null || str === undefined) return 0;
        str = String(str).trim();
        if (str === '') return 0;

        // Sacar todo excepto dígitos, coma y punto
        str = str.replace(/[^\d,.-]/g, '');

        // Si hay coma Y punto: el punto es separador de miles, la coma decimal
        if (str.includes(',') && str.includes('.')) {
            str = str.replace(/\./g, '').replace(',', '.');
        }
        // Si hay solo coma: es decimal
        else if (str.includes(',')) {
            str = str.replace(',', '.');
        }
        // Si hay solo punto: puede ser decimal o miles. Asumimos decimal.
        // (para valores chicos como 1.234 podría ser miles, pero priorizamos decimal)

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