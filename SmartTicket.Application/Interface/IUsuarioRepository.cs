using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Interface
{
    public interface IUsuarioRepository
    {
        Task<Tbl_Usuario?> GetByEmailAsync(string email);
        Task<int> CreateAsync(Tbl_Usuario usuario);
    }
}