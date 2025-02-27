
using WhatsAppPresentacionV3.Modelos;
using WhatsAppPresentacionV3.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Eventing.Reader;

namespace WhatsAppPresentacionV3.Controllers
{
    //[ApiController]
    //[Route("api/chatbot-presentacion")]
    public class PresentacionController : ControllerBase
    {
        private readonly WhatsAppMessageService _messageSendService;
        private readonly HanldeDocument _hanldeDocument;
        private static string _lastMessageState = string.Empty;
        private static string _montoFactura = string.Empty;
        private static string _tipoProgramación = string.Empty;
        private static string _oldUserMessage = string.Empty;
        private static string _usoCFDI = string.Empty;
        private static string _usuarioAUsar = string.Empty;
        private static string _usuarioAUsarID = string.Empty;
        private static readonly List<string> _nombres = new()
        {
            "Jesús Trejo", "Jesús Garza", "Jesús Zepeda", "Alejandro Trejo",
            "Alejandro Garza", "Javier Zepeda", "Carlos Mendoza", "María López",
            "Sofía Fernández", "Juan Hernández", "Luis Martínez", "Ana González",
            "Roberto Cruz", "Lucía Vargas", "Pablo Ramírez", "Gabriela Castillo",
            "Fernando Pérez", "Daniela Sánchez", "Héctor Morales", "Paola Jiménez"
        };
        private static readonly List<string> _idNombres = new()
        {
            "TEDJ800706QA1", "TEDJ800706QA2", "TEDJ800706QA3", "TEDJ800706QA4",
            "TEDJ800706QA5", "TEDJ800706QA6", "TEDJ800706QA7", "TEDJ800706QA8",
            "TEDJ800706QA9", "TEDJ800706QB1", "TEDJ800706QB2", "TEDJ800706QB3",
            "TEDJ800706QB4", "TEDJ800706QB5", "TEDJ800706QB6", "TEDJ800706QB7",
            "TEDJ800706QB8", "TEDJ800706QB9", "TEDJ800706QC1", "TEDJ800706QC2"
        };
        private static List<string> _receivedMessages = new List<string>();
        private static List<(string ButtonId, string ButtonLabelText)> _botones = new();

        public PresentacionController()
        {
            string idTelefono = "513380398517712"; // Replace with actual Phone ID
            string tokenAcceso = "EAAGf6qvOtkYBO6mkn3tdd6kfYtUShTisYAzY8Hni1PndsBRNMTqMGBBFmookHLp3dQ5hqEwEhUDsRRKiqkRXBQ8e8XQAa8n4Q4CQO0xwB72mVLiuJTIMAlCiW969ZB90jD9Q1KDuvLTQbSV0jntlmYSnGTjlhLiePRRYWbLR5UQVggfRWIUUaKd9WahlHjx1SEY5ZANPuRda1XUogcwo4Raw4ZD"; // Replace with actual Access Token
            _messageSendService = new WhatsAppMessageService(idTelefono, tokenAcceso);
            _hanldeDocument = new HanldeDocument(tokenAcceso);
        }

        //RECIBIMOS LOS DATOS DE VALIDACION VIA GET 
        [HttpGet("api/webhook")]

        //RECIBIMOS LOS PARAMETROS QUE NOS ENVIA WHATSAPP PARA VALIDAR NUESTRA URL
        public string WebhookValidation(
            [FromQuery] WebhookValidationRequest request)
        {
            //SI EL TOKEN ES hola (O EL QUE COLOQUEMOS EN FACEBOOK) 
            if (request.TokenDeVerificacion.Equals("Presenta61224"))
            {
                return request.Challenge;
            }
            return "Token Incorrecto";
        }

