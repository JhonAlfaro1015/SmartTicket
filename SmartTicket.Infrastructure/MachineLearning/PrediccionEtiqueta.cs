using Microsoft.ML.Data;

namespace SmartTicket.Infrastructure.MachineLearning;

public class PrediccionEtiqueta
{
    [ColumnName("PredictedLabel")]
    public string Etiqueta { get; set; } = null!;

    public float[] Score { get; set; } = null!;
}
