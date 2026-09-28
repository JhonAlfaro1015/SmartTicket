using Microsoft.ML.Data;

namespace SmartTicket.Infrastructure.MachineLearning;

public class SolicitudCategoriaPrediction
{
    [ColumnName("PredictedLabel")]
    public string CategoriaPredicha { get; set; } = null!;

    public float[] Score { get; set; } = null!;
}
