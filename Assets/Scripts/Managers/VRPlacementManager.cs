using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static GameManager;

public class PlacementManager : MonoBehaviour {
    public enum HandSide {
        Left,
        Right
    }

    [Serializable]
    public class PlacementPoint {
        [SerializeField] public Transform Transform;
        [SerializeField] public List<Collider> Colliders;
        [SerializeField] public IPlacementAction PlacementAction;
    }

    [SerializeField] private List<PlacementPoint> placementPoints; // Liste des points de placement.
    [SerializeField] private Transform parentParts;
    [SerializeField] private NearFarInteractor interactorRight;
    [SerializeField] private NearFarInteractor interactorLeft;

    [SerializeField] private Material transparentMaterial;

    [SerializeField] public bool showOverlay = true;

    private class HandState {
        public XRGrabInteractable HeldObject;
        public GameObject CurrentOverlay;
        public GameObject GOReplaced;
    }

    private readonly Dictionary<HandSide, HandState> handStates = new();

    private void Awake() {
        // Initialisation des états des mains
        handStates[HandSide.Right] = new HandState();
        handStates[HandSide.Left] = new HandState();
    }

    bool alreadyInit = false;

    private void OnEnable() {
        if (!alreadyInit) {
            alreadyInit = true;
            StartCoroutine(InitializeEvents());
        }
    }

    private IEnumerator InitializeEvents() {
        // Attendez que la création des colliders soit terminée
        yield return new WaitForEndOfFrame();

        foreach (PlacementPoint placementPoint in placementPoints) {
            List<Collider> colliders = placementPoint.Colliders;
            if (colliders == null || colliders.Count == 0) {
                colliders = new List<Collider>();
                foreach (Collider collider in placementPoint.Transform.GetComponentsInChildren<Collider>()) {
                    colliders.Add(collider);
                }
                placementPoint.Colliders = colliders;
            }
            if (colliders == null || colliders.Count == 0) {
                Debug.LogWarning("Un PlacementPoint ne contient pas de Collider valide !");
                continue;
            }

            //TODO (DONE) : Ajouter automatiquement le component UnifiedColliderEventManager lors de la création du CylinderCollider
            //UnifiedColliderEventManager triggerZone = placementPoint.Transform.gameObject.AddComponent<UnifiedColliderEventManager>();
            //triggerZone.SetColliders(colliders);
            if (placementPoint.Transform.gameObject.TryGetComponent<UnifiedColliderEventManager>(out UnifiedColliderEventManager triggerZone)) {
            } else {
                triggerZone = placementPoint.Transform.gameObject.AddComponent<UnifiedColliderEventManager>();
                triggerZone.SetColliders(colliders);
            }
            triggerZone.OnTriggerEnterEvent += (collider, selfGO) => HandleTriggerEnter(collider, selfGO, HandSide.Right);
            triggerZone.OnTriggerExitEvent += (collider, selfGO) => HandleTriggerExit(collider, selfGO, HandSide.Right);

            triggerZone.OnTriggerEnterEvent += (collider, selfGO) => HandleTriggerEnter(collider, selfGO, HandSide.Left);
            triggerZone.OnTriggerExitEvent += (collider, selfGO) => HandleTriggerExit(collider, selfGO, HandSide.Left);

            if (placementPoint.PlacementAction == null) {
                placementPoint.PlacementAction = placementPoint.Transform.GetComponent<IPlacementAction>();
            }
        }

        interactorRight.selectEntered.AddListener(args => HandleGrab(args, HandSide.Right));
        interactorLeft.selectEntered.AddListener(args => HandleGrab(args, HandSide.Left));
        interactorRight.selectExited.AddListener(args => HandleDrop(HandSide.Right));
        interactorLeft.selectExited.AddListener(args => HandleDrop(HandSide.Left));
    }

    private void HandleTriggerEnter(Collider collider, GameObject selfGO, HandSide hand) {
        var handState = handStates[hand];
        if ((handState.HeldObject != null) && (handState.CurrentOverlay == null) && handState.HeldObject.TryGetComponent<IPlacementAction>(out var _)) {
            if (selfGO.transform.parent == handState.HeldObject.transform) return;
            handState.GOReplaced = selfGO;
            handState.CurrentOverlay = CreateOverlay(handState.HeldObject.gameObject, selfGO.transform, hand);
        }
    }

    private void HandleTriggerExit(Collider collider, GameObject selfGO, HandSide hand) {
        var handState = handStates[hand];
        if (handState.CurrentOverlay != null) {
            Destroy(handState.CurrentOverlay);
            handState.CurrentOverlay = null;
        }
    }

    private void HandleGrab(SelectEnterEventArgs args, HandSide hand) {
        HandState handState = handStates[hand];
        if (args.interactableObject is XRGrabInteractable grabObject) {
            handState.HeldObject = grabObject;
            SavePlacement("GRAB_OBJECT", handState.HeldObject.name);
        }
    }

