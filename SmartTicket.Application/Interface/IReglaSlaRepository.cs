namespace SmartTicket.Application.Interface;

public interface IReglaSlaRepository
{
    Task<int?> GetHorasSlaAsync(int? subcategoriaId, int prioridadId);
}
