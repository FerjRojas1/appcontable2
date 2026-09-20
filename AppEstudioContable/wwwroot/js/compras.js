// AppEstudioContable/wwwroot/js/compras.js

document.addEventListener('DOMContentLoaded', function () {

    // ---------- Utilidades ----------
    const getCuitCliente = () => {
        const urlParams = new URLSearchParams(window.location.search);
        if (urlParams.has('cuit')) return urlParams.get('cuit');

        const cuitHiddenInput = document.getElementById('clienteCuit');
        return cuitHiddenInput ? cuitHiddenInput.value : '';
    };

    const clienteCuit = getCuitCliente();

    const buildUrl = (baseUrl, mes, anio, cuit) => {
        const params = [];
        if (cuit) params.push(cuit);
        if (anio) params.push(anio);
        if (mes) params.push(mes);

        return params.length > 0
            ? `${baseUrl}/${params.join('/')}`
            : baseUrl;
    };

    // ---------- Filtros de período ----------
    const mesInput = document.getElementById('mesInput');
    const anioInput = document.getElementById('anioInput');

    if (mesInput && anioInput) {
        // Validación mes
        mesInput.addEventListener('input', function () {
            this.value = this.value.replace(/[^0-9]/g, '').substring(0, 2);
            if (parseInt(this.value) > 12) this.value = '12';
            else if (parseInt(this.value) < 1 && this.value.length === 2) this.value = '01';
        });

        // Validación año
        anioInput.addEventListener('input', function () {
            this.value = this.value.replace(/[^0-9]/g, '').substring(0, 4);
        });

        // Botón: Listado
        const filtrarListadoBtn = document.getElementById('filtrarListadoBtnCompras');
        if (filtrarListadoBtn) {
            filtrarListadoBtn.addEventListener('click', function (e) {
                e.preventDefault();
                const url = buildUrl('/Compras/lista', mesInput.value, anioInput.value, clienteCuit);
                window.location.href = url;
            });
        }

        // Botón: Total Neto
        const verTotalNetoBtn = document.getElementById('verTotalNetoBtnCompras');
        if (verTotalNetoBtn) {
            verTotalNetoBtn.addEventListener('click', function (e) {
                e.preventDefault();
                const url = buildUrl('/Compras/VerNeto', mesInput.value, anioInput.value, clienteCuit);
                window.location.href = url;
            });
        }

        // Botón: Totales por comprobante
        const verTotalesComprobanteBtn = document.getElementById('verTotalesComprobanteBtnCompras');
        if (verTotalesComprobanteBtn) {
            verTotalesComprobanteBtn.addEventListener('click', function (e) {
                e.preventDefault();
                const url = buildUrl('/Compras/VerTotales', mesInput.value, anioInput.value, clienteCuit);
                window.location.href = url;
            });
        }

        // Botón: Limpiar filtros
        const limpiarFiltrosBtn = document.getElementById('limpiarFiltrosBtnCompras');
        if (limpiarFiltrosBtn) {
            limpiarFiltrosBtn.addEventListener('click', function (e) {
                e.preventDefault();
                mesInput.value = '';
                anioInput.value = '';
                const url = buildUrl('/Compras/lista', '', '', clienteCuit);
                window.location.href = url;
            });
        }
    }

    // ---------- Inicialización de DataTables ----------
    if ($.fn.DataTable) {
        console.log('DataTables disponible');
        const dtOptions = {
            paging: true,
            lengthChange: true,
            searching: true,       // 🔍 Buscador
            ordering: true,
            info: true,
            autoWidth: false,
            responsive: true,
            pageLength: 10,        // 10 filas por página
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "Todos"]],
            language: {
                url: "//cdn.datatables.net/plug-ins/1.10.25/i18n/Spanish.json"
            }
        };

        // Todas las tablas de la app
        const tablas = [
            '#tablaCompras',           // Index de Compras
            '#comprasAgregadas',       // Altas: para revisar
            '#comprasCorrectas',       // Altas: correctas
            '#comprasFallidas'         // Altas: fallidas
        ];

        tablas.forEach(sel => {
            if (document.querySelector(sel) && !$.fn.DataTable.isDataTable(sel)) {
                $(sel).DataTable(dtOptions);
                console.log('Inicializada:', sel);
            }
        });
    }

    // ---------- Validación del form de carga Excel ----------
    const form = document.getElementById('formCargaExcel');
    if (form) {
        const fileInput = document.getElementById('file');
        const fileValidationErrorDiv = document.getElementById('fileError');
        const btnCargar = form.querySelector('button[type="submit"]');
        const allowedExtensions = /\.(xlsx|xls|csv)$/i;

        form.addEventListener('submit', function (e) {
            if (fileInput) fileInput.classList.remove('is-invalid');
            if (fileValidationErrorDiv) fileValidationErrorDiv.style.display = 'none';

            if (!fileInput || fileInput.files.length === 0) {
                e.preventDefault();
                if (fileInput) fileInput.classList.add('is-invalid');
                if (fileValidationErrorDiv) {
                    fileValidationErrorDiv.textContent = 'Por favor, seleccioná un archivo.';
                    fileValidationErrorDiv.style.display = 'block';
                }
                return;
            }

            if (!allowedExtensions.test(fileInput.value)) {
                e.preventDefault();
                if (fileInput) fileInput.classList.add('is-invalid');
                if (fileValidationErrorDiv) {
                    fileValidationErrorDiv.textContent = 'Formato no válido. Solo se permiten .xlsx, .xls o .csv.';
                    fileValidationErrorDiv.style.display = 'block';
                }
                fileInput.value = '';
                return;
            }

            if (btnCargar) btnCargar.disabled = true;
            $('#modalCargando').modal('show');   // Bootstrap 4 (AdminLTE 3)
        });

        // Drag & Drop visual
        const dropZone = fileInput?.closest('.app-form-group') || fileInput?.closest('.mb-3');
        if (dropZone) {
            dropZone.addEventListener('dragover', e => {
                e.preventDefault();
                dropZone.classList.add('border', 'border-primary', 'bg-light');
            });
            dropZone.addEventListener('dragleave', () => {
                dropZone.classList.remove('border', 'border-primary', 'bg-light');
            });
            dropZone.addEventListener('drop', () => {
                dropZone.classList.remove('border', 'border-primary', 'bg-light');
            });
        }
    }

});

