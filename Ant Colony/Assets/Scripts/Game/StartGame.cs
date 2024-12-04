using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] private SceneAsset sceneAsset;

    private string sceneName;

    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
    }

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene is not set in the Inspector!");
        }
    }
}
