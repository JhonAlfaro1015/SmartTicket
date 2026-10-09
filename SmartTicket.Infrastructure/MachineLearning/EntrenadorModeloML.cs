using Microsoft.ML;
using Microsoft.ML.Data;
using SmartTicket.Application.Services.Clasificacion.ClasificacionML;

namespace SmartTicket.Infrastructure.MachineLearning;

internal static class RutasModelo
{
    public static string Categoria(string directorio) => Path.Combine(directorio, "modelo_categoria.zip");
    public static string Prioridad(string directorio) => Path.Combine(directorio, "modelo_prioridad.zip");
}

public class EntrenadorModeloML : IEntrenadorModeloML
{
    private readonly string _directorioModelos;

    public EntrenadorModeloML(string directorioModelos)
    {
        _directorioModelos = directorioModelos;
    }

    public ResultadoEntrenamiento EntrenarDesdeCsv(string rutaEntrenamiento, string? rutaValidacion = null)
    {
        var mlContext = new MLContext(seed: 42);

        var entrenamiento = Cargar(mlContext, rutaEntrenamiento);
        IDataView? validacion = !string.IsNullOrEmpty(rutaValidacion) && File.Exists(rutaValidacion)
            ? Cargar(mlContext, rutaValidacion)
            : null;

        var categoria = EntrenarYGuardar(
            mlContext, entrenamiento, validacion,
            nameof(SolicitudTextoInput.Categoria), RutasModelo.Categoria(_directorioModelos));

        var prioridad = EntrenarYGuardar(
            mlContext, entrenamiento, validacion,
            nameof(SolicitudTextoInput.Prioridad), RutasModelo.Prioridad(_directorioModelos));

        return new ResultadoEntrenamiento(
            categoria,
            prioridad,
            Contar(mlContext, entrenamiento),
            validacion is null ? 0 : Contar(mlContext, validacion),
            validacion is null
                ? "División aleatoria 80/20 (puede salir inflada con datos generados)"
                : "Archivo de validación independiente");
    }

    private static IDataView Cargar(MLContext mlContext, string ruta) =>
        mlContext.Data.LoadFromTextFile<SolicitudTextoInput>(
            path: ruta,
            hasHeader: true,
            separatorChar: ',',
            allowQuoting: true);   // necesario: hay textos con comas dentro

    private static int Contar(MLContext mlContext, IDataView datos) =>
        mlContext.Data.CreateEnumerable<SolicitudTextoInput>(datos, reuseRowObject: false).Count();

    private static MetricasModelo EntrenarYGuardar(
        MLContext mlContext, IDataView entrenamiento, IDataView? validacion,
        string columnaEtiqueta, string rutaModelo)
    {
        var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", columnaEtiqueta)
            .Append(mlContext.Transforms.Text.FeaturizeText("Features", nameof(SolicitudTextoInput.Texto)))
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        var modeloFinal = pipeline.Fit(entrenamiento);

        MulticlassClassificationMetrics metricas;
        if (validacion is not null)
        {
            metricas = mlContext.MulticlassClassification.Evaluate(modeloFinal.Transform(validacion));
        }
        else
        {
            var split = mlContext.Data.TrainTestSplit(entrenamiento, testFraction: 0.2, seed: 42);
            var modeloEvaluacion = pipeline.Fit(split.TrainSet);
            metricas = mlContext.MulticlassClassification.Evaluate(modeloEvaluacion.Transform(split.TestSet));
        }

        mlContext.Model.Save(modeloFinal, entrenamiento.Schema, rutaModelo);

        return new MetricasModelo(metricas.MicroAccuracy, metricas.MacroAccuracy, metricas.LogLoss);
    }
}
