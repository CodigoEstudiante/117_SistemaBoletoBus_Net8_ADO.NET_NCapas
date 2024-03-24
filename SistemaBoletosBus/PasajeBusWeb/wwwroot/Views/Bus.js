let tablaData;
let idEditar = 0;
const controlador = "Bus";
const modal = "mdData";

document.addEventListener("DOMContentLoaded", function (event) {

    tablaData = $('#tbData').DataTable({
       /* responsive: true,*/
        scrollCollapse: true,
        scrollX: true,
        "ajax": {
            "url": `/${controlador}/Lista`,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { title: "", "data": "idBus", visible: false },
            { title: "Numero Placa", "data": "numeroPlaca" },
            { title: "Nombre", "data": "nombre" },
            { title: "Capacidad Piso 1", "data": "capacidadPiso1" },
            { title: "Capacidad Piso 2", "data": "capacidadPiso2" },
            {
                title: "Disponible", "data": "disponible", render: function (data, type, row) {
                    return data == true ? `SI` : `NO`
                }
            },
            { title: "Fecha Creacion", "data": "fechaCreacion" },
            {
                title: "", "data": "idBus", width: "70px", render: function (data, type, row) {
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

});

$("#txtNumeroPlaca").on("keyup", function () {
    this.value = this.value.toLocaleUpperCase();
})

$("#txtNombre").on("keyup", function () {
    this.value = this.value.toLocaleUpperCase();
})

$("#btnNuevo").on("click", function () {
    idEditar = 0;
    $("#txtNumeroPlaca").val("");
    $("#txtNombre").val("");
    $("#txtCapacidadPiso1").val("0");
    $("#txtCapacidadPiso2").val("0");
    $(`#${modal}`).modal('show');
})

$("#tbData tbody").on("click", ".btn-editar", function () {
    const filaSeleccionada = $(this).closest('tr');
    const data = tablaData.row(filaSeleccionada).data();
    idEditar = data.idBus;

    $("#txtNumeroPlaca").val(data.numeroPlaca);
    $("#txtNombre").val(data.nombre);
    $("#txtCapacidadPiso1").val(data.capacidadPiso1);
    $("#txtCapacidadPiso2").val(data.capacidadPiso2);
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

            fetch(`/${controlador}/Eliminar?Id=${data.idBus}`, {
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
                        text: "No se pudo eliminar.",
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
    if ($("#txtNumeroPlaca").val().trim() == "" ||
        $("#txtNombre").val().trim() == "" ||
        $("#txtCapacidadPiso1").val() == "0" ||
        $("#txtCapacidadPiso2").val() == "0" 
    ) {
        Swal.fire({
            title: "Error!",
            text: "Debe completar todos los datos.",
            icon: "warning"
        });
        return
    }

    let objeto = {
        IdBus: idEditar,
        NumeroPlaca: $("#txtNumeroPlaca").val().trim(),
        Nombre: $("#txtNombre").val().trim(),
        CapacidadPiso1: $("#txtCapacidadPiso1").val(),
        CapacidadPiso2: $("#txtCapacidadPiso2").val(),
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