using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PaintWetSimple : MonoBehaviour
{
    [Header("Setup")]
    public Material matSec;
    public Material matHumide;
    public float targetWetness = 1f;
    public float wetSpeed = 0.5f;
    public float dryTime = 10f;

    private Renderer rend;
    private float wetness = 0f;
    private bool isComplete = false;
    private bool isDrying = false;
    private float dryTimer = 0f;

    public TaskManager taskManager;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (matSec != null)
            rend.material = matSec;
    }

    void Update()
    {
        if (isDrying)
        {
            dryTimer += Time.deltaTime;
            wetness = Mathf.Lerp(1f, 0f, dryTimer / dryTime);

            if (matSec != null && matHumide != null)
                rend.material.Lerp(matSec, matHumide, wetness);

            if (dryTimer >= dryTime)
            {
                wetness = 0f;
                isDrying = false;
                isComplete = false;
                rend.material = matSec;
                dryTimer = 0f;

                taskManager?.NextStep(); // étape suivante après séchage
            }
        }
    }

    // handInteractor : l'interactor qui tient la lingette pour haptics
    public void AddWetness(float delta, IXRSelectInteractor handInteractor)
    {
        if (isComplete || isDrying) return;

        wetness = Mathf.Clamp01(wetness + delta * wetSpeed);

        if (matSec != null && matHumide != null)
            rend.material.Lerp(matSec, matHumide, wetness);

        if (wetness >= targetWetness)
        {
            isComplete = true;

            // Haptics sur la main qui tient la lingette
            if (handInteractor != null)
            {
                var controllerInteractor = handInteractor as XRBaseInputInteractor;
                controllerInteractor?.SendHapticImpulse(0.5f, 0.2f);
            }

            // Démarrer le séchage
            isDrying = true;
            dryTimer = 0f;
        }
    }
}
