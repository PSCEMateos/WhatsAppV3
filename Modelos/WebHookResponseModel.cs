using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection.Metadata;

namespace WhatsAppPresentacionV3.Modelos
{
    public class WebHookResponseModel
    {
        public Entry[] entry { get; set; }
    }
    public class Entry
    {
        public Change[] changes { get; set; }
    }
    public class Change
    {
        public Value value { get; set; }
    }
    public class Value
    {
        public int ad_id { get; set; }
        public long form_id { get; set; }
        public long leadgen_id { get; set; }
        public int created_time { get; set; }
        public long page_id { get; set; }
        public int adgroup_id { get; set; }
        public Messages[] messages { get; set; }
    }
    public class Messages
    {
        public string id { get; set; }
        public string from { get; set; }
        public string type { get; set; }
        public string message_status { get; set; }
        public Context? context { get; set; }
        public Text? text { get; set; }
        public Document? document { get; set; }
        public Interactive? interactive { get; set; }
    }
    public class Context
    {
        public string from { get; set; }//Phone from whom it originally is
        public string id { get; set; } //Id of the original message (example: the id of the interactive message the user is answering)
    }
    public class Text
    {
        public string body { get; set; }
    }
    public class Document
    {
        public string link { get; set; } // URL to the document
        public string filename { get; set; } // Name of the document
        public string id { set; get; } //ID del MEDIA
        public string caption { set; get; } //Media asset caption
        public string mime_type { set; get; } // El tipo de documento que es. Ver Supported Media Types de Cloud API de whatsapp: https://developers.facebook.com/docs/whatsapp/cloud-api/reference/media#supported-media-types
    }
    public class Interactive
    {
        public string type { set; get; }
        public Button_reply? button_reply { set; get; }
        public List_reply? list_reply { set; get; }
    }
    public class Button_reply
    {
        public string id { set; get; }
        public string title { set; get; }
    }
    public class List_reply
    {
        public string id { set; get; }
        public string title { set; get; }
        public string? description {  set; get; }
    }
}