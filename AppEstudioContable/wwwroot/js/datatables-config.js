

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