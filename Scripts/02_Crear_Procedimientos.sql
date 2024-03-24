use DBBoletoBus
go

--- CONFIGURACION ---

create FUNCTION [dbo].[SplitString]  ( 
	@string NVARCHAR(MAX), 
	@delimiter CHAR(1)  
)
RETURNS
@output TABLE(valor NVARCHAR(MAX)  ) 
BEGIN 
	DECLARE @start INT, @end INT 
	SELECT @start = 1, @end = CHARINDEX(@delimiter, @string) 
	WHILE @start < LEN(@string) + 1
	BEGIN 
		IF @end = 0  
        SET @end = LEN(@string) + 1 

		INSERT INTO @output (valor)  
		VALUES(SUBSTRING(@string, @start, @end - @start)) 
		SET @start = @end + 1 
		SET @end = CHARINDEX(@delimiter, @string, @start) 
	END 
	RETURN
END

go
-- PROCEDIMIENTOS PARA BUSES

create procedure sp_listaBus
as
begin
	select IdBus,NumeroPlaca,Nombre,CapacidadPiso1,CapacidadPiso2,
	CapacidadTotal,Disponible,convert(char(10),FechaCreacion,103)[FechaCreacion] from Bus
end

go

create procedure sp_crearBus(
@NumeroPlaca varchar(50),
@Nombre varchar(50),
@CapacidadPiso1 int,
@CapacidadPiso2 int,
@msgError varchar(100) OUTPUT
)
as
begin
	declare @idBusGenerado int = 0

	begin try
		begin tran
		set @msgError = ''
		if(not exists(select * from Bus where NumeroPlaca = @NumeroPlaca))
		begin
			insert into Bus(NumeroPlaca,Nombre,CapacidadPiso1,CapacidadPiso2,Disponible) values
			(@NumeroPlaca,@Nombre,@CapacidadPiso1,@CapacidadPiso2,1)

			set @idBusGenerado = SCOPE_IDENTITY()

			;WITH Asientos AS (
				SELECT 1 AS Numero
				UNION ALL
				SELECT  Numero + 1 FROM Asientos WHERE Numero < (@CapacidadPiso1 + @CapacidadPiso2)
			)
			SELECT 0 NumeroPiso, Numero into #tempAsiento FROM Asientos

			update #tempAsiento set NumeroPiso = 1 where Numero <= @CapacidadPiso1
			update #tempAsiento set NumeroPiso = 2 where Numero > @CapacidadPiso1

			insert into Asiento(IdBus,NumeroPiso,NumeroAsiento)
			select @idBusGenerado,NumeroPiso,Numero from #tempAsiento

		end
		else
			set @msgError = 'El numero de placa ya se encuentra registrado'

		commit tran
	end try
	begin catch
		rollback tran
		set @msgError = ERROR_MESSAGE()
	end catch

end

go

create procedure sp_editarBus(
@IdBus int,
@NumeroPlaca varchar(50),
@Nombre varchar(50),
@CapacidadPiso1 int,
@CapacidadPiso2 int,
@msgError varchar(100) OUTPUT
)
as
begin


	begin try
		begin tran
		set @msgError = ''

		update Bus set NumeroPlaca = @NumeroPlaca, Nombre = @Nombre where IdBus = @IdBus

		declare @CapacidadPiso1Actual int = (select CapacidadPiso1 from Bus where IdBus = @IdBus)
		declare @CapacidadPiso2Actual int = (select CapacidadPiso2 from Bus where IdBus = @IdBus)

		if((@CapacidadPiso1Actual != @CapacidadPiso1) or (@CapacidadPiso2Actual != @CapacidadPiso2) )
		begin
			update Bus set CapacidadPiso1 = @CapacidadPiso1, CapacidadPiso2 = @CapacidadPiso2 where IdBus = @IdBus

			if(not exists(select * from Asiento a
			 inner join [dbo].[ReservaDetalle] rd on rd.IdAsiento = a.IdAsiento
			 where a.IdBus = @IdBus))
			 begin

				delete from Asiento where IdBus = @IdBus

				;WITH Asientos AS (
					SELECT 1 AS Numero
					UNION ALL
					SELECT  Numero + 1 FROM Asientos WHERE Numero < (@CapacidadPiso1 + @CapacidadPiso2)
				)
				SELECT 0 NumeroPiso, Numero into #tempAsiento FROM Asientos

				update #tempAsiento set NumeroPiso = 1 where Numero <= @CapacidadPiso1
				update #tempAsiento set NumeroPiso = 2 where Numero > @CapacidadPiso1

				insert into Asiento(IdBus,NumeroPiso,NumeroAsiento)
				select @IdBus,NumeroPiso,Numero from #tempAsiento

			 end
			else
				set @msgError = 'No se puede actualizar los asientos, ya existe reserva'
		end
			
		commit tran
	end try
	begin catch
		rollback tran
		set @msgError = ERROR_MESSAGE()
	end catch

