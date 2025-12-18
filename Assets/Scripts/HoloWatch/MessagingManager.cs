using UnityEngine;
using System.Collections.Generic;

namespace HoloWatch {
    public class MessagingManager : MonoBehaviour {
        public static MessagingManager Instance;

        private List<ChatMessage> globalMessages;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;
            globalMessages = new List<ChatMessage>();
        }

        public void PlayerAskHelp(string content) {
            ChatMessage message = new ChatMessage {
                sender = MessageSender.Player,
                content = content
            };

            globalMessages.Add(message);
            HolowatchUI.Instance.RefreshMessages(globalMessages);
        }

        public void SendMessageToPlayer(string content) {
            ChatMessage message = new ChatMessage {
                sender = MessageSender.GameMaster,
                content = content
            };

            globalMessages.Add(message);
            HolowatchUI.Instance.RefreshMessages(globalMessages);
        }
    }
}
