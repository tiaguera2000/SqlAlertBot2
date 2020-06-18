using AlertsBot.Services.StartVerifyAlerts.Model.RabbitQueue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;

namespace AlertsBot.Services.StartVerifyAlerts.Helpers
{
    public class RabbitHelper
    {

        public static RabbitQueue GetRabbitQueue(string rabbitUrl, string vhost, string queueName)
        {
            try
            {
                string ambiente = string.Format("{0}/api/queues/{1}/{2}", rabbitUrl, vhost, queueName);
                HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(ambiente);
                getRequest.Method = "GET";
                getRequest.Credentials = new NetworkCredential("autoscaler", "kubeauto");
                ServicePointManager.ServerCertificateValidationCallback = new
                   RemoteCertificateValidationCallback
                   (
                      delegate { return true; }
                   );

                var getResponse = (HttpWebResponse)getRequest.GetResponse();
                Stream newStream = getResponse.GetResponseStream();
                StreamReader sr = new StreamReader(newStream);
                var result = sr.ReadToEnd();
                if (result != null)
                {
                    try
                    {
                        var queue = JsonConvert.DeserializeObject<RabbitQueue>(result);
                        return queue;
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }
    }
}
