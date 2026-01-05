using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HoloWatch {
    public class MessageInfo : MonoBehaviour {
        [SerializeField] private TMP_Text masterText;
        [SerializeField] private TMP_Text visualText;
        [SerializeField] private LayoutElement paddingRight;
        [SerializeField] private LayoutElement paddingLeft;
        
        public void Initialize(string message, bool isRight = false) {
            masterText.text = message;
            visualText.text = message;
            
            paddingLeft.minWidth = isRight ? 20 : 0;
            paddingRight.minWidth = !isRight ? 20 : 0;
        }
    }
}
