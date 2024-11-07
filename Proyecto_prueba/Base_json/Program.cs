using Newtonsoft.Json.Linq;

class Program
{
    static void Main()
    {
        // Especifica la ruta del archivo JSON en tu sistema
        string filePath = getRuta();

        // Lee el contenido del archivo JSON
        string json = File.ReadAllText(filePath);

        // Parsear el JSON
        JObject jsonObject = JObject.Parse(json);

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

                // Extraer configuraciones
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

