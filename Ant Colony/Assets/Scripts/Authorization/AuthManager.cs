using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

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
    private ApiClient apiClient;

    public static Action OnUserSignIn;
    public static Action OnUserSignUp;

    private void Start()
    {
        currentUserSession = FindObjectOfType<CurrentUserSession>();
        apiClient = FindObjectOfType<ApiClient>();

        OnUserSignUp += SignUp;
        OnUserSignIn += SignIn;
    }

    public void SignIn()
    {
        signInMessageText.text = "Search Account...";
        signInMessageText.color = Color.white;
        string signInInput = signInEmail.text;
        string password = signInPassword.text;

        UserLogin userLogin = new UserLogin { Username = signInInput, Password = password };
        string jsonData = JsonConvert.SerializeObject(userLogin);

        StartCoroutine(apiClient.PostRequest("login", jsonData, HandleSignInResponse));
    }

    private void HandleSignInResponse(string response)
    {
        Debug.Log($"Sign In Response: {response}");

        if (!string.IsNullOrEmpty(response))
        {
            AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(response);
            Debug.Log($"Auth Response: Message = {authResponse.Message}, Success = {authResponse.success}, Username = {authResponse.Username}");

            signInMessageText.text = authResponse.Message;
            signInMessageText.color = authResponse.success ? Color.green : Color.red;

            if (authResponse.success)
            {
                Debug.Log("Switching to menuForm.");
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
        signUpMessageText.text = "Your account is created...";
        string email = signUpEmail.text;
        string login = signUpLogin.text;
        string password = signUpPassword.text;
        string confirmPassword = signUpConfirmPassword.text;

        if (!Regex.IsMatch(email, @"^(?:[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$"))
        {
            signUpMessageText.text = "Enter correct Email!";
            signUpMessageText.color = Color.red;
        }
        else if (login.Length > 14 || login.Length < 6)
        {
            signUpMessageText.text = "Invalid login length!";
            signUpMessageText.color = Color.red;
        }
        else if(password.Length < 6)
        {
            signUpMessageText.text = "Invalid password length!";
            signUpMessageText.color = Color.red;
        }
        else if (password != confirmPassword)
        {
            signUpMessageText.text = "Passwords don't match!";
            signUpMessageText.color = Color.red;
        }
        else if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            signUpMessageText.text = "Enter all fields!";
            signUpMessageText.color = Color.red;
        }
        else
        {
            UserRegister userRegister = new UserRegister { Username = login, Email = email, Password = password };
            string jsonData = JsonConvert.SerializeObject(userRegister);

            StartCoroutine(apiClient.PostRequest("register", jsonData, HandleSignUpResponse));
        }
    }

    private void HandleSignUpResponse(string response)
    {
        if (!string.IsNullOrEmpty(response))
        {
            AuthResponse authResponse = JsonConvert.DeserializeObject<AuthResponse>(response);
            signUpMessageText.text = authResponse.Message;
            signUpMessageText.color = authResponse.success ? Color.green : Color.red;
        }
        else
        {
            signUpMessageText.text = "Error connecting to server!";
            signUpMessageText.color = Color.red;
        }
    }

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
        public bool success;
        public string Username;
    }
}
