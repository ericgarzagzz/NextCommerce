namespace NextCommerce.Models.Configurations
{
    public class EmailSettings
    {
        private EmailSettings() { }

        public static EmailSettings Instance { get; protected set; } = new EmailSettings();

        public string Type { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public SendGridSettings? SendGrid { get; set; }
        public SmtpSettings? Smtp { get; set; }

        public class SendGridSettings
        {
            public bool Sandbox { get; set; } = false;
            public string Key { get; set; }
        }

        public class SmtpSettings
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public bool Ssl { get; set; }
        }
    }
}
