using UnityEngine;
using UnityEngine.UI;
using System;
using Npgsql;
using System.Security.Cryptography;
using System.Text;
using System.IO;

public class AuthManager : MonoBehaviour
{
    private static string configFilePath = "config"; // Путь к файлу конфигурации в папке Resources
    private DatabaseConfig databaseConfig; // Данные конфигурации базы данных

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

    private void Start()
    {
        // Загружаем данные конфигурации из файла
        LoadConfig();

        currentUserSession = FindObjectOfType<CurrentUserSession>();
        OnUserSignUp += SignUp;
        OnUserSignIn += SignIn;
    }

    private void LoadConfig()
    {
        // Загружаем содержимое файла конфигурации
        TextAsset configFile = Resources.Load<TextAsset>(configFilePath);
        if (configFile != null)
        {
            // Читаем данные конфигурации
            databaseConfig = JsonUtility.FromJson<DatabaseConfig>(configFile.text);
        }
        else
        {
            Debug.LogError("Config file not found!");
        }
    }

    public void SignIn()
    {
        string signInInput = signInEmail.text;
        string password = signInPassword.text;

        string hashedPassword = HashPassword(password);

        SignIn(signInInput, hashedPassword);
    }

    private void SignIn(string signInInput, string hashedPassword)
    {
        using (var conn = new NpgsqlConnection(GetConnectionString()))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM \"User\" WHERE Email = @input OR Username = @input";
                cmd.Parameters.AddWithValue("input", signInInput);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedHashedPassword = reader.GetString(reader.GetOrdinal("Hashed_Password"));
                        if (hashedPassword == storedHashedPassword)
                        {
                            signInMessageText.text = "Sign in successful!";
                            signInMessageText.color = Color.green;

                            menuForm.SetActive(true);
                            loginForm.SetActive(false);

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
            string hashedPassword = HashPassword(password);

            UserData userData = new UserData(login, email, hashedPassword);
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
        using (var conn = new NpgsqlConnection(GetConnectionString()))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO \"User\" (Username, Email, Hashed_Password) VALUES (@login, @email, @password)";
                cmd.Parameters.AddWithValue("login", userData.Login);
                cmd.Parameters.AddWithValue("email", userData.Email);
                cmd.Parameters.AddWithValue("password", userData.Password);
                cmd.ExecuteNonQuery();

                signUpMessageText.text = "Sign up successful!";
                signUpMessageText.color = Color.green;
            }
        }
    }

    private string GetConnectionString()
    {
        return $"Host={databaseConfig.Host};Username={databaseConfig.Username};Password={databaseConfig.Password};Database={databaseConfig.Database}";
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    private class DatabaseConfig
    {
        public string Host;
        public string Username;
        public string Password;
        public string Database;
    }
}