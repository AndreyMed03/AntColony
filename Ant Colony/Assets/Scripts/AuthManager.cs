using UnityEngine;
using UnityEngine.UI;
using System;
using Npgsql;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

public class AuthManager : MonoBehaviour
{
    public GameObject menuForm;
    public GameObject loginForm;
    public InputField signInEmail;
    public InputField signInPassword;
    public InputField signUpLogin;
    public InputField signUpEmail;
    public InputField signUpPassword;
    public InputField signUpConfirmPassword;
    public Text signUpMessageText;
    public Text signInMessageText;
    private CurrentUserSession currentUserSession;

    public static Action OnUserSignIn;
    public static Action OnUserSignUp;

    private ApiClient apiClient;

    private void Start()
    {
        currentUserSession = FindObjectOfType<CurrentUserSession>();
        apiClient = FindObjectOfType<ApiClient>();

        // Подписка на события для регистрации и входа
        OnUserSignUp += SignUp;
        OnUserSignIn += SignIn;
    }

    public void SignIn()
    {
        string signInInput = signInEmail.text;
        string password = signInPassword.text;

        UserLogin userLogin = new UserLogin { Username = signInInput, Password = password };
        string jsonData = JsonUtility.ToJson(userLogin);

        StartCoroutine(apiClient.PostRequest("login", jsonData, HandleSignInResponse));
    }

    private void HandleSignInResponse(string response)
    {
        if (!string.IsNullOrEmpty(response))
        {
            AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(response);
            if (authResponse.Success)
            {
                signInMessageText.text = "Sign in successful!";
                signInMessageText.color = Color.green;

                menuForm.SetActive(true);
                loginForm.SetActive(false);

                currentUserSession.UpdateUserLogin(authResponse.Username);
            }
            else
            {
                signInMessageText.text = authResponse.Message;
                signInMessageText.color = Color.red;
            }
        }
        else
        {
            signInMessageText.text = "Error connecting to server!";
            signInMessageText.color = Color.red;
        }
    }

    public void SignUp()
    {
        string email = signUpEmail.text;
        string login = signUpLogin.text;
        string password = signUpPassword.text;
        string confirmPassword = signUpConfirmPassword.text;

        if (password != confirmPassword)
        {
            signUpMessageText.text = "Passwords do not match!";
            signUpMessageText.color = Color.red;
        }
        else if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            signUpMessageText.text = "Введите все данные";
            signUpMessageText.color = Color.red;
        }
        else if (!Regex.IsMatch(email, @"^(?:[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$"))
        {
            signUpMessageText.text = "введите рабочую пошту!";
            signUpMessageText.color = Color.red;
        }
        else
        {
            UserRegister userRegister = new UserRegister { Username = login, Email = email, Password = password };
            string jsonData = JsonUtility.ToJson(userRegister);

            StartCoroutine(apiClient.PostRequest("register", jsonData, HandleSignUpResponse));
        }
    }

    private void HandleSignUpResponse(string response)
    {
        if (!string.IsNullOrEmpty(response))
        {
            AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(response);
            signUpMessageText.text = authResponse.Message;
            signUpMessageText.color = authResponse.Success ? Color.green : Color.red;
        }
        else
        {
            signUpMessageText.text = "Error connecting to server!";
            signUpMessageText.color = Color.red;
        }
    }

    // Внутренние классы для обработки данных
    [System.Serializable]
    private class UserLogin
    {
        public string Username;
        public string Password;
    }

    [System.Serializable]
    private class UserRegister
    {
        public string Username;
        public string Email;
        public string Password;
    }

    [System.Serializable]
    private class AuthResponse
    {
        public string Message;
        public bool Success;
        public string Username;
    }
}