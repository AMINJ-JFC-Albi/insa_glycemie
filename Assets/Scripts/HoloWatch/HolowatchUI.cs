using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HoloWatch {
    public enum HolowatchPage {
        None,
        Objective,
        Messaging,
        Conversation
    }

    [DefaultExecutionOrder(-100)]
    public class HolowatchUI : MonoBehaviour {
        public static HolowatchUI Instance;

        [SerializeField] private InputActionReference controllerActionButton;
        private Canvas canvas;

        private HolowatchPage currentPage = HolowatchPage.None;
        [SerializeField] private GameObject objectivePage;
        [SerializeField] private GameObject messagingPage;
        [SerializeField] private GameObject conversationPage;

        private StoryData currentStory;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void Start() {
            canvas = GetComponent<Canvas>();
            canvas.enabled = false;
            SwitchPage(HolowatchPage.Conversation);
            SwitchPage(HolowatchPage.Messaging);
            SwitchPage(HolowatchPage.Objective);
        }

        private void OnEnable() {
            controllerActionButton.action.performed += ToggleHoloWatch;
        }

        private void OnDisable() {
            controllerActionButton.action.performed -= ToggleHoloWatch;
        }

        private void ToggleHoloWatch(InputAction.CallbackContext obj) {
            canvas.enabled = !canvas.enabled;
        }

        public void SwitchObjectivePage() {
            SwitchPage(HolowatchPage.Objective);
        }

        public void SwitchMessagingPage() {
            SwitchPage(HolowatchPage.Messaging);
        }

        public void SwitchPage(HolowatchPage page) {
            currentPage = page;
            objectivePage.SetActive(false);
            messagingPage.SetActive(false);
            conversationPage.SetActive(false);
            switch (page) {
                case HolowatchPage.Objective:
                    objectivePage.SetActive(true);
                    break;
                case HolowatchPage.Messaging:
                    messagingPage.SetActive(true);
                    break;
                case HolowatchPage.Conversation:
                    conversationPage.SetActive(true);
                    break;
                case HolowatchPage.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, null);
            }
        }

        public static void ClearChildren(Transform parent) {
            for (int i = parent.childCount - 1; i >= 0; i--) {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        public HolowatchPage GetCurrentPage() {
            return canvas.enabled ? currentPage : HolowatchPage.None;
        }

        public bool ThisPageIsActive(HolowatchPage page) {
            return canvas.enabled && currentPage == page;
        }

        public void InitStory(StoryData story) {
            if (!CheckStory(story)) return;

            currentStory = story;
            StartObjective(story.mainObjective.objectiveId, true);
            if (story.objectives.Count > 0) {
                StartObjective(story.objectives[0].objectiveId);
            }
        }

        private bool CheckStory(StoryData story) {
            HashSet<string> ids = new HashSet<string>();
            if (string.IsNullOrEmpty(story.mainObjective?.objectiveId)) {
                Debug.LogError("CheckStory: mainObjective manquant ou objectiveId vide.");
                return false;
            }
            ids.Add(story.mainObjective.objectiveId);

            if (story.objectives == null) {
                Debug.LogWarning("CheckStory: la liste d'objectives est null.");
                return false;
            }

            foreach (ObjectiveData o in story.objectives) {
                if (o == null || string.IsNullOrEmpty(o.objectiveId)) {
                    Debug.LogError("CheckStory: un objectif est null ou a un objectiveId vide.");
                    return false;
                }
                if (!ids.Add(o.objectiveId)) {
                    Debug.LogError($"CheckStory: un objectiveId est présent deux fois dans la même histoire: {o.objectiveId}");
                    return false;
                }
            }
            return true;
        }

        public void StartObjective(string objectiveId, bool isMainObjective = false) {
            ObjectiveData obj = currentStory.objectives.Find(o => o.objectiveId == objectiveId);
            if (!obj) {
                if (isMainObjective && currentStory.mainObjective.objectiveId == objectiveId) {
                    obj = currentStory.mainObjective;
                } else {
                    Debug.LogWarning($"StartObjective: L'objective {objectiveId} n'existe pas dans l'histoire en cours.");
                    return;
                }
            }

            ObjectiveManager.Instance.AddObjective(obj, isMainObjective);
        }

        public void CompleteObjective(string objectiveId) {
            ObjectiveManager.Instance.CompleteObjective(objectiveId);
        }

        public void AddMessage(string objectiveId, string messageId) {
            List<ChatMessage> listMessage;

            if (currentStory.mainObjective.objectiveId == objectiveId) {
                listMessage = currentStory.mainObjective.conversation;
            } else {
                listMessage = currentStory.objectives.Find(o => o.objectiveId == objectiveId)?.conversation;
            }

            if (listMessage != null) {
                ChatMessage chatMessage = listMessage.Find(m => m.messageId == messageId);
                if (chatMessage != null) {
                    MessagingManager.Instance.AddMessage(objectiveId, chatMessage);
                } else {
                    Debug.LogWarning($"AddMessage: Le message {messageId} n'existe pas pour l'objective {objectiveId}.");
                }
            } else {
                Debug.LogWarning($"AddMessage: L'objective {objectiveId} n'existe pas dans l'histoire en cours.");
            }
        }

        public void AskForHelp() {
            string objectiveId = MessagingManager.Instance.GetCurrentConversation();
            ObjectiveData obj = currentStory.objectives.Find(o => o.objectiveId == objectiveId);
            if (obj != null && MessagingManager.Instance.TryGetNextHint(objectiveId, out string hint)) {
                ChatMessage helpMe = new ChatMessage {
                    sender = MessageSender.Player,
                    content = "Peux-tu m'aider ?" //DEBUG!!! écrire plusieurs messages
                };
                ChatMessage helpMessage = obj.conversation.Find(o => o.messageId == hint);

                MessagingManager.Instance.AddMessage(objectiveId, helpMe);
                MessagingManager.Instance.AddMessage(objectiveId, helpMessage);
            } else {
                if (obj != null) {
                    Debug.LogWarning($"AskForHelp: L'objective {objectiveId} n'existe pas");
                } else {
                    Debug.LogWarning($"AskForHelp: L'objective {objectiveId} n'a pas ou plus de messages d'aide.");
                }
            }
        }

        public void SetNextHints(string objectiveId, List<string> hints) {
            MessagingManager.Instance.SetNextHints(objectiveId, hints);
        }
    }
}
