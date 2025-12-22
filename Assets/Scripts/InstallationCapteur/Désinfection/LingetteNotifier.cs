using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LingetteNotifier : MonoBehaviour
{
    [Header("Setup")]
    public List<PaintWetSimple> paintables;
    public float distanceThreshold = 0.1f;

    private XRGrabInteractable grab;

    void Start()
    {
        grab = GetComponent<XRGrabInteractable>();
        if (grab == null) Debug.LogError("XRGrabInteractable manquant sur la lingette !");
    }

    void Update()
    {
        if (grab != null && grab.isSelected)
        {
            IXRSelectInteractor handInteractor = null;

            if (grab.interactorsSelecting.Count > 0)
            {
                handInteractor = grab.interactorsSelecting[0];
            }

            PaintWetSimple closest = null;
            float closestDist = float.MaxValue;

            foreach (var paintable in paintables)
            {
                float dist = Vector3.Distance(transform.position, paintable.transform.position);
                if (dist < distanceThreshold && dist < closestDist)
                {
                    closestDist = dist;
                    closest = paintable;
                }
            }

            if (closest != null)
            {
                // Passe l'interactor à PaintWetSimple pour haptics
                closest.AddWetness(Time.deltaTime, handInteractor);
            }
        }
    }
}
