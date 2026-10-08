namespace SmartTicket.Application.Common;

public static class RolesSistema
{
    public const string Administrador = "Administrador";
    public const string Supervisor = "Supervisor";
    public const string Agente = "Agente";
    public const string Cliente = "Cliente";

    public const string Personal = Administrador + "," + Supervisor + "," + Agente;
    public const string Gestion = Administrador + "," + Supervisor;
}
