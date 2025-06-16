using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SwitchScene : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;
#endif
    [SerializeField] private string sceneName;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
#endif
    }

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneTransition.SwitchToScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene is not set in the Inspector!");
        }
    }
}