        [HttpPost("api/webhook")]
        public async Task<IActionResult> Webhook([FromBody] WebHookResponseModel webhookData)
        {
            //revisar que todos los mensajes recibidos sean válidos
            if (webhookData.entry == null || !webhookData.entry.Any())
            {
                _receivedMessages.Add("Estructura de mensaje recibido inválida");
                return BadRequest("Estructura inválida");
            }
            //revisar que no sea el mensaje de prueba (probiene de "string")
            else if ((webhookData.entry[0].changes[0].value.messages[0].from == "string"))
            {
                _receivedMessages.Add("Mensaje de prueba");
                return Ok();
            }

            _receivedMessages.Add("Mensaje válido"); //Marca que está funcionando
            string fromPhoneNumber = NormalizarNumeroMexico(webhookData.entry[0].changes[0].value.messages[0].from);
            try
            {
                string messageStatus = await HandleIncomingMessage(webhookData, fromPhoneNumber);

                _receivedMessages.Add("Mensaje manejado");
                
                _receivedMessages.Add(messageStatus);
                _receivedMessages.Add(_lastMessageState);
                return Ok();
            }
            catch (Exception ex)
            {
                try
                {
                    string incomingJson = System.Text.Json.JsonSerializer.Serialize(webhookData, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true // Makes the JSON easier to read
                    });

                    _receivedMessages.Add("Error:" + ex.Message);
                    _receivedMessages.Add("WhatsApp JSON: " + incomingJson);
                }
                catch (Exception serializationEx)
                {
                    _receivedMessages.Add($"Error serializing incoming JSON: {serializationEx.Message}");
                }
                await _messageSendService.EnviarMensajeError(fromPhoneNumber, ex.Message);
                return BadRequest(ex.Message);
            }

        }

        /*[HttpGet("api/webhook/flow")]
        public string FlowWebhook([FromQuery] WebhookValidationRequest request)
        {
            if (request.TokenDeVerificacion.Equals("Presenta61224"))
            {
                return request.Challenge;
            }
            return "Token Incorrecto";
        }*/

        //Recabamos los Mensajes VIA GET 
        [HttpGet]
        //DENTRO DE LA RUTA webhook 
        [Route("messages")]
        public dynamic GetMessages()
        {
            // Return the list of received messages as the response 
            return _receivedMessages;
        }

