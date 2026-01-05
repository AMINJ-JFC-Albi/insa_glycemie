using System.Collections.Generic;
using UnityEngine;

namespace HoloWatch {
    [CreateAssetMenu(menuName = "Holowatch/Objectif", order = 1)]
    public class ObjectiveData : ScriptableObject {
        [Header("Informations")]
        public string objectiveId;
        [TextArea(1, 3)] public string title;
        
        [Header("Conversations")]
        public string conversationName = "???";
        public Sprite conversationAvatar;
        public List<ChatMessage> conversation;
    }
}
