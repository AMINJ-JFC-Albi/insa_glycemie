using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tools;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PlacementManager : MonoBehaviour
{
    public enum HandSide
    {
        Left,
        Right
    }

    [Serializable]
    public class PlacementPoint
    {
        [SerializeField]
        public Transform Transform;
        [SerializeField]
        public List<Collider> Colliders;
        [SerializeField]
        public IPlacementAction PlacementAction;
    }

    [SerializeField] private List<PlacementPoint> placementPoints; // Liste des points de placement.
    [SerializeField] private Transform parentParts;
    [SerializeField] private NearFarInteractor interactorRight;
    [SerializeField] private NearFarInteractor interactorLeft;

    [SerializeField] private Material transparentMaterial;

    [SerializeField]
    public bool showOverlay = true;

    private class HandState
    {
        public XRGrabInteractable HeldObject;
        public GameObject CurrentOverlay;
        public GameObject GOReplaced;
    }

    private readonly Dictionary<HandSide, HandState> handStates = new();

    private void Awake()
    {
        // Initialisation des états des mains
        handStates[HandSide.Right] = new HandState();
        handStates[HandSide.Left] = new HandState();
    }

    bool alreadyInit = false;

    private void OnEnable()
    {
        if (!alreadyInit)
        {
            alreadyInit = true;
            StartCoroutine(InitializeEvents());
        }
    }

    private IEnumerator InitializeEvents()
    {
        // Attendez que la création des colliders soit terminée
        yield return new WaitForEndOfFrame();

        foreach (PlacementPoint placementPoint in placementPoints)
        {
            List<Collider> colliders = placementPoint.Colliders;
            if (colliders == null || colliders.Count == 0)
            {
                colliders = new List<Collider>();
                foreach (Collider collider in placementPoint.Transform.GetComponentsInChildren<Collider>()) { colliders.Add(collider); }
                placementPoint.Colliders = colliders;
            }
            if (colliders == null || colliders.Count == 0)
            {
                Debug.LogWarning("Un PlacementPoint ne contient pas de Collider valide !");
                continue;
            }

            //TODO (DONE) : Ajouter automatiquement le component UnifiedColliderEventManager lors de la création du CylinderCollider
            //UnifiedColliderEventManager triggerZone = placementPoint.Transform.gameObject.AddComponent<UnifiedColliderEventManager>();
            //triggerZone.SetColliders(colliders);
            if (placementPoint.Transform.gameObject.TryGetComponent<UnifiedColliderEventManager>(out UnifiedColliderEventManager triggerZone)) {}
            else
            {
                triggerZone = placementPoint.Transform.gameObject.AddComponent<UnifiedColliderEventManager>();
                triggerZone.SetColliders(colliders);
            }
            triggerZone.OnTriggerEnterEvent += (collider, selfGO) => HandleTriggerEnter(collider, selfGO, HandSide.Right);
            triggerZone.OnTriggerExitEvent += (collider, selfGO) => HandleTriggerExit(collider, selfGO, HandSide.Right);

            triggerZone.OnTriggerEnterEvent += (collider, selfGO) => HandleTriggerEnter(collider, selfGO, HandSide.Left);
            triggerZone.OnTriggerExitEvent += (collider, selfGO) => HandleTriggerExit(collider, selfGO, HandSide.Left);

            if (placementPoint.PlacementAction == null)
            {
                placementPoint.PlacementAction = placementPoint.Transform.GetComponent<IPlacementAction>();
            }
        }

        interactorRight.selectEntered.AddListener(args => HandleGrab(args, HandSide.Right));
        interactorLeft.selectEntered.AddListener(args => HandleGrab(args, HandSide.Left));
        interactorRight.selectExited.AddListener(args => HandleDrop(HandSide.Right));
        interactorLeft.selectExited.AddListener(args => HandleDrop(HandSide.Left));
    }

    private void HandleTriggerEnter(Collider collider, GameObject selfGO, HandSide hand)
    {
        LoggerTool.Log("TRIGGER ENTER");
        var handState = handStates[hand];
        if ((handState.HeldObject != null) && (handState.CurrentOverlay == null) && handState.HeldObject.TryGetComponent<IPlacementAction>(out var _))
        {
            LoggerTool.Log(selfGO.name, LoggerTool.Level.Warning);
            handState.GOReplaced = selfGO;
            handState.CurrentOverlay = CreateOverlay(handState.HeldObject.gameObject, selfGO.transform, hand);
        }
        else
        {
            LoggerTool.Log("ERREUR : " + selfGO.name, LoggerTool.Level.Warning);
        }
    }

    private void HandleTriggerExit(Collider collider, GameObject selfGO, HandSide hand)
    {
        var handState = handStates[hand];
        if (handState.CurrentOverlay != null)
        {
            Destroy(handState.CurrentOverlay);
            handState.CurrentOverlay = null;
        }
    }

    private void HandleGrab(SelectEnterEventArgs args, HandSide hand)
    {
        HandState handState = handStates[hand];
        if (args.interactableObject is XRGrabInteractable grabObject)
        {
            handState.HeldObject = grabObject;
            SavePlacement("GRAB_OBJECT", handState.HeldObject.name);
        }
    }

    private void HandleDrop(HandSide hand)
    {
        var handState = handStates[hand];
        if (handState.HeldObject != null && handState.CurrentOverlay != null)
        {
            string isCorrect = "INCORRECT";
            Destroy(handState.CurrentOverlay);
            if (TextTool.CompareTexts(handState.HeldObject.name, handState.GOReplaced.name))
            {
                handState.GOReplaced.SetActive(false);
                FinalizePlacement(handState.HeldObject, handState.CurrentOverlay);
                isCorrect = "CORRECT";
                SavePlacement("PLACE_OBJECT", $"{handState.HeldObject.name} ({isCorrect})");
                GameManager.Instance.step1CheckList.CheckID(TextTool.ExtractText(handState.HeldObject.name));
                GameManager.Instance.dialogueSystemStep1.ShowDialogue();
                if (GameManager.Instance.step1CheckList.IsAllChecked()) GameManager.Instance.HandleNextState();
            }
            else
            {
                SavePlacement("PLACE_OBJECT", $"{handState.HeldObject.name} ({isCorrect})");
            }
        }
        else if(handState.HeldObject != null)
        {
            SavePlacement("DROP_OBJECT", handState.HeldObject.name);
        }
        handState.HeldObject = null;
        handState.CurrentOverlay = null;
        handState.GOReplaced = null;
    }

    private GameObject CreateOverlay(GameObject heldObject, Transform placementPoint, HandSide hand)
    {
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
        foreach (Transform child in overlay.transform)
        {
            if (!child.TryGetComponent<MeshRenderer>(out MeshRenderer _)) Destroy(child.gameObject);
        }

        if (showOverlay)
            ApplyTransparentMaterial(overlay);
        else
            foreach (Renderer renderer in overlay.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        return overlay;
    }

    private void FinalizePlacement(XRGrabInteractable heldObject, GameObject overlay)
    {
        if (heldObject.TryGetComponent<IPlacementAction>(out IPlacementAction action)) action.Execute(heldObject.gameObject, overlay, parentParts);
        else Debug.Log("Aucune Action implémenté!");
    }

    private void ApplyTransparentMaterial(GameObject obj)
    {
        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
        {
            renderer.material = transparentMaterial;
        }
    }

    // Save Datas : "Name;OldState;NewState;TimeStamp;TimeStampSinceStart;Infos"
    public void SavePlacement(string type, string objectName)
    {
        Enum subState = GameManager.Instance.mainStateMachine.GetCurrentSubState();
        string subStepName;
        if (subState != null)
        {
            subStepName = subState.ToString();
        }
        else
        {
            subStepName = "";
        }
        string id = $"SUBSTEP_{GameManager.Instance.mainStateMachine.GetCurrentSubState()}";
        string timeStamp1 = GameManager.Instance.datas.CalculateSinceEntry(id).ToString();
        string timeStamp2 = GameManager.Instance.datas.CalculateTotalGhostTime(id).ToString();
        GameManager.Instance.datas.AddData(DateTime.Now.ToString("yyyyMMddHHmmssfff"), 
                                           type, $"{subStepName};;{timeStamp1};{timeStamp2};{objectName}");
    }
}