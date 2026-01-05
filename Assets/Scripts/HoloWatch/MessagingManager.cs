using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HoloWatch {
    public class MessagingManager : MonoBehaviour {
        public static MessagingManager Instance;

        private Dictionary<string, ContactInfo> contactInfos;
        private Dictionary<string, List<ChatMessage>> messagesRecord;
        private Dictionary<string, List<string>> nextHints;
        
        [SerializeField] private Transform contactContainer;
        [SerializeField] private GameObject contactPrefab;
        
        private string currentConversation;
        [SerializeField] private TMP_Text conversationNameText;
        [SerializeField] private Button helpButton;
        [SerializeField] private Transform messageContainer;
        [SerializeField] private GameObject messagePrefab;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;
            
            contactInfos = new Dictionary<string, ContactInfo>();
            messagesRecord = new Dictionary<string, List<ChatMessage>>();
            nextHints = new Dictionary<string, List<string>>();
            
            HolowatchUI.ClearChildren(contactContainer);
        }
        
        public void CreateConversation(ObjectiveData objective) {
            GameObject obj = Instantiate(contactPrefab, contactContainer);

            ContactInfo contact = obj.GetComponentInChildren<ContactInfo>();
            contact.Initialize(objective.conversationName, objective.conversationAvatar);

            obj.GetComponentInChildren<Button>().onClick.AddListener(() => OpenConversation(objective.objectiveId));
            
            contactInfos.Add(objective.objectiveId, contact);
            messagesRecord.Add(objective.objectiveId, new List<ChatMessage>());
            nextHints.Add(objective.objectiveId, new List<string>());
        }
        
        public void OpenConversation(string objectiveId) {
            HolowatchUI.ClearChildren(messageContainer);
            
            conversationNameText.text = contactInfos[objectiveId].GetName();
            List<ChatMessage> listMessage = messagesRecord[objectiveId];
            foreach (ChatMessage chatMessage in listMessage) {
                GameObject obj = Instantiate(messagePrefab, messageContainer);
                MessageInfo messageInfo = obj.GetComponentInChildren<MessageInfo>();
                messageInfo.Initialize(chatMessage.content, chatMessage.sender == MessageSender.Player);
            }
            currentConversation = objectiveId;
            helpButton.interactable = CanHaveHelp(objectiveId);
            HolowatchUI.Instance.SwitchPage(HolowatchPage.Conversation);
        }
        
        public string GetCurrentConversation() {
            return currentConversation;
        }
        
        public void AddMessage(string objectiveId, ChatMessage chatMessage) {
            if (!messagesRecord.ContainsKey(objectiveId)) return;
            
            messagesRecord[objectiveId].Add(chatMessage);
            contactInfos[objectiveId].RefreshInfo(chatMessage.content);
            if (HolowatchUI.Instance.ThisPageIsActive(HolowatchPage.Conversation) && objectiveId == currentConversation) {
                OpenConversation(objectiveId);
            }
        }
        
        public void SetNextHints(string objectiveId, List<string> hints) {
            nextHints[objectiveId] = hints;
            if (HolowatchUI.Instance.ThisPageIsActive(HolowatchPage.Conversation) && objectiveId == currentConversation) {
                helpButton.interactable = CanHaveHelp(objectiveId);
            }
        }
        
        public bool TryGetNextHint(string objectiveId, out string hint) {
            if (CanHaveHelp(objectiveId)) {
                hint = nextHints[objectiveId][0];
                nextHints[objectiveId].RemoveAt(0);
                helpButton.interactable = CanHaveHelp(objectiveId);
                return true;
            }
            hint = null;
            return false;
        }
        
        private bool CanHaveHelp(string objectiveId) {
            return nextHints[objectiveId].Count > 0;
        }
    }
}
