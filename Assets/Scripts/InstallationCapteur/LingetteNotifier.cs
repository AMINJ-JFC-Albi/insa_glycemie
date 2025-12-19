using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LingetteNotifier : MonoBehaviour
{
    [Header("Setup")]
    public List<PaintWetSimple> paintables; // Objets que la lingette peut humidifier
    public float distanceThreshold = 0.1f;  // Distance pour détecter le contact

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
            closest.AddWetness(Time.deltaTime);
        }
    }
}

}
