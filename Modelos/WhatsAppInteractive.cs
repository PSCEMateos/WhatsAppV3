namespace WhatsAppPresentacionV3.Modelos
{
    public class WhatsAppInteractive
    {
        public class InteractiveButton
        {
            public string ButtonId { get; set; }
            public string ButtonLabel { get; set; }
        }
        public class WhatsAppMessage
        {
            public string From { get; set; }
            public string Body { get; set; }
            public string MessageType { get; set; }
            public InteractiveButton Interactive { get; set; }
        }
    }
}
