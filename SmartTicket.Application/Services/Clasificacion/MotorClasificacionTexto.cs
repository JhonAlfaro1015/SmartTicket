using System.Globalization;
using System.Text;


namespace SmartTicket.Application.Services.Clasificacion;

public class MotorClasificacionTexto : IMotorClasificacionTexto
{
    // Categoría -> lista de (palabra/frase clave normalizada, peso).
    // Los nombres de categoría deben coincidir EXACTO con dbo.Categorias.Nombre
    private static readonly Dictionary<string, (string Palabra, int Peso)[]> ReglasPorCategoria = new()
    {
        ["Soporte técnico"] =
        [
        ("internet", 20), ("router", 12), ("wifi", 12), ("senal", 10),
        ("no funciona", 18), ("sin conexion", 18), ("lento", 8), ("modem", 10)
    ],
        ["Facturación"] =
        [
        ("cobro indebido", 20), ("doble cobro", 20), ("factura incorrecta", 18),
        ("revisar mi factura", 15), ("cargo desconocido", 18)
    ],
        ["Ventas"] =
        [
        ("comprar", 15), ("adquirir", 12), ("plan nuevo", 15), ("promocion", 10), ("cotizacion", 12)
    ],
        ["Instalaciones"] =
        [
        ("instalacion", 18), ("instalar", 15), ("visita tecnica", 15), ("agendar instalacion", 18)
    ],
        ["Cancelaciones"] =
        [
        ("cancelar", 20), ("dar de baja", 20), ("cancelacion del servicio", 20)
    ],
        ["Reclamos"] =
        [
        ("reclamo", 18), ("queja", 15), ("inconformidad", 15), ("no estoy conforme", 15)
    ],
        ["Solicitudes de información"] =
        [
        ("quiero saber", 3), ("informacion sobre", 5), ("como puedo", 4),
        ("medios de pago", 4), ("cuales son", 4), ("consulta", 3),
        ("pagar", 4), ("pse", 4), ("factura", 4)
    ]
    };

    // Frases que suman urgencia sin importar la categoría detectada
    private static readonly (string Palabra, int Peso)[] ReglasUrgencia =
    [
    ("urgente", 30), ("no tengo", 25), ("desde ayer", 15), ("hace dias", 15)
];

    public ResultadoClasificacion Clasificar(string texto)
    {
        var textoNormalizado = Normalizar(texto);

        var categoriaGanadora = "Solicitudes de información"; // categoría por defecto si nada hace match
        var mejorPuntajeCategoria = 0;

        foreach (var (categoria, reglas) in ReglasPorCategoria)
        {
            var puntajeCategoria = reglas
                .Where(r => textoNormalizado.Contains(r.Palabra))
                .Sum(r => r.Peso);

            if (puntajeCategoria > mejorPuntajeCategoria)
            {
                mejorPuntajeCategoria = puntajeCategoria;
                categoriaGanadora = categoria;
            }
        }

        var puntajeUrgencia = ReglasUrgencia
            .Where(r => textoNormalizado.Contains(r.Palabra))
            .Sum(r => r.Peso);

        var puntuacionTotal = Math.Min(mejorPuntajeCategoria + puntajeUrgencia, 100);

        var prioridad = puntuacionTotal switch
        {
            >= 85 => "CRITICA",
            >= 60 => "ALTA",
            >= 30 => "MEDIA",
            _ => "BAJA"
        };

        var subcategoria = DetectarSubcategoria(categoriaGanadora, textoNormalizado);

        return new ResultadoClasificacion(
            CategoriaNombre: categoriaGanadora,
            SubcategoriaNombre: subcategoria,
            PrioridadNombre: prioridad,
            AreaNombre: categoriaGanadora, // Área usa el mismo nombre que Categoría (así quedó el script de BD)
            Puntuacion: puntuacionTotal);
    }

    private static string? DetectarSubcategoria(string categoria, string textoNormalizado)
    {
        return categoria switch
        {
            "Soporte técnico" when textoNormalizado.Contains("internet")
                                 || textoNormalizado.Contains("no tengo")
                                 || textoNormalizado.Contains("router")
                => "Sin servicio",

            "Solicitudes de información" when textoNormalizado.Contains("medios de pago")
                                            || textoNormalizado.Contains("pse")
                                            || textoNormalizado.Contains("pagar")
                => "Medios de pago",

            _ => null
        };
    }

    private static string Normalizar(string texto)
    {
        var minuscula = texto.ToLowerInvariant();
        var formaD = minuscula.Normalize(NormalizationForm.FormD);

        var sinTildes = new StringBuilder();
        foreach (var c in formaD)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sinTildes.Append(c);
        }

        return sinTildes.ToString().Normalize(NormalizationForm.FormC);
    }
}
