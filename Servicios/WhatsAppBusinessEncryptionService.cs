using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace WhatsAppPresentacionV3.Servicios
{
    public class WhatsAppBusinessEncryptionService : WhatsAppMessageService
    {

        private readonly string _tokenAcceso;
        private readonly string _facebookGraphVersion = "v21.0";
        //private readonly string _WHATSAPP_BUSINESS_ACCOUNT_ID;
        private readonly string _idTelefono;
        public WhatsAppBusinessEncryptionService(string idTelefono, string tokenAcceso) : base(idTelefono, tokenAcceso)
        {
            _tokenAcceso = tokenAcceso;
            _idTelefono = idTelefono;
        }
        private string BuildApiUrl() =>
            $"https://graph.facebook.com/{_facebookGraphVersion}/{_idTelefono}/whatsapp_business_encryption";
        public async Task<string> UploadPublicKeyAsync(string publicKey)
        {
            var payload = new
            {
                encryption_key = publicKey.Replace("\n", "").Replace("\r", "")
            };

            return await EnviarMensaje(BuildApiUrl(), payload);
        }
    }
}