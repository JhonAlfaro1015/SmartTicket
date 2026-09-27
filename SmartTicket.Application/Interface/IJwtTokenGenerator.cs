using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Interface
{
    public interface IJwtTokenGenerator
    {
        (string Token, DateTime ExpiraEn) GenerateToken(Tbl_Usuario usuario, string rolNombre);
    }
}
