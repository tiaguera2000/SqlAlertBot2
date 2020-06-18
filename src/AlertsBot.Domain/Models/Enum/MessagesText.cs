namespace AlertsBot.Domain.Models.Enum
{
    public static class MessagesText
    {
        public const string nenhumParametro = "Por favor, insira um paramenro após o comando";

        public const string limite = "Quantidade excedida";

        public const string nenhumaInformacao = "Nenhuma informação encontrada";

        public const string erroCancelar = "Problema ao cancelar plano";

        public const string sucessoCancelar = "Sucesso ao cancelar plano";

        public const string emailBloqueado = "Bloqueado";

        public const string emailLiberado = "Liberado";

        public const string help = "/get exemplo@exemplo.com - verifica um email\n\n" +
                                   "/block exemplo@exemplo.com - bloqueia um email\n\n" +
                                   "/unblock exemplo@exemplo.com - desbloqueia um email\n\n" +
                                   "/cpf cpf - verifica qual email esta vinculado a esse cpf\n\n" +
                                   "/email exemplo@exemplo.com - verifica qual cpf esta vinculado a esse email\n\n" +
                                   "/cartao 0000%0000 - verifica as ultimas transações feitas no cartão\n\n" +
                                   "/vaga idf_vaga - verifica uma vaga no bne\n\n" +
                                   "/pf cpf - verifica um candidato\n\n" +
                                   "/plano cnpj - verificar os planos do cnpj informado\n\n" +
                                   "/cancelaplano IdPlano - cancela o plano informado\n\n" +
                                   "** OBS: Substituir os parametros após o comando pelos valores que deseja executar **";
    }
}
