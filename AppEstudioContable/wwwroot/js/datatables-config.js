

const dataTableLanguage = {
    "decimal": "",
    "emptyTable": "No hay datos disponibles en la tabla",
    "info": "Mostrando _START_ a _END_ de _TOTAL_ entradas",
    "infoEmpty": "Mostrando 0 a 0 de 0 entradas",
    "infoFiltered": "(filtrado de _MAX_ entradas totales)",
    "infoPostFix": "",
    "thousands": ",",
    "lengthMenu": "Mostrar _MENU_ entradas",
    "loadingRecords": "Cargando...",
    "processing": "Procesando...",
    "search": "Buscar:",
    "zeroRecords": "No se encontraron coincidencias",
    "paginate": {
        "first": "Primero",
        "last": "Último",
    },
    "aria": {
        "sortAscending": ": activar para ordenar la columna ascendente",
        "sortDescending": ": activar para ordenar la columna descendente"
    }
};
function crearDataTable(selector, entidad = "registros", opciones = {}) {

    const lenguaje = {
        ...dataTableLanguage,

        lengthMenu: `Mostrar _MENU_ ${entidad}`,
        info: `Mostrando _START_ a _END_ de _TOTAL_ ${entidad}`,
        infoEmpty: `Mostrando 0 a 0 de 0 ${entidad}`,
        infoFiltered: `(filtrado de _MAX_ ${entidad})`,
        zeroRecords: `No se encontraron ${entidad}`,
        emptyTable: `No hay ${entidad} para mostrar`
    };

    return new DataTable(selector, {
        language: lenguaje,
        ...opciones
    });
}