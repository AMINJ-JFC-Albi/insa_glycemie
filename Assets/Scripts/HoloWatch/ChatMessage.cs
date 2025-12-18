using UnityEngine;

namespace HoloWatch {
    public enum MessageSender {
        GameMaster,
        Player
    }

    [System.Serializable]
    public class ChatMessage {
        public MessageSender sender;
        public string messageId;
        [TextArea(2, 5)] public string content;
    }
}
