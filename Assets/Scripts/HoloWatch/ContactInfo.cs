using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HoloWatch {
    public class ContactInfo : MonoBehaviour {
        [SerializeField] private TMP_Text contactNameText;
        [SerializeField] private TMP_Text lastMessageText;
        [SerializeField] private Image contactAvatarImage;
        
        public void Initialize(string contactName, Sprite contactAvatar) {
            contactNameText.text = contactName;
            contactAvatarImage.sprite = contactAvatar;
        }
        
        public void RefreshInfo(string message) {
            lastMessageText.text = message;
        }
        
        public string GetName() {
            return contactNameText.text;
        }
    }
}
