using Microsoft.ML;
using SmartTicket.Application.Services.Clasificacion.ClasificacionML;

namespace SmartTicket.Infrastructure.MachineLearning;

internal sealed class ModeloCargable(MLContext mlContext, string ruta)
{
    private readonly MLContext _mlContext = mlContext;
    private readonly string _ruta = ruta;
    private PredictionEngine<SolicitudTextoInput, PrediccionEtiqueta>? _engine;
    private DateTime _fechaArchivoCargado = DateTime.MinValue;

    public (string Etiqueta, float Confianza) Predecir(string texto)
    {
        if (!File.Exists(_ruta))
            throw new InvalidOperationException(
                "Los modelos de ML aún no han sido entrenados. Ejecuta POST /api/ml/entrenar primero.");

        var fechaActual = File.GetLastWriteTimeUtc(_ruta);
        if (_engine is null || fechaActual != _fechaArchivoCargado)
        {
            var modelo = _mlContext.Model.Load(_ruta, out _);
            _engine = _mlContext.Model.CreatePredictionEngine<SolicitudTextoInput, PrediccionEtiqueta>(modelo);
            _fechaArchivoCargado = fechaActual;
        }

        var prediccion = _engine.Predict(new SolicitudTextoInput { Texto = texto });
        return (prediccion.Etiqueta, prediccion.Score.Max());
    }
}

public class MotorClasificacionML : IMotorClasificacionML
{
    private readonly object _bloqueo = new(); // PredictionEngine no es thread-safe
    private readonly ModeloCargable _categoria;
    private readonly ModeloCargable _prioridad;

    public MotorClasificacionML(string directorioModelos)
    {
        var mlContext = new MLContext(seed: 42);
        _categoria = new ModeloCargable(mlContext, RutasModelo.Categoria(directorioModelos));
        _prioridad = new ModeloCargable(mlContext, RutasModelo.Prioridad(directorioModelos));
    }

    public (string CategoriaPredicha, float Confianza) PredecirCategoria(string texto)
    {
        lock (_bloqueo)
        {
            var (etiqueta, confianza) = _categoria.Predecir(texto);
            return (etiqueta, confianza);
        }
    }

    public (string PrioridadPredicha, float Confianza) PredecirPrioridad(string texto)
    {
        lock (_bloqueo)
        {
            var (etiqueta, confianza) = _prioridad.Predecir(texto);
            return (etiqueta, confianza);
        }
    }
}
