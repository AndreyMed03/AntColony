using UnityEngine;
using UnityEngine.UI;
using System;
using Npgsql;
using TMPro;

public class CurrentUserSession : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userLoginText;

    public void UpdateUserLogin(string login)
    {
        userLoginText.text = login;
    }
}
