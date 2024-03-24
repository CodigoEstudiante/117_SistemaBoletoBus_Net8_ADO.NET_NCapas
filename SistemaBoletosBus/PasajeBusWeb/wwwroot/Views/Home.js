let rutas = [];
let viajes = [];
let Usuario;
const customElement = $("<div>", {
    "css": {
        "font-size": "40px",
        "text-align": "center",
    },
    "class": "your-custom-class",
    "text": "Buscando..."
});
let viajeReservado;
let asientosReservados = [];

document.addEventListener("DOMContentLoaded", function (event) {
    $("#cardEncontrados").hide()
    $('#txtNumeroTarjeta').mask('0000-0000-0000-0000');
    $('#txtMesAnio').mask('00/00');
    $('#txtCodigo').mask('000');

    $.LoadingOverlay("show")

    fetch(`/Home/ObtenerUsuario`, {
        method: "GET",
        headers: { 'Content-Type': 'application/json;charset=utf-8' }
    }).then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        Usuario = responseJson.data;
        if (Usuario.idUsuario != 0) {
            $("#txtNombreCompleto").val(Usuario.nombres)
            $("#txtApellidoCompleto").val(Usuario.apellidos)
            $("#txtCorreo").val(Usuario.correo)
        }
    }).catch((error) => {
    })

    fetch(`/Ruta/Lista`, {
        method: "GET",
        headers: { 'Content-Type': 'application/json;charset=utf-8' }
    }).then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        $.LoadingOverlay("hide")
        if (responseJson.data.length > 0) {
            
            rutas = responseJson.data;
            $("#cboOrigen").append($("<option>").val("").text(""));

            const origenes = rutas.map((e) => (e.origen))
   
            const dataArr = new Set(origenes);

            let result = [...dataArr];

            result.forEach((value) => {
                $("#cboOrigen").append($("<option>").val(value).text(value));
            });
           
            $('#cboOrigen').select2({
                theme: 'bootstrap-5',
                placeholder: "Seleccionar"
            });
        }
    }).catch((error) => {
        $.LoadingOverlay("hide")
        Swal.fire({
            title: "Error!",
            text: "No se encontraron coincidencias.",
            icon: "warning"
        });
    })

    obtenerViajes();
   

    $("#txtFechaSalida").datepicker({
        defaultDate: "",
        minDate: 0
    })

    $('#cboDestino').select2({
        theme: 'bootstrap-5',
        placeholder: "Seleccionar"
    });

});

function obtenerViajes() {
    fetch(`/Viaje/Lista`, {
        method: "GET",
        headers: { 'Content-Type': 'application/json;charset=utf-8' }
    }).then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        if (responseJson.data.length > 0) {
            $.LoadingOverlay("hide")
            viajes = responseJson.data.filter((e) => e.completo == 0);

        }
    }).catch((error) => {
        $.LoadingOverlay("hide")
        Swal.fire({
            title: "Error!",
            text: "No se encontraron coincidencias.",
            icon: "warning"
        });
    })
}

$('#cboOrigen').on('select2:select', function (e) {

    $("#cboDestino").select2("destroy").select2();
    $('#cboDestino').find('option').remove()

    var data = e.params.data;
    const destinos = rutas.filter((e) => e.origen == data.id)

    $("#cboDestino").append($("<option>").val("").text(""));

    destinos.forEach((item) => {
        $("#cboDestino").append($("<option>").val(item.destino).text(`${item.destino}`));
    });

    $('#cboDestino').select2({
        theme: 'bootstrap-5',
        placeholder: "Seleccionar"
    });
});


