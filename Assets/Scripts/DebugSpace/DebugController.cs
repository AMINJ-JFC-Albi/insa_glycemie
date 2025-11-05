using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

namespace DebugSpace {
    public class DebugController : MonoBehaviour {
        void Start() {

            // Détecte si la scène est ouverte additivement ou non
            if (gameObject.scene != SceneManager.GetActiveScene()) {
                // Si c'est le cas, on n'a pas besoin d'un autre XR Origin
                Destroy(gameObject);
            } else {
                // Sinon c'est obligatoirement ouvert via l'éditeur dans l'optique de tester quelque chose
                
                // On recherche tous les XR Interaction Managers
                XRInteractionManager[] managers = FindObjectsByType<XRInteractionManager>(FindObjectsSortMode.None);
                foreach (XRInteractionManager manager in managers) {
                    // Pour trouver celui créer par Unity (car celui de notre XROrigin est désactivé de base)
                    if (!IsChildOf(manager.gameObject, gameObject)) {
                        Destroy(manager.gameObject);
                    }
                }
                
                // On active ensuite le XR Origin pour permettre le test
                if (transform.childCount == 2) {
                    transform.GetChild(1).gameObject.SetActive(true);
                    Destroy(transform.GetChild(0).gameObject);
                    
                }
            }
        }

        // Regarde si un GameObject est enfant d'un autre
        private bool IsChildOf(GameObject child, GameObject parent) {
            Transform current = child.transform;

            while (current != null) {
                if (current.gameObject == parent)
                    return true;
                current = current.parent;
            }
            return false;
        }
    }
}
