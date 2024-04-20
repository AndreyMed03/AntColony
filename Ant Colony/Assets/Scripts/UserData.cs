using UnityEngine;

[System.Serializable]
public class UserData
{
    public string Login;
    public string Email;
    public string Password;
    public string ConfirmPassword;
    public UserData(string login, string email, string password, string confirmPassword) 
    {
        this.Login = login;
        this.Email = email;
        this.Password = password;
        this.ConfirmPassword = confirmPassword;
    }
}
