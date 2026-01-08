using UnityEngine;

namespace Decraft {
    public enum DecraftType {
        Base,
        Aiguille,
        Carte,
        Cache
    }
    
    public class DecraftObject : MonoBehaviour{
        public DecraftType decraftType;
    }
}
