using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;

public class ApiClient : MonoBehaviour
{
    private string apiUrl = "http://your-api-url/api/auth";  // ”кажи здесь актуальный URL твоего API

    public IEnumerator PostRequest(string endpoint, string jsonData, System.Action<string> callback)
    {
        string url = $"{apiUrl}/{endpoint}";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
            callback(null);
        }
    }
}
