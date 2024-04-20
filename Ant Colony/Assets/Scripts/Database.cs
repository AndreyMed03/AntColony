using UnityEngine;
using Npgsql;
using System;

public class DataBase
{
    private static readonly string connString = "Host=localhost;Username=Medvednik_AS_21;Password=16122003;Database=GameDataBase";

    public static void SendToDatabase(UserData userData)
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
            }
        }
    }

    public static void GetUserByEmail(string email, Action<Exception, UserData> callback)
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