end

go


create procedure sp_eliminarBus
(
@IdBus int,
@msgError varchar(100) OUTPUT
)
as
begin
	set @msgError = ''
	if(not exists( select * from [dbo].[Viaje] v
	inner join Bus b on b.IdBus = v.IdBus
	where b.IdBus = @IdBus))
	begin
		delete from Asiento where IdBus = @IdBus
		delete top (1) from Bus where IdBus = @IdBus

	end
	else
		set @msgError = 'El bus se encuentra asignado a un viaje'
end

go
-- PROCEDIMIENTOS PARA RUTAS

create procedure sp_listaRutas
as
begin
	select IdRuta,Origen,Destino,convert(char(10),FechaCreacion,103)[FechaCreacion] from Ruta
end

go

create procedure sp_crearRuta(
@Origen varchar(50),
@Destino varchar(50),
@msgError varchar(100) OUTPUT
)
as
begin

	set @msgError = ''
	if(not exists(select * from Ruta where Origen =@Origen and Destino =@Destino))
		insert into Ruta(Origen,Destino) values(@Origen,@Destino)
	else
		set @msgError = 'El destino y origen ya existe'
end

go


create procedure sp_editarRuta(
@IdRuta int,
@Origen varchar(50),
@Destino varchar(50),
@msgError varchar(100) OUTPUT
)
as
begin

	set @msgError = ''
	if(not exists(select * from Ruta where Origen =@Origen and Destino =@Destino and IdRuta != @IdRuta))
		update Ruta set Origen =@Origen ,Destino = @Destino where IdRuta =@IdRuta
	else
		set @msgError = 'El destino y origen ya existe'
end

go

create procedure sp_eliminarRuta
(
@IdRuta int,
@msgError varchar(100) OUTPUT
)
as
begin
	set @msgError = ''
	if(not exists( select * from [dbo].[Viaje] v
	inner join Ruta r on r.IdRuta = v.IdRuta
	where r.IdRuta  = @IdRuta))
	begin
		delete top (1) from Ruta where IdRuta = @IdRuta

	end
	else
		set @msgError = 'La ruta ya esta asignada a un viaje'
end

-- PROCEDIMIENTOS PARA RUTAS
go

create procedure sp_listaViaje
as
begin
	select v.IdViaje,
	b.IdBus,b.NumeroPlaca,b.Nombre,
	r.IdRuta,r.Origen,r.Destino,
	convert(char(10),v.FechaSalida,103)[FechaSalida],convert(char(5),v.HoraSalida,108)[HoraSalida],
	convert(char(10),v.FechaLlegada,103)[FechaLlegada],convert(char(5),v.HoraLlegada,108)[HoraLlegada],
	CONVERT(varchar(10),v.Precio)[Precio],
	v.TotalAsientos,v.AsientosReservados,v.AsientoDisponibles,v.Completo,
	convert(char(10),v.FechaCreacion,103)[FechaCreacion]
	from Viaje v
	inner join Bus b on b.idbus = v.IdBus
	inner join Ruta r on r.idRuta = v.idRuta
