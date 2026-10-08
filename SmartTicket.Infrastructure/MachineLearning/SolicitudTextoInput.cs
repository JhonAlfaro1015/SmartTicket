using Microsoft.ML.Data;

namespace SmartTicket.Infrastructure.MachineLearning;

public class SolicitudTextoInput
{
    [LoadColumn(3)]
    public string Texto { get; set; } = null!;

    [LoadColumn(4)]
    public string Categoria { get; set; } = null!;

    [LoadColumn(6)]
    public string Prioridad { get; set; } = null!;
}