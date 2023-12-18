namespace MVCWebAppIsmane.Security.service
{
    public interface IJwtService
    {
        public string GenerateJwtToken(string email,string role);// You can pass other relevant user data here
    }

}
