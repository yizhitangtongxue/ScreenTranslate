
namespace ScreenTranslate
{
    internal class ChatCompletionRequest
    {
        public bool stream { get; set; }
        public string model { get; set; }
        public Message[] messages { get; set; }
        public string session_id { get; set; }
        public string chat_id { get; set; }
        public string id { get; set; }
    }

    internal class Message
    {
        public string role { get; set; }
        public Content[] content { get; set; }
    }

    internal class Content
    {
        public string type { get; set; }
        public string text { get; set; }
        public string image { get; set; }
    }

}