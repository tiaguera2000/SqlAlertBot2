using System;
using System.Collections.Generic;
using System.Text;

namespace AlertsBot.Services.StartVerifyAlerts.Model
{
    public class RabbitMonitor
    {
        public string Name{ get; set; }
        public string VirtualHost { get; set; }
        public string RabbitUrl { get; set; }
        public double Delay { get; set; }
        public TipoHorario TipoHorario { get; set; }
        public DateTime LastRuntime { get; set; }
        public int SendAmount { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }

        public RabbitMonitor(string name, string virtualHost, string rabbitUrl, string delay, string tipoHorario, string horaInicio = null, string horaFim = null)
        {
            this.Name = name.Replace("+", "%2B");
            this.VirtualHost = virtualHost.Replace("/", "%2F");
            this.RabbitUrl = rabbitUrl;
            this.Delay = Convert.ToDouble(delay);
            this.TipoHorario = (TipoHorario)Convert.ToInt32(tipoHorario);
            this.LastRuntime = DateTime.Now;
            this.SendAmount = 0;
            if(horaInicio != null && horaFim != null)
            {
                this.HoraInicio = horaInicio;
                this.HoraFim = horaFim;
            }
        }
    }
}
