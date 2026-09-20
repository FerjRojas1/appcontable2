// AppEstudioContable/wwwroot/js/calendario.js

document.addEventListener('DOMContentLoaded', function () {
    const calendarEl = document.getElementById('calendar');
    if (!calendarEl) return;

    const modalEl = document.getElementById('modalEvento');
    const formEvento = document.getElementById('formEvento');
    const $modal = $(modalEl);
    const $form = $(formEvento);

    const calendar = new FullCalendar.Calendar(calendarEl, {
        plugins: ['dayGrid', 'timeGrid', 'interaction', 'bootstrap'],   // 🔑 CLAVE
        locale: 'es',
        themeSystem: 'bootstrap',
        initialView: 'dayGridMonth',
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
        },
        buttonText: {
            today: 'Hoy',
            month: 'Mes',
            week: 'Semana',
            day: 'Día',
            list: 'Lista'
        },
        height: 'auto',
        navLinks: true,
        editable: false,
        selectable: true,
        dayMaxEvents: true,
        events: '/api/EventosAgenda',

        dateClick: function (info) {
            abrirModalNuevo(info.dateStr);
        },

        eventClick: function (info) {
            info.jsEvent.preventDefault();
            abrirModalEditar(info.event);
        }
    });

    calendar.render();

    // ---------- Abrir modal en modo "nuevo" ----------
    function abrirModalNuevo(fechaInicio) {
        $form[0].reset();
        $('#eventoId').val('');
        $('#modalEventoTitulo').text('Nuevo evento');
        $('#btnEliminarEvento').addClass('d-none');
        $('#eventoColor').val('#1e40af');

        if (fechaInicio) {
            // FullCalendar devuelve "YYYY-MM-DD" → agregamos hora
            $('#eventoFechaInicio').val(fechaInicio + 'T09:00');
        }

        $modal.modal('show');
    }

    // ---------- Abrir modal en modo "editar" ----------
    function abrirModalEditar(evento) {
        $('#eventoId').val(evento.id);
        $('#eventoTitulo').val(evento.title);
        $('#eventoDescripcion').val(evento.extendedProps?.description || '');
        $('#eventoFechaInicio').val(evento.start ? formatDateTime(evento.start) : '');
        $('#eventoFechaFin').val(evento.end ? formatDateTime(evento.end) : '');
        $('#eventoTodoElDia').prop('checked', evento.allDay);
        $('#eventoColor').val(evento.backgroundColor || '#1e40af');
        $('#modalEventoTitulo').text('Editar evento');
        $('#btnEliminarEvento').removeClass('d-none');

        $modal.modal('show');
    }

    function formatDateTime(date) {
        // date es un objeto Date → convertir a "YYYY-MM-DDTHH:mm"
        const d = new Date(date);
        const pad = n => String(n).padStart(2, '0');
        return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
    }

    // ---------- Guardar (POST o PUT) ----------
    $form.on('submit', function (e) {
        e.preventDefault();

        const id = $('#eventoId').val();
        const data = {
            id: id ? parseInt(id) : 0,
            titulo: $('#eventoTitulo').val(),
            descripcion: $('#eventoDescripcion').val(),
            fechaInicio: $('#eventoFechaInicio').val(),
            fechaFin: $('#eventoFechaFin').val() || null,
            todoElDia: $('#eventoTodoElDia').is(':checked'),
            color: $('#eventoColor').val()
        };

        const url = id ? `/api/EventosAgenda/${id}` : '/api/EventosAgenda';
        const method = id ? 'PUT' : 'POST';

        fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        })
            .then(response => {
                if (!response.ok) throw new Error('Error al guardar el evento');
                return response.status === 204 ? null : response.json();
            })
            .then(() => {
                $modal.modal('hide');
                calendar.refetchEvents();
            })
            .catch(error => {
                console.error(error);
                alert('Error al guardar el evento: ' + error.message);
            });
    });

    // ---------- Eliminar ----------
    $('#btnEliminarEvento').on('click', function () {
        const id = $('#eventoId').val();
        if (!id) return;
        if (!confirm('¿Eliminar este evento?')) return;

        fetch(`/api/EventosAgenda/${id}`, { method: 'DELETE' })
            .then(response => {
                if (!response.ok) throw new Error('Error al eliminar');
                $modal.modal('hide');
                calendar.refetchEvents();
            })
            .catch(error => {
                console.error(error);
                alert('Error al eliminar el evento: ' + error.message);
            });
    });

    // ---------- Botón "Nuevo evento" ----------
    $('#btnNuevoEvento').on('click', function () {
        abrirModalNuevo(null);
    });
});