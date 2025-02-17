using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private int audioIndex = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AudioManager audioManagerScript = (AudioManager)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Test Play audio", EditorStyles.boldLabel);
        audioIndex = EditorGUILayout.IntField("Audio Index", audioIndex);

        if (GUILayout.Button("Play audio Intro"))
        {
            audioManagerScript.LoadAudio(0);
        }

        if (GUILayout.Button("Play audio ExplainGCM_1"))
        {
            audioManagerScript.LoadAudio(1);
        }
    }
}
#endif

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audioClips;

    public int index = 0;
    private int currentIndex = 0; 
    private AudioSource audioSource; 

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        /*audioSource.clip = audioClips[index];
        audioSource.Play();*/
    }

    public void LoadAudio(int index)
    {
        currentIndex = index;
        audioSource.clip = audioClips[currentIndex];
        audioSource.Play();
    }
}
