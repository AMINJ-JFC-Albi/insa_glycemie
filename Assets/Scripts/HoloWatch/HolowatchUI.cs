using UnityEngine;
using System.Collections.Generic;

namespace HoloWatch {
    public class HolowatchUI : MonoBehaviour {
        public static HolowatchUI Instance;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        public void RefreshObjectives() {
            foreach (ObjectiveData obj in ObjectiveManager.Instance.GetActiveObjectives()) {
                Debug.Log("Objectif : " + obj.title);
            }
        }

        public void OpenConversation(ObjectiveData objective) {
            DisplayConversation(objective.conversation);
        }

        public void DisplayConversation(List<ChatMessage> messages) {
            foreach (ChatMessage msg in messages) {
                Debug.Log(msg.sender + " : " + msg.content);
            }
        }

        public void RefreshMessages(List<ChatMessage> messages) {
            foreach (ChatMessage msg in messages) {
                Debug.Log(msg.sender + " : " + msg.content);
            }
        }

        public void NotifyDoc(string text) {
            MessagingManager.Instance.SendMessageToPlayer(text);
        }
    }
}
