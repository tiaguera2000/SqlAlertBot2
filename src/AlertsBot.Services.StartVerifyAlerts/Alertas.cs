using AlertsBot.Domain.Events.Core;
using AlertsBot.Domain.Interfaces;
using AlertsBot.Services.StartVerifyAlerts.Model;
using AlertsBot.Services.StartVerifyAlerts.Model.RabbitQueue;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SharedKernel.DomainEvents.Core;
using SharedKernel.DomainEvents.CrossDomainEvents;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AlertsBot.Services.StartVerifyAlerts
{
    public class Alertas : IHostedService, IDisposable
    {
        #region Properties
        public static Thread _objThread;
        public static List<AlertaModel> _alertsHistory;
        public static List<AlertaModel> _alertsHistoryRabbit;
        public static List<RabbitMonitor> _filasMonitor;
        public static List<RabbitMonitor> _jornalMonitor;


        public static IHandler<ICrossDomainEvent> _handler;
        public static ICompanyRepository _companyService;
        public static IJobRepository _jobRepository;
        public static ICurriculum _curriculumRepository;
        public static long chatIdTriggerRelatorio = 0;
        public static long chatIdAlertasBne = 0;
        public static long chatIdGrupoAlertaRabbits = 0;

        public static IConfiguration _configuration;
        public static IPayment __paymentRepository;
        public static ILog _logger;
        #endregion Properties

        public Alertas(IHandler<ICrossDomainEvent> handler, ICompanyRepository companyRepository, IJobRepository jobRepository, ICurriculum curriculumRepository, IPayment paymentRepository, IConfiguration configuration, ILog log)
        {
            _handler = handler;
            _configuration = configuration;
            _alertsHistory = GetAlerts();
            _filasMonitor = GetRabbitFilasVerify();
            _jornalMonitor = GetJornais();
            _companyService = companyRepository;
            _jobRepository = jobRepository;
            __paymentRepository = paymentRepository;
            _curriculumRepository = curriculumRepository;
            _logger = log;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            OnProcess();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("AlertsBot.Services.StartVerifyAlerts stopping...");
            return Task.CompletedTask;
        }



        #region GetAlertsConfigs
        /// <summary>
        /// Recupera os alertas e suas mensagens
        /// </summary>
        /// <returns></returns>
        public static List<AlertaModel> GetAlerts()
        {
            chatIdAlertasBne = long.Parse(_configuration["chatIdGrupoAlertaBne"]);
            chatIdTriggerRelatorio = long.Parse(_configuration["chatIdTriggerRelatorio"]);
            chatIdGrupoAlertaRabbits = long.Parse(_configuration["chatIdGrupoAlertaRabbits"]);


            List<AlertaModel> alertasInject = new List<AlertaModel>();
            alertasInject.Add(new AlertaModel(1, "Cadastro de Empresa", "⚠️ [Cadastro de Empresa] Já faz mais de 2 horas que um novo cadastro de empresa não é realizado.", DateTime.Now, chatIdAlertasBne, 0, Convert.ToInt32(_configuration["Alertas:AlertaCadastroEmpresa:Delay"]), Convert.ToInt32(_configuration["Alertas:AlertaCadastroEmpresa:TipoHorario"])));
            alertasInject.Add(new AlertaModel(2, "Cadastro de vagas TBR", "⚠️ [Cadastro de vagas TBR] Já faz mais de 2 horas que uma vaga nova não é cadastrada.", DateTime.Now, chatIdAlertasBne, 0, Convert.ToInt32(_configuration["Alertas:AlertaCadastroVagaTbr:Delay"]), Convert.ToInt32(_configuration["Alertas:AlertaCadastroVagaTbr:TipoHorario"])));
            alertasInject.Add(new AlertaModel(3, "Alerta de CV's", "💎 [Alerta de CV's] - Hoje ({diaAtual}), foram indexados {qtCvs} cv's do TBR para o BNE. ", DateTime.Now, chatIdAlertasBne, 0, Convert.ToInt32(_configuration["Alertas:AlertaCvsIndexadosTbrParaBne:Delay"]), Convert.ToInt32(_configuration["Alertas:AlertaCvsIndexadosTbrParaBne:TipoHorario"]), Convert.ToString(_configuration["Alertas:AlertaCvsIndexadosTbrParaBne:HoraExecutar"])));
            alertasInject.Add(new AlertaModel(4, "Cadastro de CV's TBR", "⚠️ [Cadastro de CV's TBR] Já faz mais de 2 horas que um novo cadastro de cv não é realizado no TBR.", DateTime.Now, chatIdAlertasBne, 0, Convert.ToInt32(_configuration["Alertas:AlertaCadastroCvsTbr:Delay"]), Convert.ToInt32(_configuration["Alertas:AlertaCadastroCvsTbr:TipoHorario"])));
            alertasInject.Add(new AlertaModel(5, "Gerar Boleto BNE", "⚠️ [Gerar Boleto BNE] Já faz mais de 2 horas que um novo boleto não é gerado. [VIP e CIA]", DateTime.Now, chatIdAlertasBne, 0, Convert.ToInt32(_configuration["Alertas:AlertaGeracaoBoleto:Delay"]), Convert.ToInt32(_configuration["Alertas:AlertaGeracaoBoleto:TipoHorario"])));
            alertasInject.Add(new AlertaModel(6, "Gerar Cobrança Cartão", "⚠️ [Gerar Cobrança Cartão] Já faz mais de 2 horas que uma cobrança no cartão não é gerada.", DateTime.Now, chatIdAlertasBne, 0, Convert.ToInt32(_configuration["Alertas:AlertaGerarCobrancaCartao:Delay"]), Convert.ToInt32(_configuration["Alertas:AlertaGerarCobrancaCartao:TipoHorario"])));
            alertasInject.Add(new AlertaModel(7, "Vagas BNE para o TBR", "⚠️ [Vagas BNE para o TBR] As vagas anunciadas no TBR não foram para o BNE, verifique!.", DateTime.Now, chatIdAlertasBne, 0, Convert.ToInt32(_configuration["Alertas:AlertarSemVagsTbrNoBNE:Delay"]), Convert.ToInt32(_configuration["Alertas:AlertarSemVagsTbrNoBNE:TipoHorario"]), Convert.ToString(_configuration["Alertas:AlertarSemVagsTbrNoBNE:HoraExecutar"])));




            return alertasInject;
        }
        /// <summary>
        /// Recupera as filas que o alertabot deve ficar verificando, para implementar uma nova, basta incrementar no appsettings.json
        /// </summary>
        /// <returns></returns>
        public static List<RabbitMonitor> GetRabbitFilasVerify()
        {
            List<RabbitMonitor> filasMonitor = new List<RabbitMonitor>();
            var rabbitFilas = _configuration.GetSection("RabbitFilas").GetChildren();
            foreach (var fila in rabbitFilas)
            {
                filasMonitor.Add(new RabbitMonitor(fila["Name"], fila["VirtualHost"], fila["RabbitUrl"], fila["Delay"], fila["TipoHorario"]));
            }
            return filasMonitor;
        }
        /// <summary>
        /// Recupera as filas dos jornais para verificar possível fila sem consumer ou travamento
        /// </summary>
        /// <returns></returns>
        public static List<RabbitMonitor> GetJornais()
        {
            List<RabbitMonitor> jornaisMonitor = new List<RabbitMonitor>();
            var rabbitFilas = _configuration.GetSection("RabbitJornal").GetChildren();
            foreach (var fila in rabbitFilas)
            {
                jornaisMonitor.Add(new RabbitMonitor(fila["Name"], fila["VirtualHost"], fila["RabbitUrl"], fila["Delay"], fila["TipoHorario"], fila["HoraInicio"], fila["HoraFim"]));
            }
            return jornaisMonitor;
        }
        #endregion GetAlertsConfigs

        #region OnProcess
        /// <summary>
        /// Verificação continua para emitir alertas!
        /// </summary>
        public static void OnProcess()
        {
            while (true)
            {
                try
                {
                  
                    //Alertas das aplicações
                    AlertaCadastroEmpresa();
                    AlertaCadastroVagaTbr();
                    AlertaCvsIndexadosTbrParaBne();
                    AlertaCadastroCvsTbr();
                    AlertaGeracaoBoleto();
                    AlertaGerarCobrancaCartao();
                    AlertarSemVagsTbrNoBNE();

                    //Alertas do monitoramento de filas nos Rabbits
                    /*
                     TODO: É interessante implementar uma forma de pegar todas as filas do rabbit sem que seja inserido os nomes hardcode. Verificar
                     se existe alguma forma de recuperar os nomes de todas as filas, é possível que exista algum endpoint para isso.
                     Com isso não teria que inserir ou remover filas novas e antigas do appsettings.json
                     */
                    
                    AlertaRabbit();
                    //AlertaJornal();
                }
                catch (Exception ex)
                {

                    _logger.Error("AlertsBot -> Ocorreu um erro no monitoramento de alertas do AlertsBot", ex);
                }

            }
        }
        #endregion OnProcess

        #region ApplicationAlerts

        /// <summary>
        /// Alerta de Cadastro de Empresa
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="tipoHorario"></param>
        /// <param name="HoraEspecifica"></param>
        public static void AlertaCadastroEmpresa()
        {

            int delay = _alertsHistory[0].Delay;
            TipoHorario tipoHorario = _alertsHistory[0].TipoHorario;

            if (_alertsHistory[0].LastRuntime <= DateTime.Now)//Deve executar de 2 em 2 horas
            {

                DateTime verifyHour = DateTime.Now;
                bool SemCadastroEmpresaHorario = false;

                Task.Run(async () =>
                {
                    SemCadastroEmpresaHorario = await _companyService.VerifyLastSignUpCompanyHour(verifyHour);
                }).GetAwaiter().GetResult();



                //Só precisa notificar uma vez a cada duas horas
                if (_alertsHistory[0].SendAmount == 0 && SemCadastroEmpresaHorario)
                {
                    _handler.Handle(new SendMessageTelegramGroup(_alertsHistory[0].ChatId, _alertsHistory[0].Message));
                    _alertsHistory[0].SendAmount++;
                }
                if (tipoHorario == TipoHorario.Minutos)
                {
                    _alertsHistory[0].LastRuntime = DateTime.Now.AddMinutes(delay);
                }
                else if (tipoHorario == TipoHorario.Horas)
                {
                    _alertsHistory[0].LastRuntime = DateTime.Now.AddHours(delay);
                }
                //Envia relátorio para o grupo trigger, assim é possível verificar se a verificação dos alertas está sendo realizado.
                SendTriggerConfirm(_alertsHistory[0].AlertName, _alertsHistory[0].LastRuntime);
            }
            else
            {
                if (_alertsHistory[0].SendAmount != 0)
                {
                    _alertsHistory[0].SendAmount = 0;
                }

            }
        }
        /// <summary>
        /// Alerta de Cadastro de Vaga TBR
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="tipoHorario"></param>
        /// <param name="HoraEspecifica"></param>
        public static void AlertaCadastroVagaTbr()
        {
            int delay = _alertsHistory[1].Delay;
            TipoHorario tipoHorario = _alertsHistory[1].TipoHorario;

            if (_alertsHistory[1].LastRuntime <= DateTime.Now)
            {

                DateTime verifyHour = DateTime.Now;
                bool SemVagasHorario = false;
                Task.Run(async () =>
                {
                    SemVagasHorario = await _jobRepository.VerifyLastJobPosted(verifyHour);
                }).GetAwaiter().GetResult();



                //Só precisa notificar uma vez a cada duas horas
                if (_alertsHistory[1].SendAmount == 0 && SemVagasHorario)
                {
                    _handler.Handle(new SendMessageTelegramGroup(_alertsHistory[1].ChatId, _alertsHistory[1].Message));
                    _alertsHistory[1].SendAmount++;

                }

                if (tipoHorario == TipoHorario.Minutos)
                {
                    _alertsHistory[1].LastRuntime = DateTime.Now.AddMinutes(delay);
                }
                else if (tipoHorario == TipoHorario.Horas)
                {
                    _alertsHistory[1].LastRuntime = DateTime.Now.AddHours(delay);
                }
                //Envia relátorio para o grupo trigger, assim é possível verificar se a verificação dos alertas está sendo realizado.
                SendTriggerConfirm(_alertsHistory[1].AlertName, _alertsHistory[1].LastRuntime);

            }
            else
            {
                if (_alertsHistory[1].SendAmount != 0)
                {
                    _alertsHistory[1].SendAmount = 0;
                }
            }

        }
        /// <summary>
        /// Alerta de cvs indexados TBR para BNE
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="tipoHorario"></param>
        /// <param name="HoraEspecifica"></param>
        public static void AlertaCvsIndexadosTbrParaBne()
        {
            int delay = _alertsHistory[2].Delay;
            TipoHorario tipoHorario = _alertsHistory[2].TipoHorario;
            string HoraEspecifica = _alertsHistory[2].HoraEspecifica;
            if (tipoHorario == TipoHorario.HoraEspecifica && !string.IsNullOrEmpty(HoraEspecifica))
            {
                var horaRodar = DateTime.ParseExact(HoraEspecifica, "H:mm", null, System.Globalization.DateTimeStyles.None);
                if (DateTime.Now >= horaRodar && DateTime.Now <= horaRodar.AddMinutes(5))//Intervalo válido de diferença aceitavel entre a hora atual e a hora especifica, não mudar!
                {
                    if (_alertsHistory[2].SendAmount == 0)
                    {

                        int AmountCvsIndex = 0;
                        Task.Run(async () =>
                        {
                            AmountCvsIndex = await _curriculumRepository.VerifyAmountIndexCvs();
                        }).GetAwaiter().GetResult();

                        if (AmountCvsIndex > 0)
                        {
                            _handler.Handle(new SendMessageTelegramGroup(_alertsHistory[2].ChatId, _alertsHistory[2].Message.Replace("{diaAtual}", DateTime.Now.Date.ToString("dd/MM/yyyy")).Replace("{qtCvs}", AmountCvsIndex.ToString())));
                            _alertsHistory[2].SendAmount = 1;
                        }
                        else
                        {
                            _handler.Handle(new SendMessageTelegramGroup(_alertsHistory[2].ChatId, "Nenhum cv's foi indexado do TBR para o BNE, Verifique!"));
                            _alertsHistory[2].SendAmount = 1;
                        }
                        //Envia relátorio para o grupo trigger, assim é possível verificar se a verificação dos alertas está sendo realizado.
                        SendTriggerConfirmHoraSpecificTime(_alertsHistory[2].AlertName, Convert.ToDateTime(HoraEspecifica));

                    }
                }
                else
                {
                    _alertsHistory[2].SendAmount = 0;
                }
            }
        }
        /// <summary>
        /// Alerta de cadastro de cvs no TBR
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="tipoHorario"></param>
        /// <param name="HoraEspecifica"></param>
        public static void AlertaCadastroCvsTbr()
        {
            int delay = _alertsHistory[3].Delay;
            TipoHorario tipoHorario = _alertsHistory[3].TipoHorario;

            if (_alertsHistory[3].LastRuntime <= DateTime.Now)
            {

                DateTime verifyHour = DateTime.Now;
                bool SemVagasHorario = false;
                Task.Run(async () =>
                {
                    SemVagasHorario = await _curriculumRepository.VerifyLastCvSignUpTBR(verifyHour);
                }).GetAwaiter().GetResult();



                //Só precisa notificar uma vez a cada duas horas
                if (_alertsHistory[3].SendAmount == 0 && SemVagasHorario)
                {
                    _handler.Handle(new SendMessageTelegramGroup(_alertsHistory[3].ChatId, _alertsHistory[3].Message));
                    _alertsHistory[3].SendAmount++;

                }

                if (tipoHorario == TipoHorario.Minutos)
                {
                    _alertsHistory[3].LastRuntime = DateTime.Now.AddMinutes(delay);
                }
                else if (tipoHorario == TipoHorario.Horas)
                {
                    _alertsHistory[3].LastRuntime = DateTime.Now.AddHours(delay);
                }
                //Envia relátorio para o grupo trigger, assim é possível verificar se a verificação dos alertas está sendo realizado.
                SendTriggerConfirm(_alertsHistory[3].AlertName, _alertsHistory[3].LastRuntime);

            }
            else
            {
                if (_alertsHistory[3].SendAmount != 0)
                {
                    _alertsHistory[3].SendAmount = 0;
                }
            }
        }
        /// <summary>
        /// Alerta de Geração de Boleto BNE
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="tipoHorario"></param>
        /// <param name="HoraEspecifica"></param>
        public static void AlertaGeracaoBoleto()
        {

            int delay = _alertsHistory[4].Delay;
            TipoHorario tipoHorario = _alertsHistory[4].TipoHorario;

            if (_alertsHistory[4].LastRuntime <= DateTime.Now)
            {

                DateTime verifyHour = DateTime.Now;
                bool SemVagasHorario = false;
                Task.Run(async () =>
                {
                    SemVagasHorario = await __paymentRepository.VerifyLastBillet(verifyHour);
                }).GetAwaiter().GetResult();



                //Só precisa notificar uma vez a cada duas horas
                if (_alertsHistory[4].SendAmount == 0 && SemVagasHorario)
                {
                    _handler.Handle(new SendMessageTelegramGroup(_alertsHistory[4].ChatId, _alertsHistory[4].Message));
                    _alertsHistory[4].SendAmount++;

                }

                if (tipoHorario == TipoHorario.Minutos)
                {
                    _alertsHistory[4].LastRuntime = DateTime.Now.AddMinutes(delay);
                }
                else if (tipoHorario == TipoHorario.Horas)
                {
                    _alertsHistory[4].LastRuntime = DateTime.Now.AddHours(delay);
                }
                //Envia relátorio para o grupo trigger, assim é possível verificar se a verificação dos alertas está sendo realizado.
                SendTriggerConfirm(_alertsHistory[4].AlertName, _alertsHistory[4].LastRuntime);

            }
            else
            {
                if (_alertsHistory[4].SendAmount != 0)
                {
                    _alertsHistory[4].SendAmount = 0;
                }
            }
        }
        /// <summary>
        /// Alerta de Geração de Cobrança de Boleto
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="tipoHorario"></param>
        /// <param name="HoraEspecifica"></param>
        public static void AlertaGerarCobrancaCartao()
        {

            int delay = _alertsHistory[5].Delay;
            TipoHorario tipoHorario = _alertsHistory[5].TipoHorario;

            if (_alertsHistory[5].LastRuntime <= DateTime.Now)
            {

                DateTime verifyHour = DateTime.Now;
                bool SemVagasHorario = false;
                Task.Run(async () =>
                {
                    SemVagasHorario = await __paymentRepository.VerifyLastCreditCard(verifyHour);
                }).GetAwaiter().GetResult();



                //Só precisa notificar uma vez a cada duas horas
                if (_alertsHistory[5].SendAmount == 0 && SemVagasHorario)
                {
                    _handler.Handle(new SendMessageTelegramGroup(_alertsHistory[5].ChatId, _alertsHistory[5].Message));
                    _alertsHistory[5].SendAmount++;

                }

                if (tipoHorario == TipoHorario.Minutos)
                {
                    _alertsHistory[5].LastRuntime = DateTime.Now.AddMinutes(delay);
                }
                else if (tipoHorario == TipoHorario.Horas)
                {
                    _alertsHistory[5].LastRuntime = DateTime.Now.AddHours(delay);
                }

                //Envia relátorio para o grupo trigger, assim é possível verificar se a verificação dos alertas está sendo realizado.
                SendTriggerConfirm(_alertsHistory[5].AlertName, _alertsHistory[5].LastRuntime);
            }
            else
            {
                if (_alertsHistory[5].SendAmount != 0)
                {
                    _alertsHistory[5].SendAmount = 0;
                }
            }
        }
        public static void AlertarSemVagsTbrNoBNE()
        {

            int delay = _alertsHistory[6].Delay;
            TipoHorario tipoHorario = _alertsHistory[6].TipoHorario;
            string HoraEspecifica = _alertsHistory[6].HoraEspecifica;
            if (tipoHorario == TipoHorario.HoraEspecifica && !string.IsNullOrEmpty(HoraEspecifica))
            {
                var horaRodar = DateTime.ParseExact(HoraEspecifica, "H:mm", null, System.Globalization.DateTimeStyles.None);
                if (DateTime.Now >= horaRodar && DateTime.Now <= horaRodar.AddMinutes(5))//Intervalo válido de diferença aceitavel entre a hora atual e a hora especifica, não mudar!
                {
                    if (_alertsHistory[6].SendAmount == 0)
                    {

                        int AmountJobsIndexTbrInBne = -1;
                        Task.Run(async () =>
                        {
                            AmountJobsIndexTbrInBne = await _jobRepository.VerifyAmountJobsTbrInBne();
                        }).GetAwaiter().GetResult();

                        if (AmountJobsIndexTbrInBne == 0)
                        {
                            _handler.Handle(new SendMessageTelegramGroup(_alertsHistory[6].ChatId, _alertsHistory[6].Message));
                            _alertsHistory[6].SendAmount = 1;
                        }
                        //Envia relátorio para o grupo trigger, assim é possível verificar se a verificação dos alertas está sendo realizado.
                        SendTriggerConfirmHoraSpecificTime(_alertsHistory[6].AlertName, Convert.ToDateTime(HoraEspecifica));

                    }
                }
                else
                {
                    _alertsHistory[6].SendAmount = 0;
                }
            }
        }

        #endregion ApplicationAlerts

        #region RabbitAlerts
        ///Alerta de Filas Rabbits
        public static void AlertaRabbit()
        {

            //Atenção: Para inserir uma nova fila para monitoração, siga o padrão de Queue na appsettings desse projeto.
            if (_filasMonitor.Count > 0)
            {
                foreach (var fila in _filasMonitor)
                {
                    if (fila.LastRuntime <= DateTime.Now)
                    {

                        RabbitQueue result = new RabbitQueue();

                        result = Helpers.RabbitHelper.GetRabbitQueue(fila.RabbitUrl, fila.VirtualHost, fila.Name);
                        var message_deliver = (result.message_stats != null ? result.message_stats.deliver_get_details : null);
                        var lastConsumerRate = (message_deliver != null ? message_deliver.rate : -1.0);

                        RabbitQueue retornoConfirm = new RabbitQueue();
                        bool semConsumer = false;
                        if (result != null)
                        {
                            if (Convert.ToInt32(result.messages) > 0 && Convert.ToInt32(result.consumers) == 0 && lastConsumerRate != -1.0 && lastConsumerRate == 0.0)
                            {
                                retornoConfirm = Helpers.RabbitHelper.GetRabbitQueue(fila.RabbitUrl, fila.VirtualHost, fila.Name);
                                if (lastConsumerRate == 0.0)
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        retornoConfirm = Helpers.RabbitHelper.GetRabbitQueue(fila.RabbitUrl, fila.VirtualHost, fila.Name);
                                        if (retornoConfirm.message_stats.deliver_get_details != null)
                                        {
                                            if (retornoConfirm.message_stats.deliver_get_details.rate == lastConsumerRate && Convert.ToInt32(retornoConfirm.messages) > 0)
                                            {
                                                semConsumer = true;
                                            }
                                            else
                                            {
                                                semConsumer = false;
                                            }
                                        }

                                    }
                                }
                            }
                        }


                        if (fila.SendAmount == 0 && semConsumer)
                        {
                            string message = string.Format("⚠️ [Rabbit {0}] \n\n  ⬇️ Fila ⬇️\n  {1} \n\n ➢ A fila possui {2} msg enfileiradas e está sem consumer.", fila.RabbitUrl.Replace("http://", ""), fila.Name.Replace("%2B", "+"), result.messages);
                            _handler.Handle(new SendMessageTelegramGroup(chatIdGrupoAlertaRabbits, message));
                            fila.SendAmount++;


                        }
                        else if (result.message_stats != null)
                        {
                            if (result.message_stats.redeliver_details != null)
                            {
                                if (fila.SendAmount == 0 && result.message_stats.redeliver_details.rate > 0 && !semConsumer)
                                {
                                    string message = string.Format("❕ [Rabbit {0}] \n\n  ⬇️ Fila ⬇️\n  {1} \n\n ➢ A fila está sofrendo redelivered!.", fila.RabbitUrl.Replace("http://", ""), fila.Name.Replace("%2B", "+"), result.messages);
                                    _handler.Handle(new SendMessageTelegramGroup(chatIdGrupoAlertaRabbits, message));
                                    fila.SendAmount++;
                                }
                            }
                        }


                        if (fila.TipoHorario == TipoHorario.Minutos)
                        {
                            fila.LastRuntime = DateTime.Now.AddMinutes(fila.Delay);
                        }
                        else if (fila.TipoHorario == TipoHorario.Horas)
                        {
                            fila.LastRuntime = DateTime.Now.AddHours(fila.Delay);
                        }

                        //Envia relátorio para o grupo trigger, assim é possível verificar se a verificação dos alertas está sendo realizado.
                        SendTriggerConfirmRabbit(fila.Name, fila.LastRuntime);
                    }
                    else
                    {
                        if (fila.SendAmount != 0)
                        {
                            fila.SendAmount = 0;
                        }
                    }
                }
            }
        }
        #endregion RabbitAlerts

        #region JornalAlerts
        /// <summary>
        /// Realiza a verificação das filas dos jornais, verifica se está sem consumer ou se a fila travou
        /// </summary>
        public static void AlertaJornal()
        {
            if (_jornalMonitor.Count > 0)
            {
                foreach (var fila in _jornalMonitor)
                {
                    var horaInicio = DateTime.ParseExact(fila.HoraInicio, "H:mm", null, System.Globalization.DateTimeStyles.None);
                    var horaFim = DateTime.ParseExact(fila.HoraFim, "H:mm", null, System.Globalization.DateTimeStyles.None);

                    if (DateTime.Now >= horaInicio && DateTime.Now <= horaFim && fila.LastRuntime <= DateTime.Now)
                    {
                        RabbitQueue result = new RabbitQueue();

                        result = Helpers.RabbitHelper.GetRabbitQueue(fila.RabbitUrl, fila.VirtualHost, fila.Name);
                        var message_deliver = (result.message_stats != null ? result.message_stats.deliver_get_details : null);
                        var lastConsumerRate = (message_deliver != null ? message_deliver.rate : -1.0);

                        bool semConsumer = false;
                        if (result != null)
                        {
                            if (Convert.ToInt32(result.messages) > 0 && Convert.ToInt32(result.consumers) == 0 && lastConsumerRate != -1.0 && lastConsumerRate == 0.0)
                            {
                                semConsumer = true;
                            }
                        }
                        if (fila.SendAmount == 0 && semConsumer)
                        {
                            string message = string.Format("⚠️[Jornal - Rabbit {0}] \n\n  ⬇️ Fila ⬇️\n  {1} \n\n ➢ A fila possui {2} msg enfileiradas e está sem consumer.", fila.RabbitUrl.Replace("http://", ""), fila.Name.Replace("%2B", "+"), result.messages);
                            _handler.Handle(new SendMessageTelegramGroup(chatIdAlertasBne, message));
                            fila.SendAmount++;

                        }
                        else if (fila.SendAmount == 0 && result.message_stats.redeliver_details.rate > 0 && !semConsumer)
                        {
                            string message = string.Format("⚠️[Jornal - Rabbit {0}] \n\n  ⬇️ Fila ⬇️\n  {1} \n\n ➢ A fila está sofrendo redelivered!.", fila.RabbitUrl.Replace("http://", ""), fila.Name.Replace("%2B", "+"), result.messages);
                            _handler.Handle(new SendMessageTelegramGroup(chatIdGrupoAlertaRabbits, message));
                            fila.SendAmount++;
                        }

                        else
                        {
                            //Logo a fila tem consumer, porém pode não ter mensagens, agora para verificar se o jornal não travou, refazer o request novamente.
                            bool filaTravou = false;
                            if (lastConsumerRate == 0.0 && Convert.ToInt32(result.messages) > 0 && !semConsumer)
                            {
                                for (int i = 0; i < 10; i++)
                                {
                                    result = Helpers.RabbitHelper.GetRabbitQueue(fila.RabbitUrl, fila.VirtualHost, fila.Name);

                                    
                                    if (result.message_stats != null)
                                    {
                                        if(result.message_stats.deliver_get_details != null)
                                        {
                                            var ConsumerRateNow = (message_deliver != null ? message_deliver.rate : -1.0);

                                            if (ConsumerRateNow == lastConsumerRate)
                                            {
                                                filaTravou = true;
                                            }
                                            else
                                            {
                                                filaTravou = false;
                                            }
                                        }
                                    }

                                }
                            }

                            if (filaTravou && fila.SendAmount == 0)
                            {
                                string message = string.Format("⚠️[Jornal - Rabbit {0}] \n\n  ⬇️ Fila ⬇️\n  {1} \n\n ➢ É possível que o jornal tenha travado, verifique! ", fila.RabbitUrl.Replace("http://", ""), fila.Name.Replace("%2B", "+"), result.messages);
                                _handler.Handle(new SendMessageTelegramGroup(chatIdAlertasBne, message));
                            }
                        }

                        if (fila.TipoHorario == TipoHorario.Minutos)
                        {
                            fila.LastRuntime = DateTime.Now.AddMinutes(fila.Delay);
                        }
                        else if (fila.TipoHorario == TipoHorario.Horas)
                        {
                            fila.LastRuntime = DateTime.Now.AddHours(fila.Delay);
                        }

                        //Envia relátorio para o grupo trigger, assim é possível verificar se a verificação dos alertas está sendo realizado.
                        SendTriggerConfirmRabbit(fila.Name, fila.LastRuntime);
                    }
                    else
                    {
                        if (fila.SendAmount != 0)
                        {
                            fila.SendAmount = 0;
                        }
                    }
                }
            }
        }
        #endregion JornalAlerts

        /**As triggers Monitoring tem como objetivo enviar alertas para um grupo no telegram escolhido sempre que os processos de verificação terminarem, o alerta
        avisará o horário da próxima execução. Por padrão deixar desativado para não afetar o tempo de envio das mensagens.**/
        #region TriggersMonitoringTelegramGroup
        public static void SendTriggerConfirmRabbit(string name, DateTime hour)
        {
            bool ativaTrigger = Convert.ToBoolean(_configuration["AtivaTrigger"]);
            if (ativaTrigger)
            {
                string messageTriggerEnd = string.Format("✅ [Rabbit - {0}] Terminou a verificação! - Próxima verificação: {1}", name.Replace("%2B", "+"), hour.ToString("H:mm"));
                _handler.Handle(new SendMessageTelegramGroup(chatIdTriggerRelatorio, messageTriggerEnd));
            }

        }
        public static void SendTriggerConfirm(string name, DateTime hour)
        {

            bool ativaTrigger = Convert.ToBoolean(_configuration["AtivaTrigger"]);
            if (ativaTrigger)
            {
                string messageTriggerEnd = string.Format("✅ [{0}] Terminou a verificação! - Próxima verificação: {1}", name, hour.ToString("H:mm"));
                _handler.Handle(new SendMessageTelegramGroup(chatIdTriggerRelatorio, messageTriggerEnd));
            }


        }
        public static void SendTriggerConfirmHoraSpecificTime(string name, DateTime hour)
        {

            bool ativaTrigger = Convert.ToBoolean(_configuration["AtivaTrigger"]);
            if (ativaTrigger)
            {
                string messageTriggerEnd = string.Format("✅ [{0}] Terminou a verificação! - Próxima verificação amanhã ás: {1}", name, hour.ToString("H:mm"));
                _handler.Handle(new SendMessageTelegramGroup(chatIdTriggerRelatorio, messageTriggerEnd));
            }


        }
        #endregion TriggersMonitoringTelegramGroup



        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}