    private void HandleDrop(HandSide hand) {
        var handState = handStates[hand];
        if (handState.HeldObject != null && handState.CurrentOverlay != null) {
            string isCorrect = "INCORRECT";
            Destroy(handState.CurrentOverlay);
            if (TextTool.CompareTexts(handState.HeldObject.name, handState.GOReplaced.name)) {
                handState.GOReplaced.SetActive(false);
                FinalizePlacement(handState.HeldObject, handState.CurrentOverlay);
                isCorrect = "CORRECT";
                SavePlacement("PLACE_OBJECT", $"{handState.HeldObject.name} ({isCorrect})");
                //TODO : Tout ce qui suit est à déplacer dans le GameManager et IPlacementAction (cela n'a pas été fait par manque de temps)
                if (GameManager.Instance.mainStateMachine.GetCurrentSubState().ToString() == Part2State.NettoyerZone.ToString()) {
                    handState.HeldObject.gameObject.SetActive(false);
                }
                if (GameManager.Instance.mainStateMachine.GetCurrentSubState().ToString() == Part2State.CapteurDnsApplicateur.ToString()) {
                    handState.HeldObject.gameObject.SetActive(false);
                    if (hand == HandSide.Left) {
                        Destroy(handStates[HandSide.Right].CurrentOverlay);
                        handStates[HandSide.Right].CurrentOverlay = null;
                    }
                    if (hand == HandSide.Right) {
                        Destroy(handStates[HandSide.Left].CurrentOverlay);
                        handStates[HandSide.Left].CurrentOverlay = null;
                    }
                    handState.GOReplaced.transform.parent.gameObject.SetActive(false);
                    ForceDropObjects();
                    handState.GOReplaced.transform.parent.name = "applicateur_et_analyseur_glycemie";
                    if (handState.GOReplaced.transform.parent.TryGetComponent<ShowAnalyseurGlycemie>(out ShowAnalyseurGlycemie sag)) sag.Show();
                    handState.GOReplaced.transform.parent.gameObject.SetActive(true);
                }
                if (GameManager.Instance.mainStateMachine.GetCurrentSubState().ToString() == Part2State.PlacerApplicateur.ToString()) {
                    ForceDropObjects();
                    handState.HeldObject.name = "applicateur";
                    if (handState.HeldObject.TryGetComponent<ShowAnalyseurGlycemie>(out ShowAnalyseurGlycemie sag)) sag.Hide();
                    if (handState.GOReplaced.TryGetComponent<TeleportObject>(out TeleportObject to)) to.Teleport();
                }
                if (GameManager.Instance.mainStateMachine.GetCurrentSubState().ToString() == Part2State.ConnexionCapteur.ToString()) {
                    handState.HeldObject.gameObject.SetActive(false);
                }
                GameManager.Instance.step1CheckList.CheckID(TextTool.ExtractText(handState.HeldObject.name));
                GameManager.Instance.dialogueManagerStep1.ShowDialogue();
                if (GameManager.Instance.step1CheckList.IsAllChecked()) GameManager.Instance.HandleNextState();
                //Fin TODO
            } else {
                SavePlacement("PLACE_OBJECT", $"{handState.HeldObject.name} ({isCorrect})");
            }
        } else if (handState.HeldObject != null) {
            SavePlacement("DROP_OBJECT", handState.HeldObject.name);
        }
        handState.HeldObject = null;
        handState.CurrentOverlay = null;
        handState.GOReplaced = null;
    }
    internal void ForceDropObjects() {
        if (interactorRight.hasSelection) {
            var interactable = interactorRight.firstInteractableSelected;
            if (interactable != null) {
                interactorRight.EndManualInteraction();
            }
        }
        if (interactorLeft.hasSelection) {
            var interactable = interactorLeft.firstInteractableSelected;
            if (interactable != null) {
                interactorLeft.EndManualInteraction();
            }
        }
    }

    private GameObject CreateOverlay(GameObject heldObject, Transform placementPoint, HandSide hand) {
        GameObject overlay = Instantiate(
            heldObject,
            placementPoint.position,
            placementPoint.rotation,
            parentParts
        );

        overlay.transform.localScale = placementPoint.localScale;

        overlay.name = $"{heldObject.name} (overlay {hand})";

        if (overlay.TryGetComponent<XRGrabInteractable>(out XRGrabInteractable xrgi)) Destroy(xrgi);
        if (overlay.TryGetComponent<Rigidbody>(out Rigidbody rb)) Destroy(rb);
        if (overlay.TryGetComponent<Collider>(out Collider clldr)) Destroy(clldr);
        foreach (Transform child in overlay.transform) {
            if (!child.TryGetComponent<MeshRenderer>(out MeshRenderer _)) Destroy(child.gameObject);
        }

        if (showOverlay)
            ApplyTransparentMaterial(overlay);
        else
            foreach (Renderer renderer in overlay.GetComponentsInChildren<Renderer>()) {
                renderer.enabled = false;
            }
        return overlay;
    }

    private void FinalizePlacement(XRGrabInteractable heldObject, GameObject overlay) {
        if (heldObject.TryGetComponent<IPlacementAction>(out IPlacementAction action)) action.Execute(heldObject.gameObject, overlay, parentParts);
        else Debug.Log("Aucune Action implémenté!");
    }

    private void ApplyTransparentMaterial(GameObject obj) {
        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>()) {
            renderer.material = transparentMaterial;
        }
    }

    // Save Datas : "Name;OldState;NewState;TimeStamp;TimeStampSinceStart;Infos"
    public void SavePlacement(string type, string objectName) {
        Enum subState = GameManager.Instance.mainStateMachine.GetCurrentSubState();
        string subStepName;
        if (subState != null) {
            subStepName = subState.ToString();
        } else {
            subStepName = "";
        }
        string id = $"SUBSTEP_{GameManager.Instance.mainStateMachine.GetCurrentSubState()}";
        string timeStamp1 = GameManager.Instance.datas.CalculateSinceEntry(id).ToString();
        string timeStamp2 = GameManager.Instance.datas.CalculateTotalGhostTime(id).ToString();
        GameManager.Instance.datas.AddData(DateTime.Now.ToString("yyyyMMddHHmmssfff"),
            type, $"{subStepName};;{timeStamp1};{timeStamp2};{objectName}");
    }
}
