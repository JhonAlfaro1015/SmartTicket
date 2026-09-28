using Microsoft.ML;
using SmartTicket.Application.Services.Clasificacion.ClasificacionML;

namespace SmartTicket.Infrastructure.MachineLearning;

public class MotorClasificacionML : IMotorClasificacionML
{
    private readonly object _bloqueo = new();
    private readonly PredictionEngine<SolicitudTextoInput, SolicitudCategoriaPrediction>? _predictionEngine;

    public MotorClasificacionML(string rutaModelo)
    {
        if (File.Exists(rutaModelo))
        {
            var mlContext = new MLContext(seed: 42);
            var modelo = mlContext.Model.Load(rutaModelo, out _);
            _predictionEngine = mlContext.Model.CreatePredictionEngine<SolicitudTextoInput, SolicitudCategoriaPrediction>(modelo);
        }
    }

    public (string CategoriaPredicha, float Confianza) PredecirCategoria(string texto)
    {
        if (_predictionEngine is null)
            throw new InvalidOperationException(
                "El modelo de ML aún no ha sido entrenado. Ejecuta POST /api/ml/entrenar primero.");

        lock (_bloqueo) // PredictionEngine no es thread-safe
        {
            var prediccion = _predictionEngine.Predict(new SolicitudTextoInput { Texto = texto });
            var confianza = prediccion.Score.Max();
            return (prediccion.CategoriaPredicha, confianza);
        }
    }
}
