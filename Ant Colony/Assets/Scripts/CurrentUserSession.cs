using UnityEngine;
using UnityEngine.UI;
using System;
using Npgsql;

public class CurrentUserSession : MonoBehaviour
{
    [SerializeField] private Text userLoginText;

    public void UpdateUserLogin(string login)
    {
        userLoginText.text = login;
    }
}
