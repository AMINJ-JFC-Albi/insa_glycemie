using System.Collections;
using UnityEngine;
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

                fadeTeleportScript.StartTeleportSequence(testPlayer, testTeleportPosition, testTeleportRotation, testFadeDuration);

                Debug.Log("Fade and Teleport sequence initiated.");
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
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private Vector3 quadOffset = new(0, 0, 0.5f);

    private Material fadeMaterial;
    private GameObject fadeQuadInstance;

    public void StartTeleportSequence(Transform player, Vector3 teleportPosition, Quaternion teleportRotation, float fadeDuration)
    {
        StartCoroutine(FadeAndTeleport(player, teleportPosition, teleportRotation, fadeDuration));
    }

    private IEnumerator FadeAndTeleport(Transform player, Vector3 teleportPosition, Quaternion teleportRotation, float fadeDuration)
    {
        SetupFadeQuad(player);

        // 1. Fondu au noir
        yield return StartCoroutine(FadeToBlack(fadeDuration));

        // 2. Attendre 0.5 seconde
        yield return new WaitForSeconds(0.5f);

        // 3. Téléportation du joueur
        player.position = teleportPosition;
        player.rotation = teleportRotation;

        // 4. Attendre 0.5 seconde
        yield return new WaitForSeconds(0.5f);

        // 5. Fondu au noir inversé
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
