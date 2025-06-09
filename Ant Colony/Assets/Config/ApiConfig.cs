using UnityEngine;

[CreateAssetMenu(fileName = "ApiConfig", menuName = "Configs/API Config")]
public class ApiConfig : ScriptableObject
{
    public string apiBaseUrl = "http://localhost:5004/api/auth";
}
