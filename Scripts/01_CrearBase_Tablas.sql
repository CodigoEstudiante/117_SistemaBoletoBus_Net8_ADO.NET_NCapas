
create database DBBoletoBus

go

use DBBoletoBus

go

create table Bus
(
IdBus int primary key identity,
NumeroPlaca varchar(50),
Nombre varchar(50),
CapacidadPiso1 int,
CapacidadPiso2 int,
CapacidadTotal as CapacidadPiso1 + CapacidadPiso2,
Disponible bit,
FechaCreacion datetime default getdate()
)
go

create table Asiento(
IdAsiento int primary key identity,
IdBus int references Bus(IdBus),
NumeroPiso int,
NumeroAsiento int,
FechaCreacion datetime default getdate()
)
go
create table Ruta
(
IdRuta int primary key identity,
Origen varchar(50),
Destino varchar(50),
FechaCreacion datetime default getdate()
)

go

create table Viaje(
IdViaje int primary key identity,
IdBus int references Bus(IdBus),
IdRuta int references Ruta(IdRuta),
FechaSalida date,
HoraSalida time,
FechaLlegada date,
HoraLlegada time,
Precio decimal(10,2),
TotalAsientos int,
AsientosReservados int,
AsientoDisponibles as TotalAsientos - AsientosReservados,
Completo as case when TotalAsientos - AsientosReservados < 1 then 1 else 0 end,
FechaCreacion datetime default getdate()
)

go


create table Reserva(
IdReserva int primary key identity,
IdViaje int references Viaje(IdViaje),
IdPasajero int,
AsientosReservados int,
MontoTotal decimal(10,2),
FechaCreacion datetime default getdate()
)

go

create table ReservaDetalle(
IdReservaDetalle int primary key identity,
IdReserva int references Reserva(IdReserva),
IdAsiento int,
FechaCreacion datetime default getdate()
)


go

create table Usuario(
IdUsuario int primary key identity,
Nombres varchar(50),
Apellidos varchar(50),
Correo varchar(50),
Clave varchar(50),
TipoUsuario varchar(20),
FechaCreacion datetime default getdate()
)

go

insert into Usuario(Nombres,Apellidos,Correo,Clave,TipoUsuario)
values
('jose','mendez','admin@gmail.com','123','Admin'),
('yui','kasamada','yui@gmail.com','123','Pasajero')



go

SET IDENTITY_INSERT Ruta ON
insert into Ruta(IdRuta,Origen,Destino) values
(1,'Lima','Huanuco'),
(2,'Huanuco','Lima'),
(3,'Piura','Lima'),
(4,'Lima','Piura'),
(5,'Lima','Loreto'),
(6,'Loreto','Lima'),
(7,'Lima','Tacna'),
(8,'Tacna','Lima'),
(9,'Lima','Arequipa'),
(10,'Arequipa','Lima')
SET IDENTITY_INSERT Ruta OFF
