using Microsoft.ML;
using SmartTicket.Application.Services.Clasificacion.ClasificacionML;

namespace SmartTicket.Infrastructure.MachineLearning;

internal static class RutasModelo
{
    public static string Categoria(string directorio) => Path.Combine(directorio, "modelo_categoria.zip");
    public static string Prioridad(string directorio) => Path.Combine(directorio, "modelo_prioridad.zip");
}

public class EntrenadorModeloML(string directorioModelos) : IEntrenadorModeloML
{
    private readonly string _directorioModelos = directorioModelos;

    public ResultadoEntrenamiento EntrenarDesdeCsv(string rutaCsv)
    {
        var mlContext = new MLContext(seed: 42);

        IDataView dataView = mlContext.Data.LoadFromTextFile<SolicitudTextoInput>(
            path: rutaCsv,
            hasHeader: true,
            separatorChar: ',',
            allowQuoting: true);

        var metricasCategoria = EntrenarYGuardar(
            mlContext, dataView, nameof(SolicitudTextoInput.Categoria), RutasModelo.Categoria(_directorioModelos));

        var metricasPrioridad = EntrenarYGuardar(
            mlContext, dataView, nameof(SolicitudTextoInput.Prioridad), RutasModelo.Prioridad(_directorioModelos));

        var total = mlContext.Data.CreateEnumerable<SolicitudTextoInput>(dataView, reuseRowObject: false).Count();

        return new ResultadoEntrenamiento(metricasCategoria, metricasPrioridad, total);
    }

    private static MetricasModelo EntrenarYGuardar(
        MLContext mlContext, IDataView dataView, string columnaEtiqueta, string rutaModelo)
    {
        // 80% entrenamiento / 20% prueba: la métrica sale de datos que el modelo no vio
        var split = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2, seed: 42);

        var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", columnaEtiqueta)
            .Append(mlContext.Transforms.Text.FeaturizeText("Features", nameof(SolicitudTextoInput.Texto)))
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        var modeloEvaluacion = pipeline.Fit(split.TrainSet);
        var metricas = mlContext.MulticlassClassification.Evaluate(modeloEvaluacion.Transform(split.TestSet));

        // Modelo final: se reentrena con el 100% de los datos y es el que se guarda
        var modeloFinal = pipeline.Fit(dataView);
        mlContext.Model.Save(modeloFinal, dataView.Schema, rutaModelo);

        return new MetricasModelo(metricas.MicroAccuracy, metricas.MacroAccuracy, metricas.LogLoss);
    }
}
