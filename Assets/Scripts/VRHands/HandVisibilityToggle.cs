//Permet cacher/d'afficher le modèle de la main lorsque l'on attrape/lâche un objet

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HandVisibilityToggle : MonoBehaviour {
    [SerializeField] private NearFarInteractor handInteractor;
    private SkinnedMeshRenderer handModel;

    private void Start() {
        handModel = GetComponentInChildren<SkinnedMeshRenderer>();
        handInteractor.selectEntered.AddListener(OnGrab);
        handInteractor.selectExited.AddListener(OnLetGo);
    }

    private void OnLetGo(SelectExitEventArgs arg0) {
        handModel.enabled = true;
    }

    private void OnGrab(SelectEnterEventArgs arg0) {
        handModel.enabled = false;
    }
}
