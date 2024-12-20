using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ExplodedView))]
public class ExplodedViewEditor : Editor
{
    private int partIndex = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ExplodedView explodedViewScript = (ExplodedView)target;

        // Options pour tester SelectPart et DeselectPart
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Test SelectPart", EditorStyles.boldLabel);
        partIndex = EditorGUILayout.IntField("Part Index", partIndex);

        if (GUILayout.Button("Check SelectPart"))
        {
            explodedViewScript.SelectPart(partIndex);
        }

        if (GUILayout.Button("Check DeselectPart"))
        {
            explodedViewScript.DeselectPart(partIndex);
        }
    }
}
#endif

public class ExplodedView : MonoBehaviour
{
    [System.Serializable]
    public class Part
    {
        public Transform partTransform; // Reference to the part's Transform
        public Vector3 finalPosition;  // Final position in exploded view
        public string description;     // Description of the part
        public Vector3 offset;         // Offset when selected

        public bool IsSelected { get; set; } = false;
    }

    public Part[] parts = new Part[4]; // Array to hold the 4 parts
    public bool IsExplodedView { get; private set; } = false;

    public float transitionSpeed = 2f; // Speed for smooth transitions

    private bool isToggling = false;
    private bool anyPartSelectedFlag = false; // Tracks selection state

    private void Update()
    {
        // Example of triggering the exploded view or reset with keys
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleExplodedView();
        }
    }

    public void ToggleExplodedView()
    {
        if (anyPartSelectedFlag)
        {
            Debug.Log("Cannot toggle exploded view while a part is selected.");
            return;
        }

        if (isToggling) return;

        IsExplodedView = !IsExplodedView;
        isToggling = true;
        StartCoroutine(MoveAllPartsSmoothly());
    }

    public void SelectPart(int partIndex)
    {
        if (!IsExplodedView)
        {
            Debug.LogError("Cannot select part when not in exploded view mode.");
            return;
        }

        if (partIndex < 0 || partIndex >= parts.Length)
        {
            Debug.LogError("Invalid part index.");
            return;
        }

        Part part = parts[partIndex];

        if (part.IsSelected)
        {
            Debug.LogError("Part already selected.");
            return;
        }

        if (anyPartSelectedFlag)
        {
            // If another part is selected, deselect it
            int i = 0;
            while (i < parts.Length)
            {
                if (parts[i].IsSelected)
                {
                    DeselectPart(i);
                    i = parts.Length;
                }
                i++;
            }
        }

        part.IsSelected = true;
        anyPartSelectedFlag = true;
        Debug.Log(part.description);

        Vector3 offsetPosition = part.partTransform.position + part.offset;
        StartCoroutine(MovePartSmoothly(part.partTransform, offsetPosition));
    }

    public void DeselectPart(int partIndex)
    {
        if (partIndex < 0 || partIndex >= parts.Length)
        {
            Debug.LogError("Invalid part index.");
            return;
        }

        Part part = parts[partIndex];

        if (!part.IsSelected)
        {
            Debug.LogError("Part not selected.");
            return;
        }

        part.IsSelected = false;

        anyPartSelectedFlag = false;

        Vector3 targetPosition = part.finalPosition + transform.position;
        StartCoroutine(MovePartSmoothly(part.partTransform, targetPosition));
    }

    private System.Collections.IEnumerator MovePartSmoothly(Transform partTransform, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float transitionDuration = 1f / transitionSpeed;
        Vector3 initialPosition = partTransform.position;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            partTransform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }
        partTransform.position = targetPosition; // Ensure final position is precise
    }

    private System.Collections.IEnumerator MoveAllPartsSmoothly()
    {
        Vector3[] initialPositions = new Vector3[parts.Length];
        Vector3[] targetPositions = new Vector3[parts.Length];
        float transitionDuration = 1f / transitionSpeed;

        for (int i = 0; i < parts.Length; i++)
        {
            initialPositions[i] = parts[i].partTransform.position;
            targetPositions[i] = IsExplodedView ? parts[i].finalPosition + transform.position : transform.position;
        }

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].partTransform.position = Vector3.Lerp(initialPositions[i], targetPositions[i], t);
            }
            yield return null;
        }

        for (int i = 0; i < parts.Length; i++)
        {
            parts[i].partTransform.position = targetPositions[i];
        }

        isToggling = false;
    }
}
