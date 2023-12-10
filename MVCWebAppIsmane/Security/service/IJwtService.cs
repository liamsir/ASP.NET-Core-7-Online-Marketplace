namespace MVCWebAppIsmane.Security.service
{
    public interface IJwtService
    {
        public string GenerateJwtToken(string email);// You can pass other relevant user data here
    }

}