end

go

create procedure sp_crearViaje(
@IdBus int,
@IdRuta int,
@FechaSalida varchar(10),
@HoraSalida varchar(5),
@FechaLlegada varchar(10),
@HoraLlegada varchar(5),
@Precio varchar(10),
@msgError varchar(100) OUTPUT
)
as
begin
	set dateformat dmy

	set @msgError = ''
	declare @FecSalida date = convert(date,@FechaSalida)
	declare @HorSalida time = convert(time,@HoraSalida)
	declare @FecLlegada date = convert(date,@FechaLlegada)
	declare @HorLlegada time = convert(time,@HoraLlegada)
	declare @PrecioFormat decimal(10,2) = convert(decimal(10,2),@Precio)

	declare @totalAsientos int = (select MAX(numeroasiento) from Asiento where IdBus = @IdBus)

	if(not exists(select * from Viaje where IdRuta = @IdRuta and FechaSalida = @FecSalida and HoraSalida = @HoraSalida))
	begin
		insert into Viaje(IdBus,IdRuta,FechaSalida,HoraSalida,FechaLlegada,HoraLlegada,Precio,TotalAsientos,AsientosReservados) values
		(@IdBus,@IdRuta,@FecSalida,@HorSalida,@FecLlegada,@HorLlegada,@PrecioFormat,@totalAsientos,0)
	end
	else
		set @msgError = 'El viaje ya existe'

end

go

create procedure sp_editarViaje(
@IdViaje int,
@IdBus int,
@IdRuta int,
@FechaSalida varchar(10),
@HoraSalida varchar(5),
@FechaLlegada varchar(10),
@HoraLlegada varchar(5),
@Precio varchar(10),
@msgError varchar(100) OUTPUT
)
as
begin
	set dateformat dmy

	set @msgError = ''
	declare @FecSalida date = convert(date,@FechaSalida)
	declare @HorSalida time = convert(time,@HoraSalida)
	declare @FecLlegada date = convert(date,@FechaLlegada)
	declare @HorLlegada time = convert(time,@HoraLlegada)
	declare @PrecioFormat decimal(10,2) = convert(decimal(10,2),@Precio)

	declare @totalAsientos int = (select MAX(numeroasiento) from Asiento where IdBus = @IdBus)

	if(exists(select * from Viaje where IdRuta = @IdRuta and FechaSalida = @FecSalida and HoraSalida = @HoraSalida and IdViaje != @IdViaje))
	begin
		set @msgError = 'El viaje ya existe'
	end
	if(exists(select * from Reserva where IdViaje = @IdViaje))
	begin
		set @msgError = 'El viaje ya tiene reserva'
	end

	if(@msgError = '')
	begin
		update Viaje set IdBus = @IdBus, IdRuta = @IdRuta , FechaSalida = @FecSalida,
		HoraSalida = @HorSalida, FechaLlegada = @FecLlegada,HoraLlegada = @HorLlegada, Precio = @PrecioFormat,TotalAsientos = @totalAsientos
		where IdViaje = @IdViaje
	end
	

end

go

create procedure sp_eliminarViaje(
@IdViaje int,
@msgError varchar(100) OUTPUT
)
as
begin

	set @msgError = ''
	if(not exists(select * from Reserva where IdViaje = @IdViaje))
	begin
		delete from Viaje where IdViaje = @IdViaje
	end
	else
		set @msgError = 'El viaje ya tiene reserva'

end

go

create procedure sp_obtenerAsientosReserva(
@IdViaje int
)
as
begin

	select a.IdAsiento,a.IdBus,a.NumeroPiso,a.NumeroAsiento,ISNULL(rd.IdReserva,0)[IdReserva] from Viaje v
	inner join Asiento a on a.IdBus = v.IdBus
	left join [dbo].[ReservaDetalle] rd on rd.IdAsiento = a.IdAsiento
	left join [dbo].[Reserva] r on r.IdReserva = rd.IdReserva
	where v.IdViaje = @IdViaje

end

go

