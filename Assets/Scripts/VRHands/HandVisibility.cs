//DEBUG!!! à utiliser dans certains cas ???

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace VrHands {
    // Affiche ou cache le modèle de la main en fonction de si l'utilisateur saisit un objet ou non.
    public class HandVisibility : MonoBehaviour {
        private NearFarInteractor handInteractor;
        private SkinnedMeshRenderer handModel;

        private void Start() {
            handInteractor = GetComponentInChildren<NearFarInteractor>();
            handInteractor.selectEntered.AddListener(OnGrab);
            handInteractor.selectExited.AddListener(OnLetGo);
            
            handModel = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        // Affiche le modèle de la main lorsqu'un objet est lâché.
        private void OnLetGo(SelectExitEventArgs arg0) {
            handModel.enabled = true;
        }

        // Cache le modèle de la main lorsqu'un objet est saisi.
        private void OnGrab(SelectEnterEventArgs arg0) {
            handModel.enabled = false;
        }
    }
}
