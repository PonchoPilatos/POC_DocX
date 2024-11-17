using Newtonsoft.Json.Linq;
using Xceed.Document.NET;
using Xceed.Words.NET;

class Program
{
    static void Main()
    {
        // Especifica la ruta del archivo JSON en tu sistema
        string filePath = getRuta();

        // Lee el contenido del archivo JSON
        string readJson = File.ReadAllText(filePath);

        // Parsear el JSON
        JObject jsonObject = JObject.Parse(readJson);

        // Acceder al array de Respuestas
        JArray respuestas = (JArray)jsonObject["Respuestas"];
        getRespuestas(respuestas);
    }

    public static string getRuta()
    {
        string rutaJson = @"C:\Users\PERSONAL\OneDrive\Escritorio\Carpeta INNI\7 Semestre\Servicio Social\temp\Recursos Cuestionarios\Actividad_docX\estructura_organizacional_-_Informaci%C3%B3n_general_-_Respuestas.json"; // Cambia esta ruta por la correcta
        // Verifica si el archivo existe
        if (File.Exists(rutaJson))
        {
            return rutaJson;
        }
        else
        {
            Console.WriteLine("El archivo no se encontró en la ruta especificada.");
            Environment.Exit(0);
            return "xd";
        }
    }
    public static void getRespuestas(JArray respuestas)
    {

        foreach (JObject respuesta in respuestas)
        {
            // Verificar si 'celda' existe y obtener sus valores
            if (respuesta["celda"] != null && respuesta["celda"] is JObject celda)
            {
                // Extraer información de 'celda'
                string titulo = celda["titulo"]?.ToString();
                string parrafo = celda["parrafo"]?.ToString();
                var config = celda["config"];

                // Verificar si 'Input_Type' es 'file'
                if (respuesta["Input_Type"]?.ToString() == "file")
                {
                    // Mostrar la ruta del archivo
                    foreach (var property in respuesta.Properties())
                    {
                        string valor = property.Value.ToString();
                        if (!string.IsNullOrEmpty(valor) && (valor.EndsWith(".jpg") || valor.EndsWith(".png"))) // Verifica que no esté vacío y tenga la extensión correcta
                        {
                            //string RutaArchivo = filePath;
                            string position = config["position"]?.ToString();
                            Console.WriteLine($"Valor: {valor}");
                            Console.WriteLine($"Título: {titulo}");
                            Console.WriteLine($"Párrafo: {parrafo}");
                            Console.WriteLine($"Posicion: {position}");
                            Console.WriteLine("-----------------------");

                            agregarImagen(valor, titulo, parrafo, position);
                        }
                    }
                }
                else
                {
                    string tipoLetra = config["tipoLetra"]?.ToString();
                    int tamanio = (int)(config["tamaño"] ?? 0);
                    bool negrita = (bool)(config["negrita"] ?? false);
                    bool cursiva = (bool)(config["cursiva"] ?? false);

                    // Extraer el valor de respuesta
                    foreach (var property in respuesta.Properties())
                    {
                        // Ignorar claves específicas y aceptar solo valores que no sean null
                        if (property.Name != "celda" && property.Name != "Input_Type" && property.Name != "Input_Options" && property.Value.Type != JTokenType.Null)
                        {
                            string valor = property.Value.ToString();

                            // Mostrar resultados solo si el valor no es null
                            Console.WriteLine($"Valor: {valor}");
                            Console.WriteLine($"Título: {titulo}");
                            Console.WriteLine($"Párrafo: {parrafo}");
                            Console.WriteLine($"Tipo Letra: {tipoLetra}");
                            Console.WriteLine($"Tamaño: {tamanio}");
                            Console.WriteLine($"Negrita: {negrita}");
                            Console.WriteLine($"Cursiva: {cursiva}");
                            Console.WriteLine("-----------------------");

                        }
                    }
                }
            }
        }
    }

    public static void agregarImagen(string rutaImagen, string titulo, string parrafo, string posicion)
    {
        string rutaDocx = @"C:\Users\PERSONAL\OneDrive\Escritorio\Carpeta INNI\7 Semestre\Servicio Social\DocX\Proyecto_prueba\Proyecto_prueba\Archivos_doc\modificar.docx";

        if (!File.Exists(rutaDocx))
        {
            Console.WriteLine("El archivo Word no se encontró en la ruta especificada.");
            return;
        }

        try
        {
            using (var documento = DocX.Load(rutaDocx))
            {
                var tituloEncontrado = documento.Paragraphs.FirstOrDefault(p => p.Text.Contains(titulo));
                if (tituloEncontrado != null)
                {
                    var parrafoEncontrado = documento.Paragraphs.FirstOrDefault(p => p.Text.Contains(parrafo));
                    if (parrafoEncontrado != null)
                    {
                        if (!File.Exists(rutaImagen))
                        {
                            Console.WriteLine("La imagen no se encontró en la ruta especificada.");
                            return;
                        }

                        // Agrega un párrafo nuevo para la imagen
                        var imagenParrafo = documento.InsertParagraph();
                        var imagen = documento.AddImage(rutaImagen);
                        var picture = imagen.CreatePicture(384, 268);

                        // Establece la alineación para la imagen sin afectar el párrafo de texto
                        switch (posicion.ToLower())
                        {
                            case "left":
                                imagenParrafo.AppendPicture(picture).Alignment = Alignment.left;
                                break;
                            case "center":
                                imagenParrafo.AppendPicture(picture).Alignment = Alignment.center;
                                break;
                            case "right":
                                imagenParrafo.AppendPicture(picture).Alignment = Alignment.right;
                                break;
                            default:
                                Console.WriteLine("Posición no válida. Usa 'left', 'center' o 'right'.");
                                return;
                        }

                        // Inserta el párrafo de la imagen justo después del párrafo encontrado
                        parrafoEncontrado.InsertParagraphAfterSelf(imagenParrafo);

                        // Guarda el documento
                        documento.Save();
                        Console.WriteLine("Imagen agregada exitosamente.");
                    }
                    else
                    {
                        Console.WriteLine("No se encontró el párrafo correspondiente.");
                    }
                }
                else
                {
                    Console.WriteLine("No se encontró el título correspondiente.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error: {ex.Message}");
        }
    }
}

