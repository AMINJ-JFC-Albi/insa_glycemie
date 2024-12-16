using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(SciFiDoor))]
public class SciFiDoorAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SciFiDoor doorScript = (SciFiDoor)target;

        if (GUILayout.Button("Trigger Open"))
        {
            doorScript.TriggerOpen();
        }

        if (GUILayout.Button("Trigger Close"))
        {
            doorScript.TriggerClose();
        }
    }
}
#endif

public class SciFiDoor : MonoBehaviour
{
    public Transform doorLeft;
    public Transform doorRight;
    public Transform padlock;  // Ajout du padlock (référence Transform)
    public float openSpeed = 2f;
    public float openDistance = 0.012f;
    public float padlockRotationSpeed = 100f; // Vitesse de rotation du padlock (degrés par seconde)
    public Quaternion rotationPadlock = Quaternion.Euler(0, 0, -90); // Rotation de +90 degrés pour le padlock

    private Vector3 leftDoorStartPosition;
    private Vector3 rightDoorStartPosition;
    private Quaternion padlockStartRotation;

    private bool isOpening = false;
    private bool isClosing = false;
    private bool isRotatingPadlock = false;
    private bool isPadlockRotated = false; // Flag pour savoir si le padlock est déjà tourné

    void Start()
    {
        leftDoorStartPosition = doorLeft.localPosition;
        rightDoorStartPosition = doorRight.localPosition;
        padlockStartRotation = padlock.rotation; // Enregistrer la rotation initiale du padlock
    }

    void Update()
    {
        // Si on est en train de faire tourner le padlock, on l'anime
        if (isRotatingPadlock)
        {
            RotatePadlock();
        }
        // Si la porte est en train de s'ouvrir
        else if (isOpening && !isClosing)
        {
            OpenDoors();
        }
        // Si la porte est en train de se fermer
        else if (!isOpening && isClosing)
        {
            CloseDoors();
        }
    }

    public void OpenDoors()
    {
        doorLeft.localPosition = Vector3.Lerp(doorLeft.localPosition, leftDoorStartPosition - new Vector3(0, openDistance, 0), Time.deltaTime * openSpeed);
        doorRight.localPosition = Vector3.Lerp(doorRight.localPosition, rightDoorStartPosition + new Vector3(0, openDistance, 0), Time.deltaTime * openSpeed);

        if (Vector3.Distance(doorLeft.localPosition, leftDoorStartPosition - new Vector3(0, openDistance, 0)) < 0.0001f &&
            Vector3.Distance(doorRight.localPosition, rightDoorStartPosition + new Vector3(0, openDistance, 0)) < 0.0001f)
        {
            doorLeft.localPosition = leftDoorStartPosition - new Vector3(0, openDistance, 0);
            doorRight.localPosition = rightDoorStartPosition + new Vector3(0, openDistance, 0);
            isOpening = false;
        }
    }

    public void CloseDoors()
    {
        doorLeft.localPosition = Vector3.Lerp(doorLeft.localPosition, leftDoorStartPosition, Time.deltaTime * openSpeed);
        doorRight.localPosition = Vector3.Lerp(doorRight.localPosition, rightDoorStartPosition, Time.deltaTime * openSpeed);

        if (Vector3.Distance(doorLeft.localPosition, leftDoorStartPosition) < 0.0001f &&
            Vector3.Distance(doorRight.localPosition, rightDoorStartPosition) < 0.0001f)
        {
            doorLeft.localPosition = leftDoorStartPosition;
            doorRight.localPosition = rightDoorStartPosition;
            isRotatingPadlock = true;
        }
    }

    public void TriggerOpen()
    {
        isRotatingPadlock = true;
        isOpening = true;
        isClosing = false;
    }

    public void TriggerClose()
    {
        isOpening = false;
        isClosing = true;
        isRotatingPadlock = false;
    }


    private void RotatePadlock()
    {
        // Si on ouvre les portes, on effectue la rotation vers +90°
        if (isOpening)
        {
            padlock.rotation = Quaternion.RotateTowards(padlock.rotation, padlockStartRotation * rotationPadlock, padlockRotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(padlock.rotation, padlockStartRotation * rotationPadlock) < 1f)
            {
                isRotatingPadlock = false; // Arrêter la rotation
                padlock.rotation = padlockStartRotation * rotationPadlock; // Bloquer la position du padlock
                isPadlockRotated = true; // Marquer le padlock comme tourné
            }
        }
        // Si on ferme les portes, on fait tourner le padlock vers sa position de départ (0°)
        else if (isClosing)
        {
            padlock.rotation = Quaternion.RotateTowards(padlock.rotation, padlockStartRotation, padlockRotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(padlock.rotation, padlockStartRotation) < 1f)
            {
                isRotatingPadlock = false; // Arrêter la rotation
                padlock.rotation = padlockStartRotation; // Bloquer la position du padlock à son état initial
                isPadlockRotated = false; // Marquer le padlock comme revenu à son état initial
                isClosing = false;
            }
        }
    }
}
