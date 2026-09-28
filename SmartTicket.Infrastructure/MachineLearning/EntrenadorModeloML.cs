using Microsoft.ML;
using SmartTicket.Application.Services.Clasificacion.ClasificacionML;

namespace SmartTicket.Infrastructure.MachineLearning;

public class EntrenadorModeloML(string rutaModelo) : IEntrenadorModeloML
{
    private readonly string _rutaModelo = rutaModelo;

    public ResultadoEntrenamiento EntrenarDesdeCsv(string rutaCsv)
    {
        var mlContext = new MLContext(seed: 42);

        IDataView dataViewCompleto = mlContext.Data.LoadFromTextFile<SolicitudTextoInput>(
            path: rutaCsv,
            hasHeader: true,
            separatorChar: ',');

        // 80% entrenamiento / 20% prueba -> la precisión que reporta es sobre
        // datos que el modelo NUNCA vio, por eso es una métrica real y defendible
        var split = mlContext.Data.TrainTestSplit(dataViewCompleto, testFraction: 0.2, seed: 42);

        var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(SolicitudTextoInput.Categoria))
            .Append(mlContext.Transforms.Text.FeaturizeText("Features", nameof(SolicitudTextoInput.Texto)))
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        var modeloEvaluacion = pipeline.Fit(split.TrainSet);
        var predicciones = modeloEvaluacion.Transform(split.TestSet);
        var metricas = mlContext.MulticlassClassification.Evaluate(predicciones);

        // Modelo final: se reentrena con el 100% de los datos y ES el que se guarda/usa
        var modeloFinal = pipeline.Fit(dataViewCompleto);
        mlContext.Model.Save(modeloFinal, dataViewCompleto.Schema, _rutaModelo);

        var total = mlContext.Data.CreateEnumerable<SolicitudTextoInput>(dataViewCompleto, reuseRowObject: false).Count();

        return new ResultadoEntrenamiento(
            MicroAccuracy: metricas.MicroAccuracy,
            MacroAccuracy: metricas.MacroAccuracy,
            LogLoss: metricas.LogLoss,
            TotalRegistros: total);
    }
}
