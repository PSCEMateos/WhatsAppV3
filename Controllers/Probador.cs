using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using WhatsAppPresentacionV3.Servicios;

namespace WhatsAppPresentacionV3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Probador : ControllerBase
    {
        private static string businessAccountId = "461054850432097";
        private static string Token_de_acceso = "EAAGf6qvOtkYBOytETDI5BPRYYm1zWNeWMemGQj8dAiFX39ZAzH0pbr0XQQhZBGPW1ku29fJ6MJ36Bl2BzMtG2FnrCodyLYqqjjYUmBPoMFYjAygQCDQLELNgqXN6pv8bnxFP2azzyLVrM9LD4fZAEF72cr0KSyyjOMHuZCslnqKu1UUoJ4C4hZBDK9p0ScForVn1mZA8aqWy690pGhQMr2829rucEZD";
        private readonly WhatsAppBusinessService _BuisnessService;
        public Probador()
        {
            _BuisnessService = new WhatsAppBusinessService(Token_de_acceso, businessAccountId);
        }

        [HttpPost("crear-plantilla")]
        public async Task<IActionResult> CrearPlantilla(
            /*[FromQuery] string nombrePlantilla,*/
            [FromQuery] string categoria,
            [FromQuery] string idioma)
        {
            string nombrePlantilla = "amigo_plantilla_v2";

            if (string.IsNullOrEmpty(nombrePlantilla) || string.IsNullOrEmpty(categoria) || string.IsNullOrEmpty(idioma))
                return BadRequest("All fields (nombrePlantilla, categoria, idioma) are required.");

            string response = await _BuisnessService.CrearPlantillaTextAsync(nombrePlantilla, categoria, idioma);
            if (response.StartsWith("Error"))
                return StatusCode(500, response);

            return Ok(response);
        }

        private readonly string _facebookGraphVersion = "v21.0";
        private string BuildApiUrl() =>
            $"https://graph.facebook.com/{_facebookGraphVersion}/{businessAccountId}/message_templates";
        /*[HttpPost("crear-plantilla/probador1")]
        public async Task<IActionResult> CrearPlantillaProbador1()
        {
            string nombrePlantilla = $"amigo_plantilla_v2_slash_n";
            string categoria = "UTILITY";
            string idioma = "es_MX";

            var plantilla = new
            {
                name = nombrePlantilla,
                category = categoria.ToUpper(),
                language = idioma,
                components = new[]
            {
                new
                {
                    type = "BODY",
                    text = "Thank you for your order, {{1}}! \n Your confirmation number is {{2}}.\nIf you have any questions, please use the buttons below to contact support.\n      Thank you for being a customer!",
                    example = new
                    {
                        body_text = new[]
                        {
                            new[]{"Pablo", "860198-230332"}
                        }
                    }
                }
            }
            };

            using HttpClient client = new HttpClient();
            string urlBUSINESS = BuildApiUrl();
            var request = new HttpRequestMessage(HttpMethod.Post, urlBUSINESS);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token_de_acceso);
            request.Content = new StringContent(JsonSerializer.Serialize(plantilla), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error al crear la plantilla: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return Ok("Response: " + responseBody);
        }
        [HttpPost("crear-plantilla/probador2")]
        public async Task<IActionResult> CrearPlantillaProbador2()
        {
            string nombrePlantilla = $"amigo_plantilla_v3_slash_slash_n";
            string categoria = "UTILITY";
            string idioma = "es_MX";

            var plantilla = new
            {
                name = nombrePlantilla,
                category = categoria.ToUpper(),
                language = idioma,
                components = new[]
            {
                new
                {
                    type = "BODY",
                    text = "Thank you for your order, {{1}}! \\n Your confirmation number is {{2}}.\\nIf you have any questions, please use the buttons below to contact support.\\n      Thank you for being a customer!",
                    example = new
                    {
                        body_text = new[]
                        {
                            new[]{"Pablo", "860198-230332"}
                        }
                    }
                }
            }
            };

            using HttpClient client = new HttpClient();
            string urlBUSINESS = BuildApiUrl();
            var request = new HttpRequestMessage(HttpMethod.Post, urlBUSINESS);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token_de_acceso);
            request.Content = new StringContent(JsonSerializer.Serialize(plantilla), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error al crear la plantilla: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return Ok("Response: " + responseBody);
        }
        [HttpPost("crear-plantilla/probador3")]
        public async Task<IActionResult> CrearPlantillaProbador3()
        {
            string nombrePlantilla = $"amigo_plantilla_v4_slash_r_slash_n";
            string categoria = "UTILITY";
            string idioma = "es_MX";

            var plantilla = new
            {
                name = nombrePlantilla,
                category = categoria.ToUpper(),
                language = idioma,
                components = new[]
            {
                new
                {
                    type = "BODY",
                    text = "Thank you for your order, {{1}}! \r\n Your confirmation number is {{2}}.\r\nIf you have any questions, please use the buttons below to contact support.\r\n      Thank you for being a customer!",
                    example = new
                    {
                        body_text = new[]
                        {
                            new[]{"Pablo", "860198-230332"}
                        }
                    }
                }
            }
            };

            using HttpClient client = new HttpClient();
            string urlBUSINESS = BuildApiUrl();
            var request = new HttpRequestMessage(HttpMethod.Post, urlBUSINESS);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token_de_acceso);
            request.Content = new StringContent(JsonSerializer.Serialize(plantilla), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error al crear la plantilla: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return Ok("Response: " + responseBody);
        }
        [HttpPost("crear-plantilla/probador4")]
        public async Task<IActionResult> CrearPlantillaProbador4()
        {
            string nombrePlantilla = $"amigo_plantilla_v5_concatenacion_texto_multiliea";
            string categoria = "UTILITY";
            string idioma = "es_MX";

            var plantilla = new
            {
                name = nombrePlantilla,
                category = categoria.ToUpper(),
                language = idioma,
                components = new[]
            {
                new
                {
                    type = "BODY",
                    text = "Thank you for your order, {{1}}!\n"+"Your confirmation number is {{2}}.\n" +
                   "If you have any questions, please use the buttons below to contact support.\n" +
                   "      Thank you for being a customer!",
                    example = new
                    {
                        body_text = new[]
                        {
                            new[]{"Pablo", "860198-230332"}
                        }
                    }
                }
            }
            };

            using HttpClient client = new HttpClient();
            string urlBUSINESS = BuildApiUrl();
            var request = new HttpRequestMessage(HttpMethod.Post, urlBUSINESS);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token_de_acceso);
            request.Content = new StringContent(JsonSerializer.Serialize(plantilla), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error al crear la plantilla: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return Ok("Response: " + responseBody);
        }
        [HttpPost("crear-plantilla/probador5")]
        public async Task<IActionResult> CrearPlantillaProbador5()
        {
            string nombrePlantilla = $"amigo_plantilla_v6_uso_tabs_explicitos";
            string categoria = "UTILITY";
            string idioma = "es_MX";

            var plantilla = new
            {
                name = nombrePlantilla,
                category = categoria.ToUpper(),
                language = idioma,
                components = new[]
            {
                new
                {
                    type = "BODY",
                    text = "Thank you for your order, {{1}}! \n\t Your confirmation number is {{2}}.\tIf you have any questions, please use the buttons below to contact support.\t      Thank you for being a customer!",
                    example = new
                    {
                        body_text = new[]
                        {
                            new[]{"Pablo", "860198-230332"}
                        }
                    }
                }
            }
            };

            using HttpClient client = new HttpClient();
            string urlBUSINESS = BuildApiUrl();
            var request = new HttpRequestMessage(HttpMethod.Post, urlBUSINESS);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token_de_acceso);
            request.Content = new StringContent(JsonSerializer.Serialize(plantilla), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error al crear la plantilla: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return Ok("Response: " + responseBody);
        }*/
        [HttpPost("crear-plantilla/probador6")]
        public async Task<IActionResult> CrearPlantillaProbador6()
        {
            string nombrePlantilla = $"amigo_plantilla_v7_unicode_saltos_slash_u000A";
            string categoria = "UTILITY";
            string idioma = "es_MX";

            var plantilla = new
            {
                name = nombrePlantilla,
                category = categoria.ToUpper(),
                language = idioma,
                components = new[]
            {
                new
                {
                    type = "BODY",
                    text = "Thank you for your order, {{1}}!\u000A  Your confirmation number is {{2}}.\u000A If you have any questions, please use the buttons below to contact support.\u000A Thank you for being a customer!",
                    example = new
                    {
                        body_text = new[]
                        {
                            new[]{"Pablo", "860198-230332"}
                        }
                    }
                }
            }
            };

            using HttpClient client = new HttpClient();
            string urlBUSINESS = BuildApiUrl();
            var request = new HttpRequestMessage(HttpMethod.Post, urlBUSINESS);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token_de_acceso);
            request.Content = new StringContent(JsonSerializer.Serialize(plantilla), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error al crear la plantilla: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return Ok("Response: " + responseBody);
        }

        [HttpPost("crear-plantilla/probador7")]
        public async Task<IActionResult> CrearPlantillaProbador7()
        {
            string nombrePlantilla = $"amigo_plantilla_v8_header_simple";
            string categoria = "UTILITY";
            string idioma = "es_MX";

            var plantilla = new
            {
                name = nombrePlantilla,
                category = categoria.ToUpper(),
                language = idioma,
                components = new[]
            {
                    new
                    {
                        type = "HEADER",
                        text = "Thank you for your order!"
                    },
                    new
                    {
                        type = "BODY",
                        text = "Thank you for your order! \n Your confirmation number is.\nIf you have any questions, please use the buttons below to contact support.\n      Thank you for being a customer!",
                    }
                }
            };

            using HttpClient client = new HttpClient();
            string urlBUSINESS = BuildApiUrl();
            var request = new HttpRequestMessage(HttpMethod.Post, urlBUSINESS);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token_de_acceso);
            request.Content = new StringContent(JsonSerializer.Serialize(plantilla), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error al crear la plantilla: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return Ok("Response: " + responseBody);
        }

        [HttpPost("crear-plantilla/probador8")]
        public async Task<IActionResult> CrearPlantillaProbador8()
        {
            string nombrePlantilla = $"amigo_plantilla_v9_header_example_y_footer";
            string categoria = "MARKETING";
            string idioma = "es_MX";

            var plantilla = new
            {
                name = nombrePlantilla,
                category = categoria.ToUpper(),
                language = idioma,
                components = new object[]
                {
                    new
                    {
                        type = "HEADER",
                        format = "TEXT",
                        text = "Our {{1}} is on!",
                        example = new
                        {
                            header_text = new[] { "Summer Sale" } // Ejemplo para el header
                        }
                    },
                    new
                    {
                        type = "BODY",
                        text = "Shop now through {{1}} and use code {{2}} to get {{3}} off of all merchandise.",
                        example = new
                        {
                            body_text = new[]
                            {
                                new[] { "the end of August", "25OFF", "25%" } // Ejemplo para el cuerpo
                            }
                        }
                    },
                    new
                    {
                        type = "FOOTER",
                        text = "Use the buttons below to manage your marketing subscriptions"
                    },
                    new
                    {
                        type = "BUTTONS",
                        buttons = new[]
                        {
                            new
                            {
                                type = "QUICK_REPLY",
                                text = "Unsubscribe from Promos"
                            },
                            new
                            {
                                type = "QUICK_REPLY",
                                text = "Unsubscribe from All"
                            }
                        }
                    }
                }
            };



            using HttpClient client = new HttpClient();
            string urlBUSINESS = BuildApiUrl();
            var request = new HttpRequestMessage(HttpMethod.Post, urlBUSINESS);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token_de_acceso);
            request.Content = new StringContent(JsonSerializer.Serialize(plantilla), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error al crear la plantilla: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return Ok("Response: " + responseBody);
        }

        [HttpPost("crear-plantilla/probador11")]
        public async Task<IActionResult> CrearPlantillaProbador11()
        {
            string nombrePlantilla = $"plantilla_v11_prueba_video";
            string categoria = "MARKETING";
            string idioma = "es_MX";

            var plantilla = new
            {
                name = nombrePlantilla,
                category = categoria.ToUpper(),
                language = idioma,
                components = new[]
            {
                    new
                    {
                        type = "BODY",
                        text = "Este texto es para probar! \n Estás leyendo la segunda línea.\n Y si te preguntas que si se ve así por el largo, esta línea debe ser muy larga.\n Gracias!",
                    }
                }
            };

            using HttpClient client = new HttpClient();
            string urlBUSINESS = BuildApiUrl();
            var request = new HttpRequestMessage(HttpMethod.Post, urlBUSINESS);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token_de_acceso);
            request.Content = new StringContent(JsonSerializer.Serialize(plantilla), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error al crear la plantilla: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return Ok("Response: " + responseBody);
        }

        [HttpPost("crear-plantilla/probador12")]
        public async Task<IActionResult> CrearPlantillaProbador12()
        {
            string nombrePlantilla = $"plantilla_v12_prueba_video";
            string categoria = "MARKETING";
            string idioma = "es_MX";

            var plantilla = new
            {
                name = nombrePlantilla,
                category = categoria.ToUpper(),
                language = idioma,
                components = new[]
            {
                    new
                    {
                        type = "BODY",
                        text = "Este texto es para probar!{{1}} \n Estás leyendo la segunda línea.\n Y si te preguntas que si se ve así por el largo {{2}} , esta línea debe ser muy larga.\n Gracias!",
                        example = new
                        {
                        body_text = new[]
                            {
                                new[]{ "https://developers.facebook.com/docs/whatsapp", "860198-230332"}
                            }
                        }
                    }
                }
            };

            using HttpClient client = new HttpClient();
            string urlBUSINESS = BuildApiUrl();
            var request = new HttpRequestMessage(HttpMethod.Post, urlBUSINESS);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token_de_acceso);
            request.Content = new StringContent(JsonSerializer.Serialize(plantilla), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error al crear la plantilla: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return Ok("Response: " + responseBody);
        }
        [HttpPost("crear-plantilla/probador13")]//nuevo 07
        public async Task<IActionResult> CrearPlantillaProbador13()
        {
            string nombrePlantilla = $"plantilla_v13_prueba_video";
            string categoria = "MARKETING";
            string idioma = "es_MX";

            var plantilla = new
            {
                name = nombrePlantilla,
                category = categoria.ToUpper(),
                language = idioma,
                components = new[]
            {
                    new
                    {
                        type = "BODY",
                        text = "Este texto es para probar!{{1}} \n Estás leyendo la segunda línea.\n Y si te preguntas que si se ve así por el largo {{2}} , esta línea debe ser muy larga.\n Gracias!",
                        example = new
                        {
                        body_text = new[]
                            {
                                new[]{ "https://flowwindow-test.azurewebsites.net/?t=957a7e35f50b419782d0d4d537c7ccf8", "860198-230332"}
                            }
                        }
                    }
                }
            };

            using HttpClient client = new HttpClient();
            string urlBUSINESS = BuildApiUrl();
            var request = new HttpRequestMessage(HttpMethod.Post, urlBUSINESS);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token_de_acceso);
            request.Content = new StringContent(JsonSerializer.Serialize(plantilla), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error al crear la plantilla: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return Ok("Response: " + responseBody);
        }

        [HttpPost("crear-plantilla/probador14")]
        public async Task<IActionResult> CrearPlantillaProbador14()
        {
            string nombrePlantilla = $"plantilla_v14_prueba_video";
            string categoria = "MARKETING";
            string idioma = "es_MX";

            var plantilla = new
            {
                name = nombrePlantilla,
                category = categoria.ToUpper(),
                language = idioma,
                components = new[]
            {
                    new
                    {
                        type = "BODY",
                        text = "Este texto es para probar!{{1}} \n Estás leyendo la segunda línea.\n Y si te preguntas que si se ve así por el largo {{2}} , esta línea debe ser muy larga.\n Gracias!",
                        example = new
                        {
                        body_text = new[]
                            {
                                new[]{ "https://developers.facebook.com/docs/whatsapp", "860198\n-230332" }
                            }
                        }
                    }
                }
            };

            using HttpClient client = new HttpClient();
            string urlBUSINESS = BuildApiUrl();
            var request = new HttpRequestMessage(HttpMethod.Post, urlBUSINESS);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token_de_acceso);
            request.Content = new StringContent(JsonSerializer.Serialize(plantilla), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error al crear la plantilla: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return Ok("Response: " + responseBody);
        }
        [HttpPost("crear-plantilla/probador15")]
        public async Task<IActionResult> CrearPlantillaProbador15()
        {
            string nombrePlantilla = $"plantilla_v15_prueba_video";
            string categoria = "MARKETING";
            string idioma = "es_MX";

            var plantilla = new
            {
                name = nombrePlantilla,
                category = categoria.ToUpper(),
                language = idioma,
                components = new[]
            {
                    new
                    {
                        type = "BODY",
                        text = "Este texto es para probar!{{1}}\n Y si te preguntas que si se ve así por el largo {{2}} , esta línea debe ser muy larga.\n Gracias!",
                        example = new
                        {
                        body_text = new[]
                            {
                                new[]{ "https://developers.facebook.com/docs/whatsapp \nEstás leyendo la segunda línea.", "860198\n-230332" }
                            }
                        }
                    }
                }
            };

            using HttpClient client = new HttpClient();
            string urlBUSINESS = BuildApiUrl();
            var request = new HttpRequestMessage(HttpMethod.Post, urlBUSINESS);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token_de_acceso);
            request.Content = new StringContent(JsonSerializer.Serialize(plantilla), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error al crear la plantilla: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            string responseBody = await response.Content.ReadAsStringAsync();
            return Ok("Response: " + responseBody);
        }
    }
}
