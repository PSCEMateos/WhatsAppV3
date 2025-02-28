using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using WhatsAppPresentacionV3.Modelos;
using System.Net.Http;

namespace WhatsAppPresentacionV3.Servicios
{
    public class WhatsAppMessageService
    {
        private readonly string _idTelefono;
        private readonly string _tokenAcceso;
        private readonly string _facebookGraphVersion = "v21.0";

        /// <summary>
        /// Constructor de la clase WhatsAppMessageService. Inicializa el servicio con el ID de teléfono y el token de acceso.
        /// </summary>
        /// <param name="idTelefono"> ID del número de WhatsApp Business. Actualmente se usa número test que da whatsapp.</param>
        /// <param name="tokenAcceso">Token de acceso para autenticación en la API de Facebook Graph. Se tiene que generar cada cierto tiempo y modificar en elprograma.</param>
        public WhatsAppMessageService(string idTelefono, string tokenAcceso)
        {
            _idTelefono = idTelefono;
            _tokenAcceso = tokenAcceso;
        }
        private string BuildApiUrl() =>
            $"https://graph.facebook.com/{_facebookGraphVersion}/{_idTelefono}/messages";

        /// <summary>
        /// Envía un mensaje de texto, genérico a través de la API de Facebook Graph.
        /// </summary>
        /// <param name="urlFacebookGraph">URL del endpoint de la API de Facebook Graph. Ejemplo: https://graph.facebook.com/v21.0/9999999999/messages</param>
        /// <param name="message">Objeto que contiene el mensaje en formato JSON. Se usan arrays debido a la presencia de listas y botones con opciones modificables </param>
        /// <returns> True si el mensaje se envía con éxito; de lo contrario, False.</returns>
        /// <remarks>Este método sirve como base para los métodos específicos, facilitando el envío de diferentes tipos de mensajes.</remarks>
        public async Task<string> EnviarMensaje(string urlFacebookGraph, object message)
        {
            using HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, urlFacebookGraph);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenAcceso);
            request.Content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();

