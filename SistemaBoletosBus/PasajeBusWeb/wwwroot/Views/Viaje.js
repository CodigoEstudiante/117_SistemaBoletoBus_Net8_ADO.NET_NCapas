let tablaData;
let idEditar = 0;
const controlador = "Viaje";
const modal = "mdData";


document.addEventListener("DOMContentLoaded", function (event) {

    tablaData = $('#tbData').DataTable({
        responsive: true,
        scrollX: true,
        "ajax": {
            "url": `/${controlador}/Lista`,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { title: "", "data": "idViaje", visible: false },
            {
                title: "Bus", "data": "bus", width: "120px", render: function (data, type, row) {
                    return`${data.numeroPlaca} - ${data.nombre}`
                }
            },
            {
                title: "Ruta", "data": "ruta", width: "250px", render: function (data, type, row) {
                    return `Origen: ${data.origen} | Destino: ${data.destino}`
                }
            },
            { title: "Fecha Salida", "data": "fechaSalida" },
            { title: "Hora Salida", "data": "horaSalida" },
            { title: "Fecha Llegada", "data": "fechaLlegada" },
            { title: "Hora Llegada", "data": "horaLlegada" },
            { title: "Precio", "data": "precio" },
            { title: "Total Asientos", "data": "totalAsientos" },
            { title: "Asientos Reservados", "data": "asientosReservados" },
            { title: "Asiento Disponibles", "data": "asientoDisponibles" },
            {
                title: "Completo", "data": "completo", render: function (data, type, row) {
                    return data == 0 ? "No":"Si"
                }
            },
            {
                title: "", "data": "idEspecialidad", width: "70px", render: function (data, type, row) {
                    return `<button class="btn btn-outline-primary me-2 btn-editar"><i class="fa-solid fa-pen"></i></button>` +
                        `<button class="btn btn-outline-danger btn-eliminar"><i class="fa-solid fa-trash"></i></button>`
                }
            }
        ],
        "order": [0, 'desc'],
        fixedColumns: {
            start: 0,
            end: 1
        },
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        }

    });


    fetch(`/Bus/Lista`, {
        method: "GET",
        headers: { 'Content-Type': 'application/json;charset=utf-8' }
    }).then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        if (responseJson.data.length > 0) {
            $("#cboBus").append($("<option>").val("").text(""));
            
            const buses_disponibles = responseJson.data.filter((e) => e.disponible == true);
            buses_disponibles.forEach((item) => {
                $("#cboBus").append($("<option>").val(item.idBus).text(`${item.numeroPlaca} - ${item.nombre}`));
            });
            $('#cboBus').select2({
                theme: 'bootstrap-5',
                dropdownParent: $('#mdData'),
                placeholder: "Seleccionar"
            });
        }
    }).catch((error) => {
        Swal.fire({
            title: "Error!",
            text: "No se encontraron coincidencias.",
            icon: "warning"
        });
    })

    fetch(`/Ruta/Lista`, {
        method: "GET",
        headers: { 'Content-Type': 'application/json;charset=utf-8' }
    }).then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        if (responseJson.data.length > 0) {
            $("#cboRuta").append($("<option>").val("").text(""));
            
            responseJson.data.forEach((item) => {
                $("#cboRuta").append($("<option>").val(item.idRuta).text(`Origen: ${item.origen} - Destino: ${item.destino}`));
            });
            $('#cboRuta').select2({
                theme: 'bootstrap-5',
                dropdownParent: $('#mdData'),
                placeholder: "Seleccionar"
            });
        }
    }).catch((error) => {
        Swal.fire({
            title: "Error!",
            text: "No se encontraron coincidencias.",
            icon: "warning"
        });
    })

    $('#txtHoraSalida').timepicker({
        timeFormat: 'h:mm p',
        interval: 30,
        minTime: '08',
        maxTime: '21:30PM',
        startTime: '08:00',
        dynamic: false,
        dropdown: true,
        scrollbar: true,
        zindex: 9999999
    });

    $('#txtHoraLlegada').timepicker({
        timeFormat: 'h:mm p',
        interval: 30,
        minTime: '08',
        maxTime: '21:30PM',
        startTime: '08:00',
        dynamic: false,
        dropdown: true,
        scrollbar: true,
        zindex: 9999999
    });

    $("#txtFechaSalida").datepicker({
        defaultDate: "",
        minDate: 0,
        onClose: function (date) {
            const dd = moment(date, "DD/MM/YYYY");
            $('#txtFechaLlegada').datepicker('option', 'minDate', dd.toDate());
        }
    })

    $("#txtFechaLlegada").datepicker({
        defaultDate: "",
        minDate: 0
    })

});

$("#btnNuevo").on("click", function () {
    idEditar = 0;
    $("#cboRuta").val("").trigger('change')
    $("#cboBus").val("").trigger('change')
    $("#txtFechaSalida").val("");
    $("#txtHoraSalida").val("");
    $("#txtFechaLlegada").val("");
    $("#txtHoraLlegada").val("");
    $("#txtPrecio").val("0");
    $(`#${modal}`).modal('show');
})

