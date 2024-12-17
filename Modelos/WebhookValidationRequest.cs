namespace WhatsAppPresentacionV3.Modelos
{
    public class WebhookValidationRequest
    {
        public string Mode { get; set; }
        public string Challenge { get; set; }
        public string TokenDeVerificacion { get; set; }
    }
}