            return "Response" + responseBody;
        }
        /// <summary>Crea el JSON para enviar un mensaje de texto simple.</summary>
        /// <param name="numeroTelefonoObjetivo">Número de teléfono del destinatario en formato E.164.</param>
        /// <param name="mensaje">Texto del mensaje a enviar.</param>
        /// <returns>True si el mensaje se envía con éxito; de lo contrario, False.</returns>
        /// <remarks>Se usa este método para enviar mensajes de texto sin formato. Ideal para notificaciones o comunicación directa.</remarks>
        public async Task<string> EnviarTexto(string numeroTelefonoObjetivo, string mensaje)
        {
            var finalMessage = new
            {
                messaging_product = "whatsapp",
                to = numeroTelefonoObjetivo,
                type = "text",
                text = new {body = mensaje }
            };
            return await EnviarMensaje(BuildApiUrl(), finalMessage);
        }
        /// <summary>Envía un documento a través de un enlace URL público.</summary>
        /// <param name="numeroTelefonoObjetivo">Número de teléfono del destinatario en formato E.164.</param>
        /// <param name="link">URL público del documento.</param>
        /// <param name="nombreArchivo">Nombre que tendrá el archivo en el mensaje.</param>
        /// <returns>True si el mensaje se envía con éxito; de lo contrario, False.</returns>
        /// <remarks>Este método es útil para compartir documentos que están alojados en un servidor accesible públicamente. 
        /// No se puede usar si se quiere mandar el documento a WhatsApp y usar el url que proporciona</remarks>
        public async Task<string> EnviarDocumentoPorUrl(
            string numeroTelefonoObjetivo, 
            string link, 
            string nombreArchivo)
        {
            var finalMessage = new
            {
                messaging_product = "whatsapp",
                to = numeroTelefonoObjetivo,
                type = "document",
                document = new { link = link, filename = nombreArchivo }
            };
            return await EnviarMensaje(BuildApiUrl(), finalMessage);
        }
        /// <summary>Envía un documento utilizando su ID en la nube de WhatsApp Business.</summary>
        /// <param name="numeroTelefonoObjetivo">Número de teléfono del destinatario en formato E.164.</param>
        /// <param name="idDocumento">ID del documento almacenado en WhatsApp Business.</param>
        /// <param name="nombreArchivo">Nombre que tendrá el archivo en el mensaje.</param>
        /// <returns>True si el mensaje se envía con éxito; de lo contrario, False.</returns>
        /// <remarks> 
        /// Este método requiere que ya se haya subido previamente el archivo a la nube de WhatsApp Business. 
        /// O que se mande un documento en binario a WhatsApp Business, se reciba el url del documento, se pida el ID del documento y se use en este método.
        /// </remarks>
        public async Task<string> EnviarDocumentoPorId(
            string numeroTelefonoObjetivo, 
            string idDocumento, 
            string nombreArchivo)
        {
            var finalMessage = new
            {
                messaging_product = "whatsapp",
                to = numeroTelefonoObjetivo,
                type = "document",
                document = new { id = idDocumento, filename = nombreArchivo }
            };
            return await EnviarMensaje(BuildApiUrl(), finalMessage);
        }
        /// <summary>Envía un botón con un enlace URL. Al precionar el botón, se habre el URL.</summary>
        /// <param name="numeroTelefonoObjetivo">Número de teléfono del destinatario en formato E.164.</param>
        /// <param name="encabezado">Texto del encabezado del mensaje.</param>
        /// <param name="cuerpo">Texto principal del mensaje.</param>
        /// <param name="pie">Texto del pie(Letras pequeñas) del mensaje.</param>
        /// <param name="textoBoton">Texto que aparecerá en el botón.</param>
        /// <param name="link">URL al que el botón redirige.</param>
        /// <returns>True si el mensaje se envía con éxito; de lo contrario, False.</returns>
        /// <remarks>Ideal para CTA (llamadas a la acción) que requieren redirigir a los usuarios a un enlace externo.</remarks>
        public async Task<string> EnviarBotonConUrl(
            string numeroTelefonoObjetivo, 
            string encabezado, 
            string cuerpo, 
            string pie, 
            string textoBoton, 
            string link)
        {
            var finalMessage = new
            {
                messaging_product = "whatsapp",
                to = numeroTelefonoObjetivo,
                type = "interactive",
                interactive = new
                {
                    type = "button",
                    header = new { type = "text", text = encabezado },
                    body = new { text = cuerpo },
                    footer = new { text = pie },
                    action = new
                    {
                        buttons = new[]
                        {
                            new
                            {
                                type = "url",
                                url = link,
                                text = textoBoton
            }}}}};
            return await EnviarMensaje(BuildApiUrl(), finalMessage);
        }
        /// <summary>Envía una imagen utilizando un enlace URL.</summary>
        /// <param name="numeroTelefonoObjetivo">Número de teléfono del destinatario en formato E.164.</param>
        /// <param name="link">URL de acceso público de la imagen.</param>
        /// <returns>True si el mensaje se envía con éxito; de lo contrario, False.</returns>
        /// <remarks>Útil para compartir imágenes alojadas en servidores externos.</remarks>
        public async Task<string> EnviarImagenPorUrl(string numeroTelefonoObjetivo, string link)
        {
            var finalMessage = new
            {
                messaging_product = "whatsapp",
                to = numeroTelefonoObjetivo,
                type = "image",
                image = new { link = link }
            };
            return await EnviarMensaje(BuildApiUrl(), finalMessage);
        }
        /// <summary> Envía un mensaje con 2 o 3 botones interactivos.</summary>
        /// <param name="numeroTelefonoObjetivo">Requisito. Número de teléfono del destinatario en formato E.164.</param>
        /// <param name="mensajeEncabezado">Texto del encabezado del mensaje. Es un objeto JSON. Puede ser un documento, imagen, texto o video. EN el modo actual sólo puede ser texto.</param>
        /// <param name="cuerpoTexto">Requisito. Texto principal del mensaje. Máximo 1024 letras.</param>
        /// <param name="pieTexto">Texto del pie del mensaje. Máximo 60 letras.</param>
        /// <param name="botones">Lista de botones con IDs y etiquetas.
        ///     <param name="ButtonId">Requisito. ID del botón seleccionado. Whats app regresa este ID una vez el usuario seleccione. Máximo 256 letras</param>
        ///     <param name="ButtonLabelText">Requisito. Texto principal del botón a elejir. Máximo 20 letras.</param>
        /// </param>
        /// <returns>True si el mensaje se envía con éxito; de lo contrario, False.</returns>
        /// <remarks>Use este método para dar al usuario opciones de respuesta directa. Puede tener hasta 3 botones.</remarks>
        public async Task<string> EnviarBotonInteractivo(
            string numeroTelefonoObjetivo,
            string mensajeEncabezado,
            string cuerpoTexto,
            string pieTexto,
            List<(string ButtonId, string ButtonLabelText)> botones)
        {
            if (botones.Count > 3)
            {
                await EnviarMensajeError(numeroTelefonoObjetivo, "Solo aguanta hasta 3 botones.");
                throw new ArgumentException("Solo aguanta hasta 3 botones.");
            }


            // Construye el array dinamico de botones
            var buttonsArray = botones.Select(b => new
            {
                type = "reply",
                reply = new
                {
                    id = b.ButtonId,
                    title = b.ButtonLabelText
                }
            }).ToArray();

            // Contruye el mensaje final
            var finalMessage = new
            {
                messaging_product = "whatsapp",
                to = numeroTelefonoObjetivo,
                type = "interactive",
                interactive = new
                {
                    type = "button",
                    header = new
                    {
                        type = "text",
                        text = mensajeEncabezado
                    },
                    body = new
                    {
                        text = cuerpoTexto
                    },
                    footer = new
                    {
                        text = pieTexto
                    },
                    action = new
                    {
                        buttons = buttonsArray
                    }
                }
            };
            return await EnviarMensaje(BuildApiUrl(), finalMessage);
        }
        /// <summary>Envía una lista de opciones interactivas.</summary>
        /// <param name="numeroTelefonoObjetivo">Requisito. Número de teléfono del destinatario en formato E.164.</param>
        /// <param name="encabezado">Texto del encabezado del mensaje.</param>
        /// <param name="cuerpo">Requisito. Texto principal del mensaje.Máximo 1024 letras.</param>
        /// <param name="pie">Texto del pie del mensaje.</param>
        /// <param name="mainButton">Requisito. Texto del botón que inicia la lista.Máximo 20 letras</param>
        /// <param name="secciones">Lista de secciones con títulos y opciones. 
        ///     <param name="SectionTitle">Requisito. Texto del título de opciones. Máxico 24 letras</subparam>
        ///     <param name="OptionId">Requisito. ID del botón seleccionado. Whats app regresa este ID una vez el usuario seleccione. Máxico 256 letras</subparam>
        ///     <param name="OptionTitle">Requisito. Texto principal del botón a elejir. Máxico 24 letras</subparam>
        ///     <param name="OptionDescription"> Texto en el pie del botón a elejir. Máxico 72 letras</subparam>
        ///</param>
        /// <returns>True si el mensaje se envía con éxito; de lo contrario, False.</returns>
        /// <remarks>Perfecto para escenarios en los que el usuario necesita elegir entre múltiples opciones organizadas por categorías. 
        /// Puede tener hasta 10 secciones.Cada sección puede tener hasta 10 botones (Rows).</remarks>
        public async Task<string> EnviarListaDeOpciones(
            string numeroTelefonoObjetivo,
            string encabezado, 
            string cuerpo, 
            string pie, 
            string mainButton,
            List<(string SectionTitle, List<(string OptionId, string OptionTitle, string OptionDescription)> Options)> secciones)
        {
            if (secciones.Count > 10)
            {
                throw new ArgumentException("Solo aguanta hasta 10 secciones.");
            }
            var finalMessageSeccionsArray = secciones.Select(section => new
            {
                title = section.SectionTitle,
                rows = section.Options.Select(option => new
                {
                    id = option.OptionId,
                    title = option.OptionTitle,
                    description = option.OptionDescription
                }).ToArray()
            }).ToArray();

            var finalMessage = new
            {
                messaging_product = "whatsapp",
                to = numeroTelefonoObjetivo,
                type = "interactive",
                interactive = new
                {
                    type = "list",
                    header = new { type = "text", text = encabezado },
                    body = new { text = cuerpo },
                    footer = new { text = pie },
                    action = new { sections = finalMessageSeccionsArray, button = mainButton }
                }
            };
            return await EnviarMensaje($"https://graph.facebook.com/v21.0/{_idTelefono}/messages", finalMessage);
        }
        public async Task<string> EnviarMensajeTemplate(string numeroTelefonoObjetivo, string nombrePlantilla, string[] parametrosPlantilla)
{
    try
    {
        if (string.IsNullOrWhiteSpace(numeroTelefonoObjetivo))
            throw new ArgumentException("El número de teléfono no puede estar vacío.");
        else if (string.IsNullOrWhiteSpace(nombrePlantilla))
            throw new ArgumentException("El nombre de la plantilla no puede estar vacío.");
        else if (parametrosPlantilla == null || parametrosPlantilla.Length == 0)
            throw new ArgumentException("Los parámetros de la plantilla no pueden estar vacíos.");

    }
    catch (Exception ex)
    {
        return $"Error: {ex}";
    }
    var finalMessage = new
    {
        messaging_product = "whatsapp",
        to = numeroTelefonoObjetivo,
        type = "template",
        template = new
        {
            name = nombrePlantilla,
            language = new { code = "es_MX" },
            components = new[]
        {
            new {
                type = "body",
                parameters = parametrosPlantilla.Select(param => new { type = "text", text = param }).ToArray()
            }
        }
        }
    };

    return await EnviarMensaje(BuildApiUrl(), finalMessage);
}
public async Task<string> EnviarFlow(string numeroTelefonoObjetivo,string flowId, string flowToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(numeroTelefonoObjetivo))
                    throw new ArgumentException("El número de teléfono no puede estar vacío.");
                else if (string.IsNullOrWhiteSpace(flowId))
                    throw new ArgumentException("El ID del flujo no puede estar vacío.");

            }
            catch (Exception ex)
            {
                return $"Error: {ex}";
            }
            var messagePayload = new
            {
                messaging_product = "whatsapp",
                to = numeroTelefonoObjetivo,
                type = "interactive",
                interactive = new
                {
                    type = "flow",
                    body = new
                    {
                        text = "¡Prueba este Flow!"        
                        },
        action = new
        {
            name = "flow",
            parameters = new
            {
                flow_message_version = "6.3",
                mode = "draft", // Modo borrador
                flow_token = flowToken,
                flow_id = flowId,
                flow_cta = "Open", // Texto del botón
                flow_action = "data_exchange" // Acción del Flow
            }
        }
    }
};
        return await EnviarMensaje(BuildApiUrl(), messagePayload);
}
                                    
        public async Task EnviarMensajeError(string numeroTelefonoObjetivo, string mensajeError)
        {
            await EnviarTexto(numeroTelefonoObjetivo, mensajeError);
        }
    }
}

