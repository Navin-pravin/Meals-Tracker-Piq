namespace AljasAuthApi.Config
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = "43.204.40.208";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "myuser";
        public string Password { get; set; } = "mypassword";
        public string ExchangeName { get; set; } = "VisitorExchange";
        // public string QueueName { get; set; } = "visitor.created";
    }
}
