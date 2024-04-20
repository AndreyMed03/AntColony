using Proyecto26;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class AuthManager : MonoBehaviour
{
    public GameObject menuForm;
    public GameObject loginForm;

    [Header("Login / Sign In")]
    public InputField signInEmail;
    public InputField signInPassword;

    [Header("Register / Sign Up")]
    public InputField signUpLogin;
    public InputField signUpEmail;
    public InputField signUpPassword;
    public InputField signUpConfirmPassword;

    [Space(10)]
    public Text signUpMessageText;
    public Text signInMessageText;

    private string AUTH_KEY = "AIzaSyADTTP_O2WeV2tHCteBU0-Av-QRrfOmt1w";

    public static Action OnUserSignIn; //login
    public static Action OnUserSignUp; //register

    private void Start()
    {
        OnUserSignUp += SignUp;
        OnUserSignIn += SignIn;
    }

    private void OnEnable()
    {
        OnUserSignUp += SignUp;
        OnUserSignIn += SignIn;
    }

    private void OnDisable()
    {
        OnUserSignUp -= SignUp;
        OnUserSignIn -= SignIn;
    }

    private void OnDestroy()
    {
        OnUserSignUp -= SignUp;
        OnUserSignIn -= SignIn;
    }

    public void GetUserByEmail(string email)
    {
        DataBase.FindUserByEmail(email, GetUserByEmail);
    }
    public void GetUserByEmail(RequestException exception, ResponseHelper helper)
    {
        try
        {
            Dictionary<string, UserData> dict = JsonConvert.DeserializeObject<Dictionary<string, UserData>>(helper.Text);
            
            foreach (KeyValuePair<string, UserData> keyValue in dict)
            {
                menuForm.SetActive(true);
                loginForm.SetActive(false);

                CurrentUserSession.OnDisplayUser?.Invoke(keyValue.Value);
                break;
            }
        }
        catch(Exception e)
        {
            Debug.Log("User Data not Loaded!");
        }
    }

    public void SignIn()
    {
        string email = signInEmail.text;
        string password = signInPassword.text;

        SignIn(email, password);
    }

    public void SignIn(string email, string password) 
    { 
        signInMessageText.text = "Search Account...";
        signInMessageText.color = Color.white;

        string data = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<AuthData>($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={AUTH_KEY}", data, SignInCallback);
    }

    public void SignInCallback(RequestException exception, ResponseHelper helper, AuthData data)
    {
        try
        {
            signInMessageText.text = "Account Initialised!";
            signInMessageText.color = Color.green;
            GetUserByEmail(data.email);
        }
        catch (Exception e)
        {
            signInMessageText.text = exception.Message;
            signInMessageText.color = Color.red;
        }
    }

    public void SignUp()
    {
        string email = signUpEmail.text;
        string login = signUpLogin.text;
        string password = signUpPassword.text;
        
        bool IsEmailEmpty = string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email);
        bool IsPasswordEmpty = string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password);

        signUpMessageText.text = "Waiting, your account creating...";
        signUpMessageText.color = Color.white;

        if (!string.IsNullOrEmpty(login)) 
        {
            DataBase.GetUserByLogin(login, GetUserByLoginCallback);
        }
    }

    public void GetUserByLoginCallback(RequestException exception, ResponseHelper helper, UserData userData)
    {
        if (userData == null)
        {
            if (signUpPassword.text.Length >= 6 && signUpConfirmPassword.text.Length >= 6)
            {
                if (signUpConfirmPassword.text == signUpPassword.text)
                {
                    string data = "{\"email\":\"" + signUpEmail.text + "\",\"password\":\"" + signUpPassword.text + "\",\"returnSecureToken\":true}";
                    RestClient.Post<AuthData>($"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={AUTH_KEY}", data, SignUpCallback);
                }
                else if (signUpConfirmPassword.text != signUpPassword.text)
                {
                    signUpMessageText.text = "Passwords not equals!";
                    signUpMessageText.color = Color.red;
                }
            }
            else if (signInPassword.text.Length < 6 && signUpConfirmPassword.text.Length < 6)
            {
                signUpMessageText.text = "Passwords not enough length!";
                signUpMessageText.color = Color.red;
            }
        }
        else
        {
            signUpMessageText.text = "Account with your login have in system!";
            signUpMessageText.color = Color.red;
        }
    }

    public void SignUpCallback(RequestException exception, ResponseHelper helper, AuthData data) 
    {
        try
        {
            string Login = signUpLogin.text;
            string Email = signUpEmail.text;
            string Password = signUpPassword.text;
            string ConfirmPassword = signUpConfirmPassword.text;

            signUpMessageText.text = "Account created!";
            signUpMessageText.color = Color.green;

            UserData userData = new UserData(Login, Email, Password, ConfirmPassword);

            DataBase.SendToDatabase(userData, Login);
        }
        catch (Exception e) 
        {
            signUpMessageText.text = e.Message;
            signUpMessageText.color = Color.red;
        }
    }
}

[System.Serializable]
public class AuthData
{
    public string localId;
    public string email;
}