/* //Ejemplo de uso:
 *    //Declaraciones generales
 *    var idTelefono = "YOUR_PHONE_ID";
 *    var tokenAcceso = "YOUR_ACCESS_TOKEN";
 *    var service = new WhatsAppMessageService(idTelefono, tokenAcceso);
 *    
 *    //El metodo del mensaje a mandar va aquí
 *    
 *    if (success)
 *    {
 *    Console.WriteLine("El mensaje se mandó correctamente.");
 *    }
 *    else
 *    {
 *    Console.WriteLine("El mensaje no se pudo mandar.");
 *    }
 *    
 *    
 * //Ejemplo Método botones
 *      var botones = new List<(string ButtonId, string ButtonLabelText)> // Se crea una lista de 2 o 3 botones
 *      {
 *          ("option1_id", "Option 1"),
 *          ("option2_id", "Option 2"),
 *          ("option3_id", "Option 3") // Optional third button
 *      };
 *    
 *      // Se manda todo de los botones
 *      bool success = await service.EnviarBotonInteractivo(
 *      "525526903132", // Phone number in E.164 format
 *      "Select an Option", // Header text
 *      "What would you like to do?", // Body text
 *      "Choose wisely", // Footer text
 *      botones
 *      );
 *      
 *    //Ejemplo Método Enviar un documento por URL
 *    await service.EnviarDocumentoPorUrl("525526903132", "https://example.com/document.pdf", "Documento.pdf");
 *    
 *    //Ejemplo MétodoEnviar una imagen por URL
 *    await service.EnviarImagenPorUrl("525526903132", "https://example.com/image.jpg");
 *    
 *    //Ejemplo Método Enviar un botón con URL
 *    await service.EnviarBotonConUrl("525526903132", "Encabezado", "Cuerpo del mensaje", "Pie del mensaje", "Abrir sitio", "https://example.com");
 *    
 *    
 *    //Ejemplo Método Enviar una lista de opciones
 *    var secciones = new List<(string, List<(string, string, string)>)>
 *    {
 *    ("Sección 1", new List<(string, string, string)>
 *    {
 *    ("opcion1", "Opción 1", "Descripción 1"),
 *    ("opcion2", "Opción 2", "Descripción 2")
 *    }),
 *    ("Sección 2", new List<(string, string)>
 *    {
 *    ("opcion3", "Opción 3", "Descripción 3"),
 *    ("opcion4", "Opción 4", "Descripción 4")
 *    })
 *    };
 *    await service.EnviarListaDeOpciones("525526903132", "Encabezado", "Cuerpo del mensaje", "Pie del mensaje", secciones);    
 */
//Documentacion límite de caracteres/letras de cada tipo de mensaje: https://developers.facebook.com/docs/whatsapp/cloud-api/guides/send-messages

//Como declarar una lista
//request.Content = new StringContent("{\"messaging_product\": \"whatsapp\", \"to\": \"" + telefono + "\",\"type\": \"interactive\",\"interactive\": {\"type\": \"button\", \"header\": {\"type\": \"text\", \"text\": \""+MESSAGE_HEADER+"\"}, \"body\": {\"text\": \""+BODY_TEXT+"\"}, \"footer\": {\"text\": \""+Footer_Text+"\"}, \"action\": {\"buttons\": [{\"type\": \"reply\", \"reply\": {\"id\": \""+BUTTON1_ID+"\", \"title\": \""+BUTTON1_LABEL_TEXT+"\"} }, {\"type\": \"reply\", \"reply\": {\"id\": \""+BUTTON2_ID+"\", \"title\": \""+BUTTON2_LABEL_TEXT+"\"}}] }}}"); 