        private async Task<string> HandleIncomingMessage(WebHookResponseModel webhookData, string fromPhoneNumber)
        {
            _receivedMessages.Add("Handeling incoming Message");
            var incomingMessage = webhookData.entry[0].changes[0].value.messages[0];
                    
            if (incomingMessage.interactive != null)
            {
                _receivedMessages.Add("Message is interactive");
                return await HandleInteractiveMessage(webhookData, fromPhoneNumber);
            }
            else if (incomingMessage.text != null)
            {
                _receivedMessages.Add("Message is text");
                return await HandleTextMessage(webhookData, fromPhoneNumber);
            }
            else if (incomingMessage.document != null)
            {
                _receivedMessages.Add("Message is document");
                return await HandleDocumentMessage(webhookData);
            }
            _receivedMessages.Add("Message not supported");
            return "Message type not supported";
        }
        private async Task<string> HandleInteractiveMessage(
            WebHookResponseModel webhookData, 
            string fromPhoneNumber)
        {
            var incomingMessage = webhookData.entry[0].changes[0].value.messages[0];
            if (incomingMessage.interactive.button_reply != null)
            {
                string selectedButtonId = incomingMessage.interactive.button_reply.id;
                _receivedMessages.Add($"Message is button reply with ID: {selectedButtonId}");
                return await HandleInteractiveButtonMessage(webhookData, selectedButtonId, fromPhoneNumber);
            }
            else if (incomingMessage.interactive.list_reply != null)
            {
                string selectedListButtonId = incomingMessage.interactive.list_reply.id;
                _receivedMessages.Add($"Message is List reply with ID: {selectedListButtonId}");
                return await HandleInteractiveListMessage(webhookData, selectedListButtonId, fromPhoneNumber);
            }
            /*else if (incomingMessage.interactive.flow_reply != null)
            {
                string flowName = incomingMessage.interactive.flow_reply.name;
                string flowBody = incomingMessage.interactive.flow_reply.body;
                _receivedMessages.Add($"Flow received: {flowName} - {flowBody}");
                return await HandleInteractiveFlowMessage(webhookData, fromPhoneNumber);
            }*/
                return "";
        }
        private async Task<string> HandleInteractiveButtonMessage(
            WebHookResponseModel webhookData, 
            string selectedButtonId, 
            string fromPhoneNumber)
        {
            string messageStatus = "";
            _receivedMessages.Add($"Selecting path");
            switch (selectedButtonId)
            {
                case "opcion_Generar_Factura":
                    messageStatus = await _messageSendService.EnviarTexto(fromPhoneNumber, "Generando factura. ¿a quien se la vas a generar?");
                    break;

                case "opcion_Enviar_Documento_Firmar":
                    messageStatus = await _messageSendService.EnviarTexto(fromPhoneNumber, "Favor de enviar el documento");
                    break;

                case "opcion_Programación_Aplicaciones"://Paso 11: El usuario elige Programación de aplicaciones
                                                        //Paso 12: Bot: "Dame la cantidad a cobrar antes de impuestos."
                    _tipoProgramación = "Programación de Aplicaciones";
                    messageStatus = await _messageSendService.EnviarTexto(fromPhoneNumber, "Dame la cantidad a cobrar antes de impuestos");
                    break;

                case "opcion_Programación_videojuegos"://Paso 11: El usuario elige Programación de aplicaciones
                                                       //Paso 12: Bot: "Dame la cantidad a cobrar antes de impuestos."
                    _tipoProgramación = "Programación de Videojuegos";
                    messageStatus = await _messageSendService.EnviarTexto(fromPhoneNumber, "Dame la cantidad a cobrar antes de impuestos");
                    break;

                default:

                    if (_idNombres.Contains(selectedButtonId))
                    {
                        _usuarioAUsar = _nombres[_idNombres.IndexOf(selectedButtonId)];// Paso 7: El bot recibe nombre
                        _usuarioAUsarID = selectedButtonId;
                        //Paso 8: Bot: responde con un mensaje "¡Bien! ¿Que producto vamos a facturar a {nombre} ({ID})?"
                        messageStatus = await _messageSendService.EnviarTexto(fromPhoneNumber, $"¡Bien! ¿Que producto vamos a facturar a {_usuarioAUsar} ({_usuarioAUsarID})?");
                        selectedButtonId = "ID_De_Nombre";
                    }
                    else
                    {
                        messageStatus = await _messageSendService.EnviarTexto(fromPhoneNumber, "Opción no reconocida.");
                    }
                    
                    break;
            }

            _receivedMessages.Add($"Updating _lastMessageState");
            _lastMessageState = selectedButtonId;

            return messageStatus;
        }
        private async Task<string> HandleInteractiveListMessage(//Método puramente representativo
            WebHookResponseModel webhookData,
            string selectedListId,
            string fromPhoneNumber)
        {
            //string userMessage = webhookData.entry[0].changes[0].value.messages[0].text.body;
            string messageStatus = "";
            _receivedMessages.Add($"Selecting List path");
            switch (_lastMessageState)
            {
                case "ruta_Factura_programación_3":
                    switch (selectedListId)
                    {
                        case "Gastos_en_General_ID":
                            _usoCFDI = "Gastos en general ";
                            messageStatus = await SendInvoiceMessagesAsync(fromPhoneNumber);
                            break;

                        case "Adquisicion_de_Mercancia_ID":
                            _usoCFDI = "Adquisición de mercancía ";
                            messageStatus = await SendInvoiceMessagesAsync(fromPhoneNumber);
                            break;

                        case "Honorarios_Medicos_Dentales_y_Gastos_Hospitalarios_ID":
                            _usoCFDI = "Honorarios médicos, dentales y gastos hospitalarios";
                            messageStatus = await SendInvoiceMessagesAsync(fromPhoneNumber);
                            break;

                        case "Pagos_Por_Servicios_Educativos_ID":
                            _usoCFDI = "Pagos por servicios educativos (colegiaturas)";
                            messageStatus = await SendInvoiceMessagesAsync(fromPhoneNumber);
                            break;

                        case "Devoluciones_Descuentos_o_Bonificaciones_ID":
                            _usoCFDI = "Devoluciones, descuentos o bonificaciones ";
                            messageStatus = await SendInvoiceMessagesAsync(fromPhoneNumber);
                            break;
                        default:
                            messageStatus = await _messageSendService.EnviarMensaje(fromPhoneNumber, "Seleccionar otra opción");
                            selectedListId = _lastMessageState;
                            break;
                    }
                    break;
            }
            //switch (selectedListId)
            //{
            //    case "opcion1":
            //        messageStatus = await _messageSendService.EnviarTexto(fromPhoneNumber, "Seleccionó de una lista");
            //        break;
            //}
            _receivedMessages.Add($"Updating _lastMessageState");
            _lastMessageState = selectedListId;
            return messageStatus;
        }

