using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using UnityEngine.SceneManagement;
//using Firebase.Analytics;

public class LevelLoader : MonoBehaviour
{

    private void Start()
    {
        LoadLevel(0);
    }

    public void LoadLevel (int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            Debug.Log(operation.progress);
            yield return null;
        }
    }

}
