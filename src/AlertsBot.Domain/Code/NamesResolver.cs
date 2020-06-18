using Microsoft.Extensions.Configuration;
using SharedKernel.DomainEvents.CrossDomainEvents;
using System;
using System.Linq;

namespace AlertsBot.Domain.Code
{
    public class NamesResolver : DefaultNamesResolver
    {
        public NamesResolver()
        { }
        public NamesResolver(IConfiguration configuration)
        {
            Namespace = configuration.GetChildren().Any(item => item.Key == "Namespace") ?
                                configuration["Namespace"] : null;
        }

        public NamesResolver(string name, string nameExchange)
        {
            Namespace = name;
            NameExchange = nameExchange;
        }

        public string Namespace { get; set; }
        public string NameExchange { get; set; }

        public override string ExchangeName(Type type)
        {
            var name = base.ExchangeName(type);

            if (!string.IsNullOrWhiteSpace(Namespace))
                name = $"{name}[{Namespace}]";

            if (!string.IsNullOrEmpty(NameExchange))
                name = NameExchange;

            return name;
        }

        public override string QueueName<T>(Action<T> onEvent)
        {
            var name = base.QueueName(onEvent);

            if (!string.IsNullOrWhiteSpace(Namespace))
                name = $"{name}[{Namespace}]";

            return name;
        }
    }
}