// ---------- Guardar edición de compra desde el modal ----------
document.body.addEventListener('click', function (e) {
    const btn = e.target.closest('.btn-guardar-modal');
    if (!btn) return;

    const idCompra = btn.getAttribute('data-id');
    const modal = document.getElementById('modalEdit_' + idCompra);
    if (!modal) {
        console.error("No se encontró el modal con id:", 'modalEdit_' + idCompra);
        return;
    }

    const getInputValue = (name) =>
        parseFloat(modal.querySelector(`input[name="${name}"]`)?.value.replace(',', '.') || '0');

    const total = getInputValue('Total');
    const netoGravado = getInputValue('NetoGravado');
    const noGravado = getInputValue('NoGravado');
    const exento = getInputValue('Exento');
    const iva = getInputValue('Iva');
    const iva0 = getInputValue('Iva0');
    const iva25 = getInputValue('Iva25');
    const iva5 = getInputValue('Iva5');
    const iva105 = getInputValue('Iva105');
    const iva21 = getInputValue('Iva21');
    const iva27 = getInputValue('Iva27');
    const grav0 = getInputValue('Grav0');
    const grav25 = getInputValue('Grav25');
    const grav5 = getInputValue('Grav5');
    const grav105 = getInputValue('Grav105');
    const grav21 = getInputValue('Grav21');
    const grav27 = getInputValue('Grav27');

    const tolerancia = 0.1;

    const totalValido = Math.abs(total - (netoGravado + iva + exento + noGravado)) < tolerancia;
    const ivaValido = Math.abs(iva - (iva0 + iva25 + iva5 + iva105 + iva21 + iva27)) < tolerancia;
    const netoValido = Math.abs(netoGravado - (grav0 + grav25 + grav5 + grav105 + grav21 + grav27)) < tolerancia;

    if (!totalValido) {
        alert("El Total no coincide con la suma de Neto Gravado + IVA + Exento + No Gravado.");
        return;
    }
    if (!ivaValido || !netoValido) {
        alert("La suma de IVA o Neto Gravado desglosado no coincide con los valores totales.");
        return;
    }

    const formData = new FormData(modal.querySelector('form'));
    const data = new URLSearchParams();
    for (const pair of formData) data.append(pair[0], pair[1]);

    fetch('/Compras/Edit2', { method: 'POST', body: data })
        .then(response => {
            if (!response.ok) throw new Error('Error al guardar en el servidor');
            return response.json();
        })
        .then(result => {
            if (result.success) {
                alert("Compra actualizada correctamente.");
                $('#modalEdit_' + idCompra).modal('hide');

                const $row = $(btn).closest('tr');
                const $tabla = $row.closest('table');
                if ($.fn.DataTable.isDataTable($tabla)) {
                    $tabla.DataTable().row($row).remove().draw(false);
                } else {
                    $row.remove();
                }
            } else {
                alert("Error: " + (result.message || "No se pudo actualizar."));
            }
        })
        .catch(error => {
            console.error(error);
            alert("Error al guardar la compra: " + error.message);
        });
}); 