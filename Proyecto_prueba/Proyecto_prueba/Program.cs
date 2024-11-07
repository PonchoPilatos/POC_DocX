using Newtonsoft.Json.Linq;
using Xceed.Document.NET;
using Xceed.Words.NET;

class Program
{
    static void Main()
    {
        // Especifica la ruta del archivo JSON y Word
        string filePathJson = getRuta();
        string filePathWord = @"C:\Users\PERSONAL\OneDrive\Escritorio\Carpeta INNI\7 Semestre\Servicio Social\DocX\Proyecto_prueba\Proyecto_prueba\Archivos_doc\modificar.docx"; // Cambia esta ruta a la correcta

        // Lee el contenido del archivo JSON
        string json = File.ReadAllText(filePathJson);
        JObject jsonObject = JObject.Parse(json);

        // Accede al array de Respuestas
        JArray respuestas = (JArray)jsonObject["Respuestas"];

        // Procesa las respuestas y escribe en el Word
        using (DocX document = DocX.Load(filePathWord))
        {
            getRespuestas(respuestas, document);
            document.Save(); // Guarda los cambios
        }
    }

    public static string getRuta()
    {
        string rutaJson = @"C:\Users\PERSONAL\OneDrive\Escritorio\Carpeta INNI\7 Semestre\Servicio Social\temp\Recursos Cuestionarios\Actividad_docX\estructura_organizacional_-_Informaci%C3%B3n_general_-_Respuestas.json";
        if (File.Exists(rutaJson))
        {
            return rutaJson;
        }
        else
        {
            Console.WriteLine("El archivo no se encontró en la ruta especificada.");
            Environment.Exit(0);
            return string.Empty;
        }
    }

    public static void getRespuestas(JArray respuestas, DocX document)
    {
        foreach (JObject respuesta in respuestas)
        {
            if (respuesta["celda"] != null && respuesta["celda"] is JObject celda)
            {
                // Extraer variables del JSON
                string titulo = celda["titulo"]?.ToString();
                string parrafo = celda["parrafo"]?.ToString();
                string valor = respuesta.Properties().FirstOrDefault(p => p.Name != "celda" && p.Name != "Input_Type" && p.Name != "Input_Options" && p.Value.Type != JTokenType.Null)?.Value.ToString();

                string tipoLetra = celda["config"]?["tipoLetra"]?.ToString();
                int tamanio = (int)(celda["config"]?["tamaño"] ?? 12); // Tamaño por defecto 12 si no se especifica
                bool negrita = (bool)(celda["config"]?["negrita"] ?? false);
                bool cursiva = (bool)(celda["config"]?["cursiva"] ?? false);

                // Buscar el título en los párrafos del documento
                Paragraph tituloEncontrado = null;
                foreach (var paragraph in document.Paragraphs)
                {
                    if (paragraph.Text.Contains(titulo))
                    {
                        tituloEncontrado = paragraph;
                        break;
                    }
                }

                if (tituloEncontrado != null)
                {
                    // Dentro del título, buscar el párrafo correspondiente
                    Paragraph parrafoEncontrado = null;
                    foreach (var paragraph in document.Paragraphs)
                    {
                        if (paragraph.Text.Contains(parrafo))
                        {
                            parrafoEncontrado = paragraph;
                            break;
                        }
                    }

                    if (parrafoEncontrado != null)
                    {
                        // Insertar el texto `valor` con las características de formato
                        var textoFormateado = parrafoEncontrado.Append($" {valor}");
                        textoFormateado.Font(tipoLetra).FontSize(tamanio);

                        if (negrita)
                            textoFormateado.Bold();
                        if (cursiva)
                            textoFormateado.Italic();

                        Console.WriteLine($"Se añadió el texto en el párrafo: {parrafo}");
                    }
                    else
                    {
                        Console.WriteLine($"No se encontró el párrafo: {parrafo}");
                    }
                }
                else
                {
                    Console.WriteLine($"No se encontró el título: {titulo}");
                }
            }
        }
    }
}

