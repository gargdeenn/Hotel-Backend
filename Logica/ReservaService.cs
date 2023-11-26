using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Datos;
using Entity;

namespace Logica
{
    public class ReservaService
    {
        private readonly HotelContext _context;

        public ReservaService(HotelContext context)
        {
            _context = context;
        }
        public GuardarReservaResponse Guardar(Reserva reserva)
        {
            try
            {
                var numero = _context.Reservas.ToList().Count + 1;

                reserva.IdReserva = numero+"";
                
                var respuesta = _context.Habitaciones.Find(reserva.IdHabitacion);
                respuesta.Estado = "Pendiente";
                respuesta.FechaDisponible = reserva.FechaSalida;
                reserva.CalcularFactura(respuesta);
                _context.Reservas.Add(reserva);
                _context.Habitaciones.Update(respuesta);
                _context.SaveChanges();
                return new GuardarReservaResponse(reserva);
            }
            catch (Exception e)

            {
                return new GuardarReservaResponse($"Error de la Aplicacion: {e.Message}");
            }

        }
        public GuardarReservaResponse Actualizar(Reserva reserva)
        {
            try{
                var response = _context.Habitaciones.Find(reserva.IdHabitacion);
                if(response != null){
                    response.Estado = reserva.Habitacion.Estado;
                    if (reserva.Habitacion.Estado.Equals("desocupado"))
                    {
                        response.FechaDisponible = DateTime.Now;
                    }
                    _context.Habitaciones.Update(response);
                    _context.SaveChanges();
                    reserva.Habitacion = response;
                    return new GuardarReservaResponse(reserva);
                }

                return new GuardarReservaResponse("No se encontró");

            }catch(Exception e){
                    return new GuardarReservaResponse($"Error de la Aplicacion: {e.Message}");
            }
        }
        public ConsultaReservaResponse ConsultarTodos()
        {
            try
            {
                List<Reserva> reservas = _context.Reservas.ToList();
                foreach (var item in reservas)
                {
                    item.Habitacion = _context.Habitaciones.Find(item.IdHabitacion);
                }
                return new ConsultaReservaResponse(reservas);
            }
            catch (Exception e)
            {
                return new ConsultaReservaResponse($"Error en la aplicacion:  {e.Message}");
            }
        }

        public ConsultaReservaCedulaResponse BuscarxIdentificacion(string Cedula)
        {
            try{

            List<Reserva> reservas = _context.Reservas.Where(p => p.Cedula.Equals(Cedula)).ToList();
            return new ConsultaReservaCedulaResponse(reservas);
            }catch(Exception e){
                return new ConsultaReservaCedulaResponse($"Error en la aplicacion:  {e.Message}");
            }
        }

        public EliminarReservaResponse Eliminar(string numeroreserva)
        {
             EliminarReservaResponse eliminarReservaResponse = new EliminarReservaResponse();
            Reserva reserva = new Reserva();
            try
            {
                if ((reserva = _context.Reservas.Find(numeroreserva)) != null)
            {
                eliminarReservaResponse.Error=false;
                eliminarReservaResponse.Mensaje="Reserva eliminada correctamente";
                _context.Reservas.Remove(reserva);
                _context.SaveChanges();
            }
            }
            catch (Exception e)
            {
                eliminarReservaResponse.Error=true;
                eliminarReservaResponse.Mensaje=$"Hubo un error al momento de eliminar la reserva, {e.Message}";
            }
            return eliminarReservaResponse;
        }

        public class ConsultaReservaResponse
        {

            public ConsultaReservaResponse(List<Reserva> reservas)
            {
                Error = false;
                Reservas = reservas;
            }

            public ConsultaReservaResponse(string mensaje)
            {
                Error = true;
                Mensaje = mensaje;
            }
            public Boolean Error { get; set; }
            public string Mensaje { get; set; }
            public List<Reserva> Reservas { get; set; }
        }
         public class EliminarReservaResponse{
            public String Mensaje { get; set; }
            public bool Error { get; set; }
        }

        public class GuardarReservaResponse

        {

            public GuardarReservaResponse(Reserva reserva)

            {
                Error = false;

                Reserva = reserva;

            }



            public GuardarReservaResponse(string mensaje)

            {
                Error = true;
                Mensaje = mensaje;
            }

            public bool Error { get; set; }

            public string Mensaje { get; set; }

            public Reserva Reserva { get; set; }

        }
         public class ConsultaReservaCedulaResponse
        {

            public ConsultaReservaCedulaResponse(List<Reserva> reservas)
            {
                Error = false;
                Reservas = reservas;
            }

            public ConsultaReservaCedulaResponse(string mensaje)
            {
                Error = true;
                Mensaje = mensaje;
            }
            public Boolean Error { get; set; }
            public string Mensaje { get; set; }
            public List<Reserva> Reservas { get; set; }
        }
    }
}