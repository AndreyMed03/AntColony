using UnityEngine;
using Npgsql;
using System;
using System.IO;

public class DataBase : MonoBehaviour
{
    private static string configFilePath = "config"; // Путь к файлу конфигурации в папке Resources

    private static string GetConnectionString()
    {
        // Загружаем содержимое файла конфигурации
        TextAsset configFile = Resources.Load<TextAsset>(configFilePath);
        if (configFile != null)
        {
            // Читаем данные конфигурации
            DatabaseConfig databaseConfig = JsonUtility.FromJson<DatabaseConfig>(configFile.text);
            // Формируем строку подключения
            return $"Host={databaseConfig.Host};Username={databaseConfig.Username};Password={databaseConfig.Password};Database={databaseConfig.Database}";
        }
        else
        {
            Debug.LogError("Config file not found!");
            return null;
        }
    }

    // Получение пользователя по электронной почте
    public static void GetUserByEmail(string email, Action<Exception, UserData> callback)
    {
        string connString = GetConnectionString();
        if (connString != null)
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
                            UserData userData = new UserData(
                                reader.GetString(reader.GetOrdinal("Username")),
                                reader.GetString(reader.GetOrdinal("Email")),
                                reader.GetString(reader.GetOrdinal("Password"))
                            );
                            callback(null, userData);
                        }
                        else
                        {
                            callback(new Exception("User not found"), null);
                        }
                    }
                }
            }
        }
    }
}

// Класс для хранения данных конфигурации
[System.Serializable]
public class DatabaseConfig
{
    public string Host;
    public string Username;
    public string Password;
    public string Database;
}