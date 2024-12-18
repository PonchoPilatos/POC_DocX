using Newtonsoft.Json.Linq;
using Proyecto_prueba;
using System.Drawing;
using System.Globalization;
using Xceed.Document.NET;
using Xceed.Words.NET;

class Program
{
    static void Main()
    {

        // Especifica la ruta del archivo JSON en tu sistema
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        string filePath = getRuta();

        // Lee el contenido del archivo JSON
        string readJson = File.ReadAllText(filePath);

        // Parsear el JSON
        JObject jsonObject = JObject.Parse(readJson);

        // Acceder al array de Respuestas
        JArray answers = (JArray)jsonObject["Respuestas"];
        getAnswers(answers);
    }

    public static string getRuta()
    {
        string rutaJson = @"C:\Users\PERSONAL\OneDrive\Escritorio\Carpeta INNI\7 Semestre\Servicio Social\temp\Recursos Cuestionarios\Actividad_docX\cuestionario_1.json"; // Cambia esta ruta por la correcta
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
    public static void getAnswers(JArray answers_file)
    {
        
        string tittle = "", paragraph = "", textRow = "", textCell = "", total_position="";
        List<DocAnswerDto> list_AnswerImage = new List<DocAnswerDto>();
        List<DocAnswerDto> list_AnswerText = new List<DocAnswerDto>();

        foreach (JObject answer_file in answers_file)
        {
            // Verificar si 'celda' existe y obtener sus valores
            if (answer_file["celda"] != null && answer_file["celda"] is JObject cell)
            {

                int type_answer = 0;

                if (cell.ContainsKey("titulo"))
                {
                    tittle = cell["titulo"]?.ToString();
                    paragraph = cell["parrafo"]?.ToString();
                    type_answer = 1;
                    
                }
                else
                {
                    textRow = cell["Text_Row"]?.ToString();
                    textCell = cell["Text_Cell"]?.ToString();
               
                    if (cell["total_position"]?.ToString() != null) total_position = cell["total_position"]?.ToString();

                    type_answer = 2;
                    
                }
                // Extraer información de 'celda'

                var config = cell["config"];

                // Verificar si 'Input_Type' es 'file'
                if (answer_file["Input_Type"]?.ToString() == "file")
                {
                    // Mostrar la ruta del archivo
                    foreach (var property in answer_file.Properties())
                    {
                        string value = property.Value.ToString();
                        if (!string.IsNullOrEmpty(value) && (value.EndsWith(".jpg") || value.EndsWith(".png"))) // Verifica que no esté vacío y tenga la extensión correcta
                        {
                            //string RutaArchivo = filePath;
                            string position = config["position"]?.ToString();

                            list_AnswerImage.Add(new DocAnswerDto(value, tittle, paragraph, position));
                        }
                    }
                }
                else
                {
                    string tipoLetra = config["tipoLetra"]?.ToString();
                    int tamanio = (int)(config["tamaño"] ?? 0);
                    bool negrita = (bool)(config["negrita"] ?? false);
                    bool cursiva = (bool)(config["cursiva"] ?? false);

                    if (type_answer == 1)
                    {

                        // Extraer el valor de respuesta
                        foreach (var property in answer_file.Properties())
                        {
                            // Ignorar claves específicas y aceptar solo valores que no sean null
                            if (property.Name != "celda" && property.Name != "Input_Type" && property.Name != "Input_Options" && property.Value.Type != JTokenType.Null)
                            {
                                string valor = property.Value.ToString();

                                // Mostrar resultados solo si el valor no es null
                                Console.WriteLine($"Valor: {valor}");
                                Console.WriteLine($"Título: {tittle}");
                                Console.WriteLine($"Párrafo: {paragraph}");
                                Console.WriteLine($"Tipo Letra: {tipoLetra}");
                                Console.WriteLine($"Tamaño: {tamanio}");
                                Console.WriteLine($"Negrita: {negrita}");
                                Console.WriteLine($"Cursiva: {cursiva}");
                                Console.WriteLine("-----------------------");


                            }
                        }
                    }
                    else if (type_answer == 2)
                    {

                        // Extraer el valor de respuesta
                        foreach (var property in answer_file.Properties())
                        {
                            // Ignorar claves específicas y aceptar solo valores que no sean null
                            if (property.Name != "celda" && property.Name != "Input_Type" && property.Name != "Input_Options" && property.Value.Type != JTokenType.Null)
                            {
                                string valor = property.Value.ToString();
                   
                                list_AnswerText.Add(new DocAnswerDto(valor, textRow, textCell, tipoLetra, tamanio, negrita, cursiva, total_position));

                            }
                        }
                    }

                }
            }
        }

        //if (list_AnswerImage.Count > 0) addImage(list_AnswerImage);
        if (list_AnswerText.Count > 0)
        {
            foreach (var answer in list_AnswerText)
            {
                Console.WriteLine($"Valor: {answer.Value()}");
                Console.WriteLine($"Fila: {answer.Aux_text1()}");
                Console.WriteLine($"Columna: {answer.Aux_text2()}");
                Console.WriteLine($"Posicion en la que se encuentra el total: {answer.Total_position()}");
                Console.WriteLine($"Tipo Letra: {answer.Font()}");
                Console.WriteLine($"Tamaño: {answer.Size()}");
                Console.WriteLine($"Negrita: {answer.Bold_type()}");
                Console.WriteLine($"Cursiva: {answer.Italics()}");
                Console.WriteLine("-----------------------");
            }

            addTextTable(list_AnswerText);
        }

        if (list_AnswerImage.Count > 0)
        {
            foreach (var answer in list_AnswerImage)
            {
                Console.WriteLine($"Valor: {answer.Value()}");
                Console.WriteLine($"Título: {answer.Aux_text1()}");
                Console.WriteLine($"Párrafo: {answer.Aux_text2()}");
                Console.WriteLine($"Posicion: {answer.Position()}");
                Console.WriteLine("-----------------------");
              
            }

            addImage(list_AnswerImage);
        }
    }

    
    public static void addImage(List<DocAnswerDto> list_images)
    {
        //Ruta del documento docx
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
                foreach (var image in list_images)
                {

                    var tituloEncontrado = documento.Paragraphs.FirstOrDefault(p => p.Text.Contains(image.Aux_text1()));
                    if (tituloEncontrado != null)
                    {
                        var parrafoEncontrado = documento.Paragraphs.FirstOrDefault(p => p.Text.Contains(image.Aux_text2()));
                        if (parrafoEncontrado != null)
                        {
                            if (!File.Exists(image.Value()))
                            {
                                Console.WriteLine("La imagen no se encontró en la ruta especificada.");
                                return;
                            }

                            // Agrega un párrafo nuevo para la imagen
                            var image_paragraph = documento.InsertParagraph();
                            var image_aux = documento.AddImage(image.Value());
                            var picture = image_aux.CreatePicture(384, 268);

                            // Establece la alineación para la imagen sin afectar el párrafo de texto
                            switch (image.Position().ToLower())
                            {
                                case "left":
                                    image_paragraph.AppendPicture(picture).Alignment = Alignment.left;
                                    break;
                                case "center":
                                    image_paragraph.AppendPicture(picture).Alignment = Alignment.center;
                                    break;
                                case "right":
                                    image_paragraph.AppendPicture(picture).Alignment = Alignment.right;
                                    break;
                                default:
                                    Console.WriteLine("Posición no válida. Usa 'left', 'center' o 'right'.");
                                    return;
                            }

                            // Inserta el párrafo de la imagen justo después del párrafo encontrado
                            parrafoEncontrado.InsertParagraphAfterSelf(image_paragraph);
                            parrafoEncontrado.InsertParagraphAfterSelf(image_paragraph);


                            // Guarda el documento

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
                documento.Save();

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error: {ex.Message}");
        }
    }

    private static string CleanText(string input)
    {
        return input?.Trim().Replace("\r", "").Replace("\n", "") ?? "";
    }

    public static void addTextTable(List<DocAnswerDto> list_AnswerText) {

        //Ruta del documento docx
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
                foreach (var answerText in list_AnswerText)
                {

                    for (int t = 1; t < documento.Tables.Count; t++)
                    {
                        var table = documento.Tables[t]; // Selecciona la primera tabla
                        int fila = -1, columna = -1; //indices para filas y columnas
                        string text = ""; //cadena auxiliar para comparar correlaciones entre filas y columnas

                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            var row = table.Rows[i];

                            // Iterar sobre cada celda en la fila
                            for (int j = 0; j < row.Cells.Count; j++)
                            {
                                var cell = row.Cells[j];
                                if (cell.Paragraphs.Count > 1)
                                {
                                    text = cell.Paragraphs[1].Text;
                                }
                                else if (cell.Paragraphs.Count == 1)
                                {
                                    text = cell.Paragraphs[0].Text;
                                }

                                if (text == answerText.Aux_text1())
                                {
                                    fila = i;
                                    if (columna != -1) break;
                                }

                                if (text == answerText.Aux_text2())
                                {
                                    columna = j;
                                    if (fila != -1) break;
                                }

                            }

                        }

                        if (fila != -1 && columna != -1)
                        {
                            
                            // Agregar texto a la celda encontrada
                            table.Rows[fila].Cells[columna].Paragraphs[0]
                                .Append(answerText.Value())
                                .Font(answerText.Font())
                                .FontSize(answerText.Size())
                                .Bold(answerText.Bold_type())
                                .Italic(answerText.Italics());
                            

                            if (string.IsNullOrEmpty(answerText.Total_position()))
                            {
                                Console.WriteLine("no entro");



                            }
                            else
                            {
   
                                string[] aux = answerText.Total_position().Split('/');

                                string aux_fila = aux[0];
                                string aux_columna = aux[1];
                                string aux_content = table.Rows[int.Parse(aux_fila)].Cells[int.Parse(aux_columna)].Paragraphs[0].Text;



                                if (string.IsNullOrEmpty(aux_content))
                                {

                                    table.Rows[int.Parse(aux_fila)].Cells[int.Parse(aux_columna)].Paragraphs[0].Append("0");
                                    aux_content = table.Rows[int.Parse(aux_fila)].Cells[int.Parse(aux_columna)].Paragraphs[0].Text;

                                }
                                

                                //Sumatoria de el valor actual de la celda de total mas el valor insertado
                                float sum = float.Parse(answerText.Value()) + float.Parse(aux_content);
                                if (int.Parse(aux_columna)==3) Console.WriteLine($"{answerText.Value()}\n{aux_fila},{aux_columna}: {aux_content} y {sum}");

                                table.Rows[int.Parse(aux_fila)].Cells[int.Parse(aux_columna)].Paragraphs[0].Remove(false);
                                table.Rows[int.Parse(aux_fila)].Cells[int.Parse(aux_columna)].Paragraphs[0].Append(sum.ToString());



                            }
                            //Console.WriteLine($"Texto agregado correctamente en la celda ({fila}, {columna}).");
                            
                        }

                    }

                }

                // Guardar el documento
                documento.Save();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error: {ex.Message}");
        }


    }
}

