using Microsoft.ML;
using Microsoft.ML.Data;
using SmartTicket.Application.Common;
using SmartTicket.Application.Services.Clasificacion.ClasificacionML;

namespace SmartTicket.Infrastructure.MachineLearning;

internal static class RutasModelo
{
    public static string Categoria(string directorio) => Path.Combine(directorio, "modelo_categoria.zip");
    public static string Prioridad(string directorio) => Path.Combine(directorio, "modelo_prioridad.zip");
}

internal class FilaEvaluada
{
    public string Texto { get; set; } = null!;
    public string Categoria { get; set; } = null!;
    public string Prioridad { get; set; } = null!;

    [ColumnName("PredictedLabel")]
    public string Predicha { get; set; } = null!;
}

public class EntrenadorModeloML(string directorioModelos) : IEntrenadorModeloML
{
    private readonly string _directorioModelos = directorioModelos;

    public ResultadoEntrenamiento EntrenarDesdeCsv(string rutaEntrenamiento, IReadOnlyList<string> rutasValidacion)
    {
        var mlContext = new MLContext(seed: 42);

        var entrenamiento = Cargar(mlContext, rutaEntrenamiento);
        var validaciones = rutasValidacion
            .Where(File.Exists)
            .Select(ruta => (Nombre: Path.GetFileName(ruta), Datos: Cargar(mlContext, ruta)))
            .ToList();

        var categoria = EntrenarYEvaluar(
            mlContext, entrenamiento, validaciones,
            nameof(SolicitudTextoInput.Categoria), RutasModelo.Categoria(_directorioModelos), ordenOrdinal: null);

        var prioridad = EntrenarYEvaluar(
            mlContext, entrenamiento, validaciones,
            nameof(SolicitudTextoInput.Prioridad), RutasModelo.Prioridad(_directorioModelos),
            ordenOrdinal: ConstantesML.OrdenPrioridad);

        return new ResultadoEntrenamiento(
            mlContext.Data.CreateEnumerable<SolicitudTextoInput>(entrenamiento, reuseRowObject: false).Count(),
            categoria,
            prioridad);
    }

    private static IDataView Cargar(MLContext mlContext, string ruta) =>
        mlContext.Data.LoadFromTextFile<SolicitudTextoInput>(
            path: ruta,
            hasHeader: true,
            separatorChar: ',',
            allowQuoting: true);   // hay textos con comas dentro

    private static List<MetricasValidacion> EntrenarYEvaluar(
        MLContext mlContext,
        IDataView entrenamiento,
        List<(string Nombre, IDataView Datos)> validaciones,
        string columnaEtiqueta,
        string rutaModelo,
        string[]? ordenOrdinal)
    {
        var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", columnaEtiqueta)
            .Append(mlContext.Transforms.Text.FeaturizeText("Features", nameof(SolicitudTextoInput.Texto)))
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        var modelo = pipeline.Fit(entrenamiento);
        mlContext.Model.Save(modelo, entrenamiento.Schema, rutaModelo);

        var resultados = new List<MetricasValidacion>();

        foreach (var (nombre, datos) in validaciones)
        {
            var predicciones = modelo.Transform(datos);
            var metricas = mlContext.MulticlassClassification.Evaluate(predicciones);

            var filas = mlContext.Data
                .CreateEnumerable<FilaEvaluada>(predicciones, reuseRowObject: false)
                .ToList();

            var errores = new List<ErrorClasificacion>();
            var dentroDeUnNivel = 0;

            foreach (var fila in filas)
            {
                var esperada = columnaEtiqueta == nameof(SolicitudTextoInput.Categoria)
                    ? fila.Categoria
                    : fila.Prioridad;

                if (esperada != fila.Predicha)
                    errores.Add(new ErrorClasificacion(fila.Texto, esperada, fila.Predicha));

                if (Distancia(ordenOrdinal, esperada, fila.Predicha) <= 1)
                    dentroDeUnNivel++;
            }

            resultados.Add(new MetricasValidacion(
                nombre,
                filas.Count,
                metricas.MicroAccuracy,
                metricas.MacroAccuracy,
                metricas.LogLoss,
                filas.Count == 0 ? 0 : (double)dentroDeUnNivel / filas.Count,
                errores));
        }

        return resultados;
    }

    // Distancia entre dos etiquetas ordinales. Sin orden (categoría): 0 si coinciden, "infinita" si no.
    private static int Distancia(string[]? orden, string a, string b)
    {
        if (a == b) return 0;
        if (orden is null) return int.MaxValue;

        var ia = Array.IndexOf(orden, a);
        var ib = Array.IndexOf(orden, b);
        return ia < 0 || ib < 0 ? int.MaxValue : Math.Abs(ia - ib);
    }
}