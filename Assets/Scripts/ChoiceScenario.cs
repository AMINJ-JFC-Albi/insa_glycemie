using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class VRButtonASceneLoader : MonoBehaviour
{
    public string sceneName;

    private InputDevice rightController;

    void Update()
    {
        // Récupère la manette droite si elle n'est pas valide
        if (rightController == null || !rightController.isValid)
        {
            rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            if (rightController.isValid)
                Debug.Log($"[DEBUG] Manette droite détectée : {rightController.name}");
        }

        if (rightController.isValid)
        {
            bool aPressed = false;

            // Vérifie le bouton A (primaryButton sur la main droite)
            if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out aPressed))
            {
                Debug.Log($"[DEBUG] Bouton A pressé ? {aPressed}");

                if (aPressed)
                {
                    if (!string.IsNullOrEmpty(sceneName))
                    {
                        Debug.Log($"[DEBUG] Chargement de la scène : {sceneName}");
                        SceneManager.LoadScene(sceneName);
                    }
                    else
                    {
                        Debug.LogWarning("[DEBUG] Aucun nom de scène défini !");
                    }
                }
            }
            else
            {
                Debug.Log("[DEBUG] Impossible de lire l'état du bouton A sur la manette droite.");
            }
        }
    }
}
