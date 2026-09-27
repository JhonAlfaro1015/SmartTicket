using SmartTicket.Domain.Entity;

namespace SmartTicket.Application.Interface;

public interface IClienteRepository
{
    Task<int> CreateAsync(Tbl_Cliente cliente);
    Task<Tbl_Cliente?> GetByIdAsync(int clienteId);
}