$("#btnBuscar").on("click", function () {
    

    const origen = $("#cboOrigen").val();
    const destino = $("#cboDestino").val();
    const fecha = $("#txtFechaSalida").val();

    if (origen == "" || destino == null || fecha == "") {
        Swal.fire({
            text: "Debe completar todos los datos",
            icon: "warning"
        });
        return
    }

    $.LoadingOverlay("show", {
        image: "",
        custom: customElement
    });

    $("#cardEncontrados").show();
    const encontrados = viajes.filter((e) => e.ruta.origen == origen && e.ruta.destino == destino && e.fechaSalida == fecha);
    $("#cnEncontrados").html("");
    if (encontrados.length != 0) {
        encontrados.forEach((item) => {
            $("#cnEncontrados").append(`
                <div class="col">
                    <div class="card text-center">
                        <div class="card-header">
                            <h5 class="card-title">${item.ruta.origen} - ${item.ruta.destino}</h5>
                        </div>
                        <div class="card-body">
                            <h5 class="card-title">Fecha: ${item.fechaSalida} </h5>
                            <p class="card-text">Hora: ${item.horaSalida}<br />Precio: ${item.precio} </p>
                            <button class="btn btn-primary btn-verDetalle" data-id="${item.idViaje}">Ver detalle</button>
                        </div>
                    </div>
                </div>`)
        })
    } else {
        $("#cnEncontrados").append(`<div class="col-sm-12">
            <div class="alert alert-warning" role="alert">
                No se encontraron viajes
            </div>
        </div>`)
    }

    $.LoadingOverlay("hide")

})

$(document).on('click', '.btn-verDetalle', function () {
    const id = $(this).data("id")
    
    const viaje = viajes.find((e) => e.idViaje == id);
    viajeReservado = viaje;

    $.LoadingOverlay("show")

    fetch(`/Viaje/ObtenerAsientosReserva?IdViaje=${viaje.idViaje}`, {
        method: "GET",
        headers: { 'Content-Type': 'application/json;charset=utf-8' }
    }).then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        $.LoadingOverlay("hide")
        if (responseJson.data.length > 0) {

            const AsientosPiso1 = responseJson.data.filter((e) => e.numeroPiso == 1);
            const AsientosPiso2 = responseJson.data.filter((e) => e.numeroPiso == 2);
            console.log(AsientosPiso2)

            if (AsientosPiso1.length > 0)
                $("#divPiso1").show()
            else
                $("#divPiso1").hide()

            if (AsientosPiso2.length > 0)
                $("#divPiso2").show()
            else
                $("#divPiso2").hide()

            $("#tbPiso1 tbody").html("")
            while (AsientosPiso1.length > 0) {
                const fila = AsientosPiso1.slice(0, 3);
                
                const tr = $("<tr>")
                let espacios = 1;
                fila.forEach((e) => {
                    const clase = e.idReserva == 0 ? "asientos border-primary" : "asientos_reservados bg-danger text-white border-danger";
                    if (espacios != 3) {
                        tr.append(
                            $("<td>").append(
                                $(`<div data-text="${e.numeroAsiento}" data-id="${e.idAsiento}">`).addClass(`border  rounded p-3 d-flex justify-content-center align-items-center ${clase}`)
                                    .text(e.numeroAsiento)
                            )
                        )
                    } else {
                        tr.append(
                            $("<td>").attr({ "width":"40"})
                        )
                        tr.append(
                            $("<td>").append(
                                $(`<div data-text="${e.numeroAsiento}" data-id="${e.idAsiento}">`).addClass(`border  rounded p-3 d-flex justify-content-center align-items-center ${clase}`)
                                    .text(e.numeroAsiento)
                            )
                        )
                    }
                    espacios = espacios + 1;
                })

                $("#tbPiso1 tbody").append(tr);

                const quitar = AsientosPiso1.splice(0, 3);
            }

            $("#tbPiso2 tbody").html("")
            while (AsientosPiso2.length > 0) {
                const fila = AsientosPiso2.slice(0, 3);

                const tr = $("<tr>")
                let espacios = 1;
                fila.forEach((e) => {
                    const clase = e.idReserva == 0 ? "asientos border-primary" : "asientos_reservados bg-danger text-white border-danger";
                    if (espacios != 3) {
                        tr.append(
                            $("<td>").append(
                                $(`<div data-text="${e.numeroAsiento}" data-id="${e.idAsiento}">`).addClass(`border rounded p-3 d-flex justify-content-center align-items-center ${clase}`)
                                    .text(e.numeroAsiento)
                            )
                        )
                    } else {
                        tr.append(
                            $("<td>").attr({ "width": "40" })
                        )
                        tr.append(
                            $("<td>").append(
                                $(`<div data-text="${e.numeroAsiento}" data-id="${e.idAsiento}">`).addClass(`border rounded p-3 d-flex justify-content-center align-items-center ${clase}`)
                                    .text(e.numeroAsiento)
                            )
                        )
                    }
                    espacios = espacios + 1;
                })

                $("#tbPiso2 tbody").append(tr);

                const quitar = AsientosPiso2.splice(0, 3);
            }
        }
    }).catch((error) => {
        $.LoadingOverlay("hide")
        Swal.fire({
            title: "Error!",
            text: "No se encontraron coincidencias.",
            icon: "warning"
        });
    })

    
    $("#spOrigenDestino").text(`${viaje.ruta.origen} - ${viaje.ruta.destino }`)
    $("#spFechaSalida").text(viaje.fechaSalida)
    $("#spHoraSalida").text(viaje.horaSalida)
    $("#spPrecio").text(viaje.precio)
    $(".card-paso1").hide()

    $(".card-paso2").show()

})

