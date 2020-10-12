namespace LessPaper.WriteService.Options
{
    public class AppSettings
    {
        public ValidationRules ValidationRules { get; set; }

        public MinioSettings Minio { get; set; }

        public RabbitMqSettings RabbitMq { get; set; }

        public GuardClientSettings GuardService { get; set; }
    }


    public class ValidationRules
    {

        public int MaxFileSizeInBytes { get; set; }
    }
}
