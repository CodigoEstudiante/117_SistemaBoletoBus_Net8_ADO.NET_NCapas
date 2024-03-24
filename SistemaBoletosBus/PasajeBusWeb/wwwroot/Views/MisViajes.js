
document.addEventListener("DOMContentLoaded", function (event) {

    tablaData = $('#tbData').DataTable({
        scrollCollapse: true,
        scrollX: true,
        "ajax": {
            "url": `/Viaje/ViajesUsuario`,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { title: "", "data": "idReserva", visible: false },
            { title: "Origen", "data": "origen", width: "150px" },
            { title: "Destino", "data": "destino", width: "150px" },
            { title: "Bus", "data": "nombreBus", width:"100px" },
            { title: "Fecha Salida", "data": "fechaSalida", width: "100px" },
            { title: "Hora Salida", "data": "horaSalida", width: "100px" },
            { title: "Fecha Llegada", "data": "fechaLlegada", width: "120px" },
            { title: "Hora Llegada", "data": "horaLlegada", width: "100px" },
            { title: "Precio", "data": "precio" },
            { title: "Cantidad Asientos", "data": "cantidadAsientos", width: "160px" },
            { title: "Monto Total", "data": "montoTotal", width: "160px" },
            { title: "Numero Asientos", "data": "numeroAsientos", width: "160px" },
        ],
        "order": [0, 'desc'],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        }
    });

});