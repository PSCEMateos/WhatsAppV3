# WhatsAppPresentacionV3
Este proyecto permite integrar un bot con la API de WhatsApp para gestionar la comunicación con los usuarios. El bot recibe y procesa mensajes entrantes, maneja respuestas interactivas y envía documentos y mensajes de texto. El proyecto está compuesto por un controlador que se encarga de gestionar las solicitudes webhook, tres servicios para manejar la lógica de la comunicación y cuatro modelos que estructuran los datos.

## Tabla de Contenidos
- [Descripción General](#descripción-general)
- [Cómo se Comunica con la API de WhatsApp](#cómo-se-comunica-con-la-api-de-whatsapp)
- [Modelos](#modelos)
  - [WebHookResponseModel](#webhookresponsemodel)
  - [UploadedMedia](#uploadedmedia)
  - [WhatsAppInteractive](#whatsappinteractive)
  - [WebhookValidationRequest](#webhookvalidationrequest)
  - [WebHookResponseModel](#webhookresponsemodel)
- [Servicios](#servicios)
  - [HandleDocument](#handledocument)
  - [SendLocalDocument](#sendlocaldocument)
  - [WhatsAppMessageService](#whatsappmessageservice)
- [Controlador](#Controlador)
- [Manejo de Errores](#manejo-de-errores)
- [Dependencias](#dependencias)

## Descripción
Este proyecto está diseñado para interactuar con la API de WhatsApp Business. El controlador recibe las solicitudes webhook, las valida y procesa los mensajes entrantes. Dependiendo del tipo de mensaje (texto, interactivo o documento), el controlador delega el procesamiento a los servicios correspondientes para manejar la lógica de negocio. Los servicios son responsables de gestionar la lógica de negocio y la generación de respuestas adecuadas.

## Cómo se Comunica con la API de WhatsApp
El bot se comunica con la API de WhatsApp a través de solicitudes HTTP POST y GET, así como el uso de webhooks para recibir mensajes. Cuando un usuario interactúa con el bot en WhatsApp, WhatsApp envía un webhook a la URL configurada. El controlador procesa estos webhooks y responde dependiendo del punto del flujo en el que se esté y lo que solicita el cliente.

### Comunicación:

1. **Validación del Webhook:**
   - Cuando se configura el webhook con WhatsApp, se envía una solicitud `GET` a la URL configurada para validar la conexión.
   - El controlador valida el token de verificación y responde con el `Challenge` proporcionado por WhatsApp.

2. **Manejo de Mensajes Recibidos:**
   - Cuando WhatsApp envía un mensaje a la URL del webhook, se recibe como una solicitud `POST`.
   - El controlador procesa el mensaje entrante y, dependiendo de su tipo (texto, interactivo o documento), invoca los servicios correspondientes para manejar la lógica de negocio.

3. **Respuesta del Bot:**
   - Después de procesar el mensaje, el controlador usa el servicio `WhatsAppMessageService` para enviar la respuesta de vuelta al usuario a través de la API de WhatsApp.
   - El bot puede enviar mensajes de texto, respuestas interactivas (botones y listas) o documentos, según sea necesario.

## Modelos

### WebHookResponseModel
En este modelo se guarda un todo mensaje recibido por WhatsApp Ya que todos los mensajes de WhatsApp tienen el siguente formato JSON:
```json
{
  "object": "whatsapp_business_account",
  "entry": [
    {
      "id": "Nuestra_ID_Business_Account",
      "changes": [
        {
          "value": {
            "messaging_product": "whatsapp",
            "metadata": {
              "display_phone_number": "Nuestro_Numero",
              "phone_number_id": "ID_de_Nuestro_Numero"
            }
            # Payload Dependiente de tipo de mensaje
          },
          "field": "messages"
        }
      ]
    }
  ]
}
```

### UploadedMedia
Este modelo guarda el ID y la URL extraída de haber subido un documento a WhatsApp.

### WhatsAppInteractive
Este modelo se usa para guardar la información que se le enviará a `EnviarListaDeOpciones` o `EnviarBotonInteractivo`.


### WebhookValidationRequest
Simplifica la vaidación del webhook.

## Servicios

### HandleDocument
Maneja todas las acciones relacionadas con la recepción de documentos.

#### Flujo de recibir un documento
1. Se recibe el webhook de WhatsApp respecto al documento, de donde se extraen:
    - El nombre del documento
    - El MIME type (tipo de documento, Ej: PDF, XML, texto)
    - El ID que WhatsApp le otorga al documento
2. Se prepara la localización de descarga con el nombre del documento.
3. Usando la función `ObtenerURLDocumentoDeID`, se solicita la URL temporal asignada por WhatsApp para descargar.
4. Usando la función `DescargarDocumento`, se solicita el documento para guardarlo en un byte array.
5. Al recibir el documento, se guarda en la localización de descarga.

#### Variables
Recibe el token de acceso y `WebHookResponseModel` para procesar.
Retorna "Documento procesado y descargado correctamente", o un error.

### SendLocalDocument
Recibe un token de verificación y el ID de teléfono (modificable a un número de teléfono) al que se enviará un documento guardado localmente.

#### Flujo de mandar un documento local
1. Se confirma que el documento indicado exista.
2. Se transforma el documento a un array binario.
3. Se recaba información del documento:
    - Nombre: string `nombreDocumentoMandar`
    - Tipo: `mimeType`, utilizando la función `GetMimeType`
4. Se hace la solicitud HTTP para enviar el array binario a WhatsApp.
5. Se obtiene el ID del documento proporcionado por WhatsApp: `idDocumentoMandar`.
6. Se envía el documento con la función `EnviarDocumentoPorId`.

#### GetMimeType
Recaba la extensión del documento y la transforma a la extensión usada por WhatsApp.

### WhatsAppMessageService
Servicio principal para enviar distintos tipos de mensajes.

#### Constructor y variables de todo el servicio
- `idTelefono`: ID del número de WhatsApp Business (de la empresa). Actualmente se usa un número de prueba proporcionado por WhatsApp.
- `tokenAcceso`: Token de acceso para autenticación en la API de Facebook Graph. Debe generarse periódicamente y modificarse en el programa.
- `facebookGraphVersion`: Versión de Facebook Graph con la que el programa es compatible.
- `BuildApiUrl`: URL a la que se manda la solicitud de mensajes.

#### EnviarMensaje
Función que las demás funciones llaman para enviar el mensaje. Envía un mensaje de texto a través de la API de Facebook Graph.
Todas las demás funciones construyen el mensaje a enviar y llaman a esta función para enviarlo.

##### Flujo
1. Crea un cliente HTTP que se borra al terminar la función.
2. Añade los headers:
    - "Bearer" con el token de autenticación
    - "Content-Type" con "application/json"
3. Usando `POST`, manda el mensaje y espera la respuesta.

##### Variables
- `urlFacebookGraph`: URL del endpoint de la API de Facebook Graph a la que las funciones llaman. Ejemplo: `https://graph.facebook.com/v21.0/9999999999/messages`.
- `message`: Objeto que contiene el mensaje en formato JSON. Se usan arrays debido a la presencia de listas y botones con opciones modificables.

#### EnviarTexto
Crea el JSON para enviar un documento a través de un enlace URL público.

##### Variables del JSON
- `messaging_product`: Siempre es "whatsapp".
- `to`: Emparejado con el número al que se le enviará el mensaje, usa la variable `numeroTelefonoObjetivo`.
- `type`: "text" ya que es un texto simple.
- `text`: El mensaje que verá el receptor.

#### EnviarDocumentoPorUrl
Crea el JSON para enviar un documento a través de un enlace URL público

##### Variables del JSON
- `messaging_product`: Siempre es "whatsapp".
- `to`: Emparejado con el número al que se le enviará el mensaje, usa la variable `numeroTelefonoObjetivo`.
- `type`: "document" ya que se enviará un documento.
- `document`: Objeto compuesto por:
    - `link`: URL público del documento a enviar, usa la variable `link`.
    - `filename`: Nombre del archivo a enviar, usa la variable `nombreArchivo`.

#### EnviarDocumentoPorId
Envía un documento utilizando su ID en la nube de WhatsApp Business. Requiere que WhatsApp YA le haya asignado un ID.
Un documento puede subirse a WhatsApp manualmente o utilizando la función `SendLocalDocument`.

##### Variables del JSON
- `messaging_product`: Siempre es "whatsapp".
- `to`: Emparejado con el número al que se le enviará el mensaje, usa la variable `numeroTelefonoObjetivo`.
- `type`: "document" ya que se enviará un documento.
- `document`: Objeto compuesto por:
    - `id`: ID asignado por WhatsApp al documento a enviar, usa la variable `idDocumento`.
    - `filename`: Nombre del archivo a enviar, usa la variable `nombreArchivo`.

#### EnviarBotonConUrl
Envía un botón con un enlace URL. Al precionar el botón, se habre el URL

##### Variables del JSON
- messaging_product: siempre es "whatsapp"
- to: emparejado con el número al que se le va a mandar mensaje, usa la variable "numeroTelefonoObjetivo"
- type: es "interactive" ya que se manda un objeto interactivo
- interactive: objeto compuesto por
    a. type : es "button" ya que el objeto interactivo es un botón
    b. header: objeto compuesto por
        1.type: es "text" pero si es modificado puede ser media para mandar una imágen
        2.text: es el encabesado del mensaje, usa la variable "encabezado"
    c. body: es el texto principal del mensaje que verá el receptor del mensaje, usa la variable "cuerpo"
    d. footer: es el texto pequeño del mensaje, se ve en gris y es posible que algunas personas no lo puedan leer, usa la variable "pie"
    e. action: objeto compuesto por
        1.buttons: objeto compuesto por
            + type: es la acción al presionar el botón, en este caso es "url"
            + url: URL público al que se manda al receptor del mensaje al presionar el botón, usa la variable "link"
            + text: texto que se muestra en el botón, hasta 20 caracteres(incluyendo espacios), usa la variable "textoBoton"

#### EnviarImagenPorUrl
Envía una imagen utilizando un enlace URL público.

##### Variables del JSON
- messaging_product: siempre es "whatsapp"
- to: emparejado con el número al que se le va a mandar mensaje, usa la variable "numeroTelefonoObjetivo"
- type: es "image" ya que se manda una imágen
- image: objeto compuesto por la variable link: el URL público de la imágen

#### EnviarBotonInteractivo
Envía un mensaje con hasta 3 botones interactivos.
Empieza revisando que no sean más de 3 botones solicitados

##### Variables de cada botón
- type: acción del botón al ser presionado, es este caso "reply" ya que retorna una acción al programa
- reply: objeto compuesto por
    a. id: mensaje que retorna al ser presionado, usa la variable "ButtonId" de la lista de "botones""
    b. title: texto que se muestra en el botón, hasta 20 caracteres(incluyendo espacios), usa la variable "ButtonLabelText" de la lista de "botones"

##### Variables del JSON
- messaging_product: siempre es "whatsapp"
- to: emparejado con el número al que se le va a mandar mensaje, usa la variable "numeroTelefonoObjetivo"
- type: es "interactive" ya que se manda un objeto interactivo
- interactive: objeto compuesto por
    a. type : es "button" ya que el objeto interactivo es un botón o más
    b. header: objeto compuesto por
        1.type: es "text" pero si es modificado puede ser media para mandar una imágen
        2.text: es el encabesado del mensaje, usa la variable "mensajeEncabezado"
    c. body: es el texto principal del mensaje que verá el receptor del mensaje, usa la variable "cuerpoTexto"
    d. footer: es el texto pequeño del mensaje, se ve en gris y es posible que algunas personas no lo puedan leer, usa la variable "pieTexto"
    e. action: objeto compuesto por
        1.buttons: objeto compuesto por un array de botones cada uno con las variables de un botón


#### EnviarListaDeOpciones
Envía una lista de opciones interactivas
Empieza revisando que no sean más de 10 secciones solicitadas, cada sección aguanta hasta 10 botones

##### Variables de cada botón
Los botones se de una lista son referidos como "rows"

- id: mensaje que retorna al ser presionado, usa la variable "OptionId" de la lista de "Options"
- title: texto que se muestra en el botón, hasta 20 caracteres(incluyendo espacios), usa la variable "OptionTitle" de la lista de "Options"
- description: texto alargado usado para explicar que representa cada opción, usa la variable "OptionDescription" de la lista de "Options"

##### Variables de cada sección
- title: texto que se muestra al inicio de cada sección, representa el título de la sección y la diferencia de funcionalidad de cada una
- rows: objeto compuesto por un array de botones cada uno con las variables de un botón(row)

##### Variables del JSON
- messaging_product: siempre es "whatsapp"
- to: emparejado con el número al que se le va a mandar mensaje, usa la variable "numeroTelefonoObjetivo"
- type: es "interactive" ya que se manda un objeto interactivo
- interactive: objeto compuesto por
    a. type : es "list" ya que el objeto interactivo es una lista de botones con secciones
    b. header: objeto compuesto por
        1.type: es "text"
        2.text: es el encabesado del mensaje, usa la variable "encabezado"
    c. body: es el texto principal del mensaje que verá el receptor del mensaje, usa la variable "cuerpo"
    d. footer: es el texto pequeño del mensaje, se ve en gris y es posible que algunas personas no lo puedan leer, usa la variable "pie"
    e. action: objeto compuesto por
        1. sections: objeto compuesto por un array de secciones cada una con las variables de una sección
        2. button: botón principal, al ser presionado muestra las secciones


#### EnviarMensajeError
Función por ser eliminada, se usa para marcar un error al desarrollar y mandarlo a un teléfono

## Controlador

### WebhookValidation
Destino al que los webhooks están apuntados. Valida que sean de WhatsApp usando un token de verificación que se le comparte a WhatsApp manualmente.

### Presentable

#### Flujo Presentable
1.	El programa recibe mensaje de WhatsApp
2.	Responde con un mensaje “¡Hola!” y un botón interactivo de 2 opciones:
    -	Generar factura
    -	Enviar documento a firmar
3.	El usuario elige generar factura
4.	Responde con un mensaje "Generando factura. ¿a quien se la vas a generar?"
5.	El usuario responde con un nombre, ejemplo: "Jesús"
6.	Responde con un botón interactivo de hasta 3 opciones "encontré a"
    -	Jesús Trejo
    -	Jesús Garza
    -	Jesús Zepeda
7.	El usuario elige Jesús Trejo 
8.	Responde con un mensaje "¡Bien! ¿Que producto vamos a facturar a Jesús Trejo (TEDJ800706QA3)?"
9.	El usuario responde escribiendo Programación
10.	Responde con un botón interactivo de 2 opciones y con texto "En tus productos encontré los siguientes"
    -	Programación de aplicaciones
    -	Programación de videojuegos
11.	El usuario elige Programación de aplicaciones
12.	Responde con un mensaje "Dame la cantidad a cobrar antes de impuestos"
13.	Usuario: 10,000
14.	Responde con una lista interactiva con el texto "¿Cual es el uso de CFDI para esta factura?"
    a.	Gastos en general
    b.	Adquisición de mercancía
    c.	Honorarios médicos, dentales y gastos hospitalarios
    d.	Pagos por servicios educativos (colegiaturas)
    e.	Devoluciones, descuentos o bonificaciones
15.	Responde con un mensaje "Generando factura"
16.	Manda pdf y xml con mensaje: Aquí está tu factura


#### Manejo de mensajes


## Manejo de Errores


## Dependencias

- **API de WhatsApp:** Se requiere acceso a la API de WhatsApp Business para enviar y recibir mensajes.
- **ASP.NET Core:** El controlador está construido usando ASP.NET Core para servicios web API.
- **Servicio de Envío de Mensajes:** El `WhatsAppMessageService` se usa para enviar mensajes y documentos al usuario.