$(document).on("click", "div.asientos", function () {

    const idAsiento = $(this).data("id");
    const numeroAsiento = $(this).data("text");

    if ($(this).hasClass("bg-primary")) {
        $(this).removeClass("text-white bg-primary");
        const nuevoAsientosReservados = asientosReservados.filter((el) => el.IdAsiento != idAsiento);
        asientosReservados = nuevoAsientosReservados;
    }
    else {
        $(this).addClass("text-white bg-primary");
        asientosReservados.push({ IdAsiento: idAsiento, NumeroAsiento: numeroAsiento });
    }
});

$("#btnRegresar").on("click", function () {
    $(".card-paso1").show()
    $(".asientos").removeClass("text-white bg-primary");
    $(".card-paso2").hide()
})
$("#bntContinuar").on("click", function () {

    if (Usuario.idUsuario == 0) {
        Swal.fire({
            title: "Error!",
            text: `Debe iniciar sesion para continuar`,
            icon: "warning"
        });
        return;
    }

    if (asientosReservados.length < 1) {
        Swal.fire({
            title: "Error!",
            text: `Debe seleccionar minimo un asiento`,
            icon: "warning"
        });
        return;
    }

    const precio = parseFloat(viajeReservado.precio);
    const cantidad = asientosReservados.length;
    

    $("#txtNroAsientos").val(cantidad);
    $("#txtMontoTotal").val(precio * cantidad);

    $("#mdReserva").modal("show")
})
$("#btnGuardar").on("click", function () {
    const inputs = $(".input-reserva").serializeArray();
    const inputText = inputs.find((e) => e.value == "");
    const idsAsientosConcatenados = asientosReservados.map(asiento => asiento.IdAsiento).join(',');

    if (inputText != undefined) {
        Swal.fire({
            title: "Error!",
            text: `Debe completar el campo: ${(inputText.name).replace(/_/g,' ') }`,
            icon: "warning"
        });
        return
    }
    const precio = parseFloat(viajeReservado.precio);
    const cantidad = asientosReservados.length;

    const reserva = {
        Viaje: {
            IdViaje: viajeReservado.idViaje
        },
        Pasajero: {
            IdPasajero: Usuario.idUsuario
        },
        AsientosReservados: cantidad,
        MontoTotal: (precio * cantidad).toString(),
        IdAsientos: idsAsientosConcatenados
    }

    fetch(`/Home/Reservar`, {
        method: "POST",
        headers: { 'Content-Type': 'application/json;charset=utf-8' },
        body: JSON.stringify(reserva)
    }).then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        if (responseJson.data == "") {
            obtenerViajes();
            $(".card-paso1").show();

            $("#cboOrigen").val("").trigger('change');
            $("#cboDestino").val("").trigger('change');
            $("#txtFechaSalida").val("");

            $("#cardEncontrados").hide();
            $(".card-paso2").hide()
            $(`#mdReserva`).modal('hide');

            Swal.fire({
                text: "Registrado correctamente",
                icon: "success"
            });

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


})
