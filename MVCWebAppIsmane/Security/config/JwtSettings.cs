namespace MVCWebAppIsmane.Security.config
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public int ExpiryInHours { get; set; }

        public JwtSettings()
        {
            SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        }
    }

}
