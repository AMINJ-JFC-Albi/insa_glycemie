using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneVR(sceneName));
    }

    IEnumerator LoadSceneVR(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(sceneName);
    }
}