$("#tbData tbody").on("click", ".btn-editar", function () {
    const filaSeleccionada = $(this).closest('tr');
    const data = tablaData.row(filaSeleccionada).data();
    idEditar = data.idViaje;

    $("#cboRuta").val(data.ruta.idRuta).trigger('change')
    $("#cboBus").val(data.bus.idBus).trigger('change')
    $("#txtFechaSalida").val(data.fechaSalida);
    $("#txtHoraSalida").val(data.horaSalida);
    $("#txtFechaLlegada").val(data.fechaLlegada);
    $("#txtHoraLlegada").val(data.horaLlegada);
    $("#txtPrecio").val(data.precio);
    $(`#${modal}`).modal('show');
})


$("#tbData tbody").on("click", ".btn-eliminar", function () {
    const filaSeleccionada = $(this).closest('tr');
    const data = tablaData.row(filaSeleccionada).data();

    Swal.fire({
        text: `Desea eliminar?`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Si, continuar",
        cancelButtonText: "No, volver"
    }).then((result) => {
        if (result.isConfirmed) {

            fetch(`/${controlador}/Eliminar?Id=${data.idViaje}`, {
                method: "DELETE",
                headers: { 'Content-Type': 'application/json;charset=utf-8' }
            }).then(response => {
                return response.ok ? response.json() : Promise.reject(response);
            }).then(responseJson => {

                if (responseJson.data == "") {
                    Swal.fire({
                        title: "Eliminado!",
                        text: "Eliminado correctamente",
                        icon: "success"
                    });
                    tablaData.ajax.reload();
                } else {
                    Swal.fire({
                        title: "Error!",
                        text: responseJson.data,
                        icon: "warning"
                    });
                }
            }).catch((error) => {
                Swal.fire({
                    title: "Error!",
                    text: "No se pudo eliminar.",
                    icon: "warning"
                });
            })
        }
    });
})


$("#btnGuardar").on("click", function () {
    const inputs = $(".data-in").serializeArray();
    const inputText = inputs.find((e) => e.value == "");
    const inputNumber = inputs.find((e) => e.name == "Precio" && e.value == "0");

    if (inputText != undefined) {
        Swal.fire({
            title: "Error!",
            text: `Debe completar el campo: ${inputText.name}`,
            icon: "warning"
        });
        return
    }
    if (inputNumber != undefined) {
        Swal.fire({
            title: "Error!",
            text: `Debe ingresar el precio`,
            icon: "warning"
        });
        return
    }

    const HoraSalida = moment($("#txtHoraSalida").val(), ['h:mm A'])
    const HoraLlegada = moment($("#txtHoraLlegada").val(), ['h:mm A'])

    let objeto = {
        IdViaje: idEditar,
        Bus: {
            IdBus: $("#cboBus").val()
        },
        Ruta: {
            IdRuta: $("#cboRuta").val()
        },
        FechaSalida: $("#txtFechaSalida").val(),
        HoraSalida: HoraSalida.format('HH:mm'),
        FechaLlegada: $("#txtFechaLlegada").val(),
        HoraLlegada: HoraLlegada.format('HH:mm'),
        Precio: $("#txtPrecio").val().trim()
    }

    if (idEditar != 0) {

        fetch(`/${controlador}/Editar`, {
            method: "PUT",
            headers: { 'Content-Type': 'application/json;charset=utf-8' },
            body: JSON.stringify(objeto)
        }).then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        }).then(responseJson => {
            if (responseJson.data == "") {
                idEditar = 0;
                Swal.fire({
                    text: "Se guardaron los cambios!",
                    icon: "success"
                });
                $(`#${modal}`).modal('hide');
                tablaData.ajax.reload();
            } else {
                Swal.fire({
                    title: "Error!",
                    text: responseJson.data,
                    icon: "warning"
                });
            }
        }).catch((error) => {
            Swal.fire({
                title: "Error!",
                text: "No se pudo editar.",
                icon: "warning"
            });
        })
    } else {
        fetch(`/${controlador}/Crear`, {
            method: "POST",
            headers: { 'Content-Type': 'application/json;charset=utf-8' },
            body: JSON.stringify(objeto)
        }).then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        }).then(responseJson => {
            console.log(responseJson)
            if (responseJson.data == "") {
                Swal.fire({
                    text: "Registrado correctamente",
                    icon: "success"
                });
                $(`#${modal}`).modal('hide');
                tablaData.ajax.reload();
            } else {
                Swal.fire({
                    title: "Error!",
                    text: responseJson.data,
                    icon: "warning"
                });
            }
        }).catch((error) => {
            Swal.fire({
                title: "Error!",
                text: "No se pudo registrar.",
                icon: "warning"
            });
        })
    }
});