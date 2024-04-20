using UnityEngine;
using UnityEngine.UI;
using System;
using Npgsql;

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

    private static readonly string connString = "Host=localhost;Username=Medvednik_AS_21;Password=16122003;Database=GameDataBase";

    // Добавляем события для входа и регистрации
    public static Action OnUserSignIn;
    public static Action OnUserSignUp;

    private void Start()
    {
        currentUserSession = FindObjectOfType<CurrentUserSession>();
        // Подписываемся на события в начале
        OnUserSignUp += SignUp;
        OnUserSignIn += SignIn;
    }

    public void SignIn()
    {
        string email = signInEmail.text;
        string password = signInPassword.text;

        SignIn(email, password);
    }

    private void SignIn(string email, string password)
    {
        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM \"User\" WHERE Email = @email";
                cmd.Parameters.AddWithValue("email", email);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedPassword = reader.GetString(reader.GetOrdinal("Password"));
                        if (password == storedPassword)
                        {
                            signInMessageText.text = "Sign in successful!";
                            signInMessageText.color = Color.green;

                            // Активируем Menu Form и деактивируем Login Form при успешном входе
                            menuForm.SetActive(true);
                            loginForm.SetActive(false);

                            // Обновляем текст имени пользователя
                            string userLogin = reader.GetString(reader.GetOrdinal("Username"));
                            currentUserSession.UpdateUserLogin(userLogin);
                        }
                        else
                        {
                            signInMessageText.text = "Incorrect password!";
                            signInMessageText.color = Color.red;
                        }
                    }
                    else
                    {
                        signInMessageText.text = "User not found!";
                        signInMessageText.color = Color.red;
                    }
                }
            }
        }
    }

    public void SignUp()
    {
        string email = signUpEmail.text;
        string login = signUpLogin.text;
        string password = signUpPassword.text;
        string confirmPassword = signUpConfirmPassword.text;

        if (password == confirmPassword)
        {
            UserData userData = new UserData(login, email, password);
            SignUp(userData);
        }
        else
        {
            signUpMessageText.text = "Passwords do not match!";
            signUpMessageText.color = Color.red;
        }
    }

    private void SignUp(UserData userData)
    {
        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO \"User\" (Username, Email, Password) VALUES (@login, @email, @password)";
                cmd.Parameters.AddWithValue("login", userData.Login);
                cmd.Parameters.AddWithValue("email", userData.Email);
                cmd.Parameters.AddWithValue("password", userData.Password);
                cmd.ExecuteNonQuery();

                signUpMessageText.text = "Sign up successful!";
                signUpMessageText.color = Color.green;
            }
        }
    }
}

