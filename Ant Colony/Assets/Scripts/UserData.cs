using UnityEngine;
using Npgsql;
using System;

[System.Serializable]
public class UserData
{
    public string Login;
    public string Email;
    public string Password;

    public UserData(string login, string email, string password)
    {
        this.Login = login;
        this.Email = email;
        this.Password = password;
    }
}
