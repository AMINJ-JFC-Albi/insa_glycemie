using System.Collections;
using System.Collections.Generic;
using HoloWatch;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    
    public StoryData storyData;

    public Transform holoWatch;
    public Transform rightAnchorHoloWatch;
    public Transform leftAnchorHoloWatch;
    
    private List<string> objIds;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    
    void Start() {
        holoWatch.SetParent(rightAnchorHoloWatch);
        objIds = new List<string> { storyData.mainObjective.objectiveId };
        foreach (ObjectiveData obj in storyData.objectives) {
            objIds.Add(obj.objectiveId);
        }
        HolowatchUI.Instance.InitStory(storyData);
        StartCoroutine(DocMessage());
    }

    private IEnumerator DocMessage() {
        yield return new WaitForSeconds(5f);
        HolowatchUI.Instance.AddMessage(objIds[0], "First");
        yield return new WaitForSeconds(10f);
        HolowatchUI.Instance.AddMessage(objIds[0], "PS");
        HolowatchUI.Instance.SetNextHints(objIds[1], new List<string>() {"1-1"});
    }
    
    public void LoadScene(string sceneName) {
        StartCoroutine(LoadSceneVR(sceneName));
    }

    IEnumerator LoadSceneVR(string sceneName) {
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(sceneName);
    }
}
