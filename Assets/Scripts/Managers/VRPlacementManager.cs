using System;
using System.Collections;
using System.Collections.Generic;
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

    private void OnEnable()
    {
        StartCoroutine(InitializeEvents());
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


    private void OnDisable()
    {
        foreach (PlacementPoint placementPoint in placementPoints)
        {
            List<Collider> colliders = placementPoint.Colliders;
            if (colliders == null || colliders.Count == 0)
            {
                Debug.LogWarning("Un PlacementPoint ne contient pas de Collider valide !");
                continue;
            }
        }
    }

    private void HandleTriggerEnter(Collider collider, GameObject selfGO, HandSide hand)
    {
        var handState = handStates[hand];
        if (handState.HeldObject != null && handState.CurrentOverlay == null)
        {
            handState.GOReplaced = selfGO;
            handState.CurrentOverlay = CreateOverlay(handState.HeldObject.gameObject, selfGO.transform, hand);
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
        }
    }

    private void HandleDrop(HandSide hand)
    {
        var handState = handStates[hand];
        if (handState.HeldObject != null && handState.CurrentOverlay != null)
        {
            handState.GOReplaced.SetActive(false);
            FinalizePlacement(handState.HeldObject, handState.CurrentOverlay);
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

        ApplyTransparentMaterial(overlay);
        return overlay;
    }

    private void FinalizePlacement(XRGrabInteractable heldObject, GameObject overlay)
    {
        if (heldObject.TryGetComponent<IPlacementAction>(out IPlacementAction action)) action.Execute(heldObject.gameObject, overlay, parentParts);
        else Debug.Log("Aucune Action implémenté!");
        Destroy(overlay);
    }

    private void ApplyTransparentMaterial(GameObject obj)
    {
        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
        {
            renderer.material = transparentMaterial;
        }
    }
}