using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections;

public class ApiClient : MonoBehaviour
{
    private string apiUrl = "http://localhost:5004/api/auth";

    public IEnumerator PostRequest(string endpoint, string jsonData, System.Action<string> callback)
    {
        string url = $"{apiUrl}/{endpoint}";
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Успешный ответ
                callback(request.downloadHandler.text);
            }
            else
            {
                callback(request.downloadHandler.text);
            }
        } // Освобождаем ресурсы
    }
}
