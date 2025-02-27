using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using WhatsAppPresentacionV3.Modelos;
using System.Net.Http;

namespace WhatsAppPresentacionV3.Servicios
{
    public class WhatsAppFlow
    {
        private readonly string _idTelefono;
        private readonly string _tokenAcceso;
        private readonly string _facebookGraphVersion = "v21.0";

        /// <summary>
        /// Constructor de la clase WhatsAppMessageService. Inicializa el servicio con el ID de teléfono y el token de acceso.
        /// </summary>
        /// <param name="idTelefono"> ID del número de WhatsApp Business. Actualmente se usa número test que da whatsapp.</param>
        /// <param name="tokenAcceso">Token de acceso para autenticación en la API de Facebook Graph. Se tiene que generar cada cierto tiempo y modificar en elprograma.</param>
        public WhatsAppFlow(string idTelefono, string tokenAcceso)
        {
            _idTelefono = idTelefono;
            _tokenAcceso = tokenAcceso;
        }
        private string BuildApiUrl() =>
            $"https://graph.facebook.com/{_facebookGraphVersion}/{_idTelefono}/flows";
        public async Task<string> CreateWhatsAppFlowAsync(string flowDefinitionJson)
        {
            using HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, BuildApiUrl());

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenAcceso);

            request.Content = new StringContent(JsonSerializer.Serialize(flowDefinitionJson), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                return $"Error creating flow: {errorResponse}";
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return $"Flow created successfully: {responseBody}";
        }
        public async Task<string> CrearFormularioConStringAsync(string accessToken)
        {
            string flowDefinitionJson = @"
            {
                ""name"": ""pokemon_survey"",
                ""language"": ""en_US"",
                ""content"": {
                    ""actions"": [
                        {
                            ""title"": ""Survey"",
                            ""description"": ""Help us know you better!"",
                            ""type"": ""QUESTION"",
                            ""question"": {
                                ""text"": ""What's your name?"",
                                ""input"": {
                                    ""type"": ""TEXT"",
                                    ""next_action"": ""ask_group""
                                }
                            }
                        },
                        {
                            ""id"": ""ask_group"",
                            ""type"": ""QUESTION"",
                            ""question"": {
                                ""text"": ""Which group do you belong to?"",
                                ""input"": {
                                    ""type"": ""TEXT"",
                                    ""next_action"": ""ask_pokemon""
                                }
                            }
                        },
                        {
                            ""id"": ""ask_pokemon"",
                            ""type"": ""QUESTION"",
                            ""question"": {
                                ""text"": ""What's your favorite Pokémon?"",
                                ""input"": {
                                    ""type"": ""TEXT"",
                                    ""next_action"": ""thank_user""
                                }
                            }
                        },
                        {
                            ""id"": ""thank_user"",
                            ""type"": ""MESSAGE"",
                            ""message"": {
                                ""text"": ""Thank you! Your information has been recorded.""
                            }
                        }
                    ]
                }
            }";

            return await CreateWhatsAppFlowAsync(flowDefinitionJson);
        }

    }
}