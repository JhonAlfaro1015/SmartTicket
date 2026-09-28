namespace SmartTicket.Application.Interface
{
    public interface IJwtTokenGenerator
    {
        (string Token, DateTime ExpiraEn) GenerateToken(int id, string email, string nombre, string rolNombre);
    }
}
