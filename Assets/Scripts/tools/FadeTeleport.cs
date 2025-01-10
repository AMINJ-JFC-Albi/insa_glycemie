using System;
using System.Collections;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Interactables;




#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(FadeTeleport))]
public class FadeTeleportEditor : Editor
{
    private Transform testPlayer;
    private Vector3 testTeleportPosition = Vector3.zero;
    private Quaternion testTeleportRotation = Quaternion.identity;
    private float testFadeDuration = 1.0f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Test Fade and Teleport", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        testPlayer = (Transform)EditorGUILayout.ObjectField("Player", testPlayer, typeof(Transform), true);

        testTeleportPosition = EditorGUILayout.Vector3Field("Teleport Position", testTeleportPosition);

        Vector3 rotationEuler = testTeleportRotation.eulerAngles;
        rotationEuler = EditorGUILayout.Vector3Field("Teleport Rotation (Euler)", rotationEuler);
        testTeleportRotation = Quaternion.Euler(rotationEuler);

        testFadeDuration = EditorGUILayout.FloatField("Fade Duration (s)", testFadeDuration);

        EditorGUILayout.Space();

        if (GUILayout.Button("Test Fade and Teleport"))
        {
            if (testPlayer != null)
            {
                FadeTeleport fadeTeleportScript = (FadeTeleport)target;

                fadeTeleportScript.StartTeleportSequence(testPlayer, testTeleportPosition, testTeleportRotation, testFadeDuration, () =>
                {
                    Debug.Log("Custom function executed during teleportation!");
                });

                Debug.Log("Fade and Teleport sequence initiated.");
            }
            else
            {
                EditorGUILayout.HelpBox("Veuillez assigner un Transform au champ 'Player' pour tester.", MessageType.Warning);
            }
        }

        if (GUILayout.Button("Test Fade"))
        {
            if (testPlayer != null)
            {
                FadeTeleport fadeTeleportScript = (FadeTeleport)target;

                fadeTeleportScript.StartTeleportSequence(testPlayer, testFadeDuration, () =>
                {
                    Debug.Log("Custom function executed during teleportation!");
                });

                Debug.Log("Fade sequence initiated.");
            }
            else
            {
                EditorGUILayout.HelpBox("Veuillez assigner un Transform au champ 'Player' pour tester.", MessageType.Warning);
            }
        }
    }
}
#endif

public class FadeTeleport : MonoBehaviour
{
    [SerializeField] private GameObject fadeQuadPrefab;
    [SerializeField] private Transform teleportTransform;
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private Vector3 quadOffset = new(0, 0, 0.5f);
    [SerializeField] private GameObject[] objectsToDisactive;
    [SerializeField] private GameObject[] objectsToActive;

    private Material fadeMaterial;
    private GameObject fadeQuadInstance;
    private bool oneTime = true;

    public void StartVRTeleportSequence(SelectEnterEventArgs args)
    {
        if (oneTime)
        {
            oneTime = false;
            if (TryGetComponent<XRSimpleInteractable>(out XRSimpleInteractable xrsi)) xrsi.enabled = false;

            Transform player = args.interactorObject.transform.parent.parent.parent;

            StartTeleportSequence(player, teleportTransform.position, teleportTransform.rotation, 1.0f, () =>
            {
                // Réinitialiser du déplacement de l'XROrigin
                if (player.TryGetComponent<XROrigin>(out XROrigin xrOrigin)) xrOrigin.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                if (objectsToActive != null) foreach (GameObject go in objectsToActive) go.SetActive(true);
                if (objectsToDisactive != null) foreach (GameObject go in objectsToDisactive)
                    {
                        if (go.TryGetComponent<FadeTeleport>(out var _))
                        {
                            if (go.TryGetComponent<MeshRenderer>(out MeshRenderer mr)) mr.enabled = false;
                            if (go.TryGetComponent<MeshCollider>(out MeshCollider mc)) mc.enabled = false;
                        }
                        else go.SetActive(false);
                    }

                GameManager.Instance.HandleNextState();
            });
        }
    }

    public void StartTeleportSequence(Transform player, float fadeDuration, Action customAction = null)
    {
        StartCoroutine(FadeAndTeleport(player, player.localPosition, player.localRotation, fadeDuration, customAction));
    }

    public void StartTeleportSequence(Transform player, Vector3 teleportPosition, Quaternion teleportRotation, float fadeDuration, Action customAction = null)
    {
        StartCoroutine(FadeAndTeleport(player, teleportPosition, teleportRotation, fadeDuration, customAction));
    }

    private IEnumerator FadeAndTeleport(Transform player, [AllowNull] Vector3 teleportPosition, Quaternion teleportRotation, float fadeDuration, Action customAction)
    {
        SetupFadeQuad(player);

        // 1. Fondu au noir
        yield return StartCoroutine(FadeToBlack(fadeDuration));

        // 2. Attendre 0.5 seconde
        yield return new WaitForSeconds(0.5f);

        // 3. Exécuter l'action personnalisée (si spécifiée)
        customAction?.Invoke();

        // 4. Téléportation du joueur
        player.position = teleportPosition;
        player.rotation = teleportRotation;

        // 5. Attendre 0.5 seconde
        yield return new WaitForSeconds(0.5f);

        // 6. Fondu au noir inversé
        yield return StartCoroutine(FadeFromBlack(fadeDuration));
    }

    private void SetupFadeQuad(Transform player)
    {
        if (fadeQuadInstance == null)
        {
            fadeQuadInstance = Instantiate(fadeQuadPrefab);
            fadeMaterial = fadeQuadInstance.GetComponent<MeshRenderer>().material;

            fadeMaterial.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        }
        Transform cameraTransform = player.GetComponentInChildren<Camera>().transform;
        fadeQuadInstance.transform.SetParent(cameraTransform);
        fadeQuadInstance.transform.localPosition = quadOffset;
        fadeQuadInstance.transform.localRotation = Quaternion.identity;
        fadeQuadInstance.transform.localScale = new Vector3(3, 3, 1);
    }

    private IEnumerator FadeToBlack(float fadeDuration)
    {
        Color color = fadeMaterial.color;
        float elapsed = 0f;
        fadeQuadInstance.SetActive(true);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeMaterial.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeMaterial.color = color;
    }

    private IEnumerator FadeFromBlack(float fadeDuration)
    {
        Color color = fadeMaterial.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (elapsed / fadeDuration));
            fadeMaterial.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeMaterial.color = color;
        fadeQuadInstance.SetActive(false);
    }
}