create procedure sp_guardarReserva(
@IdViaje int,
@IdPasajero int,
@AsientosReservados int,
@MontoTotal varchar(10),
@IdAsientos varchar(max),
@msgError varchar(100) OUTPUT
)
as
begin
	declare @tableID TABLE(Id int,TableName varchar(10));
	declare @MontoTotalValor decimal(10,2);

	begin try
		begin tran
			set @msgError = ''
			set @MontoTotalValor = CONVERT(decimal(10,2),@MontoTotal)
	
			insert into Reserva(IdViaje,IdPasajero,AsientosReservados,MontoTotal)
			output inserted.IdReserva,'Reserva'[TableName] INTO @tableID(Id,TableName)
			values(@IdViaje,
			@IdPasajero,
			@AsientosReservados,
			@MontoTotalValor)

			insert into ReservaDetalle(IdReserva,IdAsiento)
			select (select Id from @tableID where TableName = 'Reserva'),valor from [dbo].[SplitString](@IdAsientos,',') 

			update Viaje set AsientosReservados = AsientosReservados + (select count(valor) from [dbo].[SplitString](@IdAsientos,','))
			where IdViaje = @IdViaje

		commit tran
	end try
	begin catch
		rollback tran
		set @msgError = ERROR_MESSAGE()
	end catch
end

go

create procedure sp_obtenerUsuario(  
@Correo varchar(50),  
@Clave varchar(50)  
)  
as  
begin  
  
 select IdUsuario,Nombres,Apellidos,Correo,Clave,TipoUsuario from Usuario where Correo = @Correo and Clave = @Clave  
  
end  
  
go

create procedure sp_crearUsuario(
@Nombres varchar(50),
@Apellidos varchar(50),
@Correo varchar(50),
@Clave varchar(50),
@msgError varchar(100) OUTPUT
)
as
begin
	
	set @msgError = ''
	if(not exists(select * from Usuario where Correo = @Correo))
	begin
		insert into Usuario(Nombres,Apellidos,Correo,Clave,TipoUsuario) values
		(@Nombres,@Apellidos,@Correo,@Clave,'Pasajero')
	end
	else
		set @msgError = 'El correo ya existe'
end

go

create procedure sp_obtenerViajesUsuario(  
@IdUsuario int
)  
as  
begin  
  
 select r.IdReserva, ru.Origen,ru.Destino,b.Nombre[NombreBus],
 convert(char(10),v.FechaSalida,103)[FechaSalida],
 convert(char(5),v.HoraSalida,108)[HoraSalida],
  convert(char(10),v.FechaLlegada,103)[FechaLlegada],
 convert(char(5),v.HoraLlegada,108)[HoraLlegada],
 convert(varchar(10),v.Precio)Precio,
 r.AsientosReservados[CantidadAsientos] ,
 convert(varchar(10),r.MontoTotal)MontoTotal,
	a.NumeroAsientos
 from Reserva r
 inner join Viaje v on v.IdViaje = r.IdViaje
 inner join Bus b on b.IdBus = v.IdBus
 inner join Ruta ru on ru.IdRuta = v.IdRuta
 cross apply (
	SELECT STUFF((
    SELECT ', ' + convert(varchar,asi.NumeroAsiento)
    FROM ReservaDetalle rd
	inner join Asiento asi on asi.IdAsiento = rd.IdAsiento
	where  IdReserva = r.IdReserva
    FOR XML PATH('')), 1, 2, '') AS NumeroAsientos
 ) a
 where r.IdPasajero = @IdUsuario
 order by r.IdReserva desc
end  


go

declare @msg varchar(50)
exec sp_crearBus 'PBUS001','BUS 01',15,12,@msg output
exec sp_crearBus 'PBUS002','BUS 02',15,12,@msg output
exec sp_crearBus 'PBUS003','BUS 03',15,12,@msg output
exec sp_crearBus 'PBUS004','BUS 04',15,12,@msg output
exec sp_crearBus 'PBUS005','BUS 05',15,12,@msg output
