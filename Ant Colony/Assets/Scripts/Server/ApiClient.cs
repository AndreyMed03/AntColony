using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    [SerializeField] private ApiConfig config;

    public IEnumerator PostRequest(string endpoint, string jsonData, Action<string> onSuccess, Action<string> onError = null)
    {
        if (config == null)
        {
            Debug.LogError("API Config is not assigned in ApiClient.");
            yield break;
        }

        string url = $"{config.apiBaseUrl}/{endpoint}";
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

#if UNITY_2020_2_OR_NEWER
            yield return request.SendWebRequest();
#else
            yield return request.Send();
#endif

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                string errorMessage = $"Error {request.responseCode}: {request.error}\n{request.downloadHandler.text}";
                Debug.LogError(errorMessage);
                onError?.Invoke(errorMessage);
            }
        }
    }
}