        private async Task<string> HandleTextMessage(
            WebHookResponseModel webhookData, 
            string fromPhoneNumber)
        {
            string messageStatus = "";
            string userMessage = webhookData.entry[0].changes[0].value.messages[0].text.body;
            switch (_lastMessageState)
            {
                case "":  
                         
                    if (userMessage != _oldUserMessage)//Paso 1: El bot recibe mensaje de WhatsApp o petición por otro medio
                    {
                        messageStatus = await _messageSendService.EnviarTexto(fromPhoneNumber, "¡Hola!");//Paso 2.1: Bot: responde con un mensaje “¡Hola!”
                        _receivedMessages.Add("Mensaje inicial: " + messageStatus);
                        //Construye la lista del botón a enviar.
                        _botones = new List<(string ButtonId, string ButtonLabelText)> { ("opcion_Generar_Factura ", "Generar factura"), ("opcion_Enviar_Documento_Firmar", "Enviar documento"), };
                        //Paso 2.2: Bot: responde con una lista de opciones:{Generar factura} y {Enviar documento a firmar}
                        messageStatus = await _messageSendService.EnviarBotonInteractivo(fromPhoneNumber, "Soy TimbraBot, ¿En que puedo ayudarte?", "Selecciona opción", "Hay 2 opciones", _botones);
                        _receivedMessages.Add("Mensaje: " + userMessage + " De: " + fromPhoneNumber);
                    }
                    break;

                case "opcion_Generar_Factura"://Paso 5: El usuario da un nombre
                                              //Paso 6: Bot muestra lista: "encontré a:"
                    if (!string.IsNullOrWhiteSpace(userMessage) && _oldUserMessage != userMessage)
                    {
                        messageStatus = await EnviarListaCurada(userMessage, fromPhoneNumber);
                    }
                    else
                    {
                        messageStatus = await _messageSendService.EnviarTexto(fromPhoneNumber, "Por favor, envía un nombre válido.");
                    }
                    break;

                case "opcion_Enviar_Documento_Firmar":
                    if (userMessage == "documento")
                    {
                        messageStatus = await _messageSendService.EnviarTexto(fromPhoneNumber, "Aquí está el documento firmado");
                        _lastMessageState = "";// Reset del estado después de procesar
                        _receivedMessages.Add("Ruta opcion_Enviar_Documento_Firmar: " + messageStatus);
                        messageStatus = await _messageSendService.EnviarDocumentoPorUrl(fromPhoneNumber, "https://test-timbrame.azurewebsites.net/Ejemplo/24a8a905-f155-4726-a27f-1451a8bf5388.pdf", "DocumentoFirmado.pdf");
                    }
                    else { messageStatus = await _messageSendService.EnviarTexto(fromPhoneNumber, "Eso no es documento"); }
                    break;

                case "ID_De_Nombre"://Paso 9: El bot recibe mensaje progra
                                    //Paso 8: Bot: muestra lista de productos
                    if (userMessage.StartsWith("prog", StringComparison.OrdinalIgnoreCase))
                    {
                        _botones = new List<(string ButtonId, string ButtonLabelText)> { ("opcion_Programación_Aplicaciones", "aplicaciones"), ("opcion_Programación_videojuegos", "videojuegos"), };
                        messageStatus = await _messageSendService.EnviarBotonInteractivo(fromPhoneNumber, "Productos de", "Programación", "de", _botones);
                    }
                    else
                    {
                        messageStatus = await _messageSendService.EnviarTexto(fromPhoneNumber, "No tenemos ese producto");
                    }
                    break;

                case "opcion_Programación_Aplicaciones": case "opcion_Programación_videojuegos": //Paso 13: El bot recibe mensaje cantidad
                                                        //Paso 14: Bot: muestra lista de productos
                    _montoFactura = userMessage;
                    _lastMessageState = "Mandando lista";
                    var listaSecciones = new List<(string, List<(string, string, string)>)>
                    {
                        ("CFDI para factura", new List<(string, string, string)>
                        {
                            ("Gastos_en_General_ID", "1", "Gastos en general"),
                            ("Adquisicion_de_Mercancia_ID", "2", "Adquisicion de mercancia"),
                            ("Honorarios_Medicos_Dentales_y_Gastos_Hospitalarios_ID", "3", "Honorarios medicos, dentales y gastos hospitalarios"),
                            ("Pagos_Por_Servicios_Educativos_ID", "4", "Pagos por servicios educativos"),
                            ("Devoluciones_Descuentos_o_Bonificaciones_ID", "5", "Devoluciones, descuentos o bonificaciones")
                        }),
                        ("Seccion 2", new List<(string, string, string)>
                        {
                            ("No_Seleccionar_1", "No seleccionar", "Esto está para que no lo selecciones"),
                            ("No_Seleccionar_2", "No seleccionar", "NO LO ELIJAS")
                        })
                    };
                    messageStatus = await _messageSendService.EnviarListaDeOpciones(fromPhoneNumber, "Uso de CFDI", "Favor de indicar el uso de CFDI para esta factura", "Hay 5 opciones", "Expandir opciones",listaSecciones);

                    //await _messageSendService.EnviarTexto(fromPhoneNumber, "Marca el número para definir el uso de CFDI para esta factura");
                    //await _messageSendService.EnviarTexto(fromPhoneNumber, "1 para Gastos en general \n2 para Adquisición de mercancía \n3 para Honorarios médicos, dentales y gastos hospitalarios \n4 para Pagos por servicios educativos (colegiaturas) \n5 para Devoluciones, descuentos o bonificaciones");
                    _lastMessageState = "ruta_Factura_programación_3";
                    break;
                    /*
                case "ruta_Factura_programación_3":
                    switch (userMessage)
                    {
                        case "1":
                            _usoCFDI = "Gastos en general ";
                            messageStatus = await SendInvoiceMessagesAsync(fromPhoneNumber, userMessage);
                            break;

                        case "2":
                            _usoCFDI = "Adquisición de mercancía ";
                            messageStatus = await SendInvoiceMessagesAsync(fromPhoneNumber, userMessage);
                            break;

                        case "3":
                            _usoCFDI = "Honorarios médicos, dentales y gastos hospitalarios";
                            messageStatus = await SendInvoiceMessagesAsync(fromPhoneNumber, userMessage);
                            break;

                        case "4":
                            _usoCFDI = "Pagos por servicios educativos (colegiaturas)";
                            messageStatus = await SendInvoiceMessagesAsync(fromPhoneNumber, userMessage);
                            break;

                        case "5":
                            _usoCFDI = "Devoluciones, descuentos o bonificaciones ";
                            messageStatus = await SendInvoiceMessagesAsync(fromPhoneNumber, userMessage);
                            break;
                    }
                    break;*/

                    default:
                    await _messageSendService.EnviarTexto(fromPhoneNumber, "Accion no reconocida");
                    break;
            }
            if (userMessage != _oldUserMessage) { _oldUserMessage = userMessage; }
            return messageStatus;
        }
        /*private async Task<string> HandleInteractiveFlowMessage(WebHookResponseModel webhookData,
            string fromPhoneNumber)
        {

        }*/
        private async Task<string> HandleDocumentMessage(WebHookResponseModel webhookData)
        {
            await _hanldeDocument.ProcesaRecibirDocumento(webhookData);

            //Aqui va la lógica de manejar documentos recibidos
            return "Documento";
        }
        private async Task<string> EnviarListaCurada(string userMessage, string fromPhoneNumber)
        {
            string messageStatus = "";
            var listaCurada = _nombres
                .Where(r => r.StartsWith(userMessage, StringComparison.OrdinalIgnoreCase))
                .Take(3).ToList();//Compara el nombre recibido sin importar mayusculas y regresa 3 nombres. 
            //El boton interactivo que se usa en esta prueba está limitado a 3 botonoe, cada botón con 20 caracteres incluyendo espacios

            if (listaCurada.Any())//Revisa si el nombre tiene coincidencias
            {
                //Construye la lista de botones, cada boton tiene un {id} y un {body_text}
                _botones = listaCurada.Select(nombre =>
                {
                    int index = _nombres.IndexOf(nombre);
                    string id = _idNombres[index];
                    return (id, nombre);
                }).ToList();

                //Construye el JSON que se usa para enviar el botón interactivo y lo manda por la función EnviarBotonInteractivo del objeto _messageSendService
                messageStatus = await _messageSendService.EnviarBotonInteractivo(
                    fromPhoneNumber,
                    "Selecciona un nombre",
                    "Opciones encontradas",
                    "Haz clic en uno de los nombres:",
                    _botones
                    );
                _receivedMessages.Add("Lista curada: " + messageStatus);
                _lastMessageState = "ruta_Factura_Nombre_Elegido";
            }
            else
            {
                messageStatus = await _messageSendService.EnviarTexto(fromPhoneNumber, "No hay coincidencias.");
                _receivedMessages.Add("Sin coincidencias: " + messageStatus);
            }
            _receivedMessages.Add("Empty 1" + messageStatus);
            return messageStatus;
        }
        //Normalizamos el número si es mexicano
        private string NormalizarNumeroMexico(string numeroTelefono)
        {
            if (numeroTelefono.StartsWith("52") && numeroTelefono.Length == 13 && numeroTelefono[2] == '1')
            {
                return "52" + numeroTelefono.Substring(3); // Remover el tercer carácter que posiblemente representa es hecho de ser un mensaje de whatsapp business
            }
            return numeroTelefono;
        }
        public async Task<string> SendInvoiceMessagesAsync(string fromPhoneNumber/*, string userMessage*/)
        {
            // Send the initial "Generando factura" message
            await _messageSendService.EnviarTexto(fromPhoneNumber, "Generando factura");

            // Send detailed invoice information
            await _messageSendService.EnviarTexto(fromPhoneNumber, $"Facturando a {_usuarioAUsar} ({_usuarioAUsarID}), el producto {_tipoProgramación}, con la cantidad a cobrar antes de impuestos {_montoFactura}, con el uso de CFDI {_usoCFDI}");

            // Save the old user message (optional, depends on your logic)
            //_oldUserMessage = userMessage;

            // Send PDF document via URL
            await _messageSendService.EnviarDocumentoPorUrl(fromPhoneNumber, "https://test-timbrame.azurewebsites.net/Ejemplo/24a8a905-f155-4726-a27f-1451a8bf5388.pdf", "Factura.pdf");
            // Reset the last message state
            _lastMessageState = "";
            // Send XML document via URL
            return await _messageSendService.EnviarDocumentoPorUrl(fromPhoneNumber, "https://test-timbrame.azurewebsites.net/Ejemplo/24a8a905-f155-4726-a27f-1451a8bf5388.xml", "Factura.xml");

        }
    }
}
