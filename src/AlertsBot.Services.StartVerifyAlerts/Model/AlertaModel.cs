using System;
using System.Collections.Generic;
using System.Text;

namespace AlertsBot.Services.StartVerifyAlerts.Model
{
    public class AlertaModel
    {
        public int Id { get; set; }
        public string AlertName { get; set; }
        public string Message { get; set; }
        public DateTime LastRuntime { get; set; }
        public long ChatId { get; set; }
        public int SendAmount { get; set; }
        public int Delay { get; set; }
        public TipoHorario TipoHorario { get; set; }
        public string HoraEspecifica { get; set; }



        public AlertaModel(int id, string alertName, string message, DateTime lastRuntime, long chatId, int sendAmount, int delay, int tipoHorario, string HoraEspecifica = null)
        {
            this.Id = id;
            this.AlertName = alertName;
            this.Message = message;
            this.LastRuntime = lastRuntime;
            this.LastRuntime = lastRuntime;
            this.ChatId = chatId;
            this.SendAmount = sendAmount;
            this.Delay = delay;
            this.TipoHorario = (TipoHorario)tipoHorario;
            if(HoraEspecifica != null)
            {
                this.HoraEspecifica = HoraEspecifica;
            }
        }

    }
}
