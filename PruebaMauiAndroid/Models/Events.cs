using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteAndroidAgenda.Models
{
    public class AgendaEvent
    {
        public string Titulo { get; set; }
        public DateTime HoraObjetivo { get; set; }
        public string TiempoRestante => CalcularTiempoRestante();

        private string CalcularTiempoRestante()
        {
            TimeSpan tiempoRestante = HoraObjetivo - DateTime.Now;
            if (tiempoRestante.TotalDays > 1)
                return $"{(int)tiempoRestante.TotalDays} días restantes";
            if (tiempoRestante.TotalHours > 1)
                return $"{(int)tiempoRestante.TotalHours} horas restantes";
            if (tiempoRestante.TotalMinutes > 1)
                return $"{(int)tiempoRestante.TotalMinutes} minutos restantes";
            if (tiempoRestante.TotalSeconds > 0)
                return "Menos de un minuto restante";
            return "Evento expirado";
        }
    }
}
