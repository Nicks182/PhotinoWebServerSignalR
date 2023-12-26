
namespace PhotinoWebServerSignalR
{
    public class MessageObject
    {
        public string Message { get; set; }
        public MessageObjectType MessageType { get; set; }
    }

    public enum MessageObjectType
    {
        _DoNothing,
        Greetings
    }
}
