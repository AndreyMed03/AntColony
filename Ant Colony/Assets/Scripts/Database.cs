using Proyecto26;
using System;

public class DataBase
{
    public static readonly string DATABASE = "https://ant-colony-d793a-default-rtdb.firebaseio.com/users";

    public static void SendToDatabase(UserData userData, string separator)
    {
        RestClient.Put<UserData>(DATABASE + "/" + separator + ".json", userData);
    }

    public static void GetUserByLogin(string login, Action<RequestException, ResponseHelper, UserData> GetInfoCallback)
    {
        RestClient.Get<UserData>($"{DATABASE}/{login}.json", GetInfoCallback);
    }
    public static void FindUserByEmail(string email, Action<RequestException, ResponseHelper> GetInfoCallback)
    {
        RestClient.Get($"{DATABASE}.json?orderBy=%22Email%22&equalTo=%22{email}%22", GetInfoCallback);
    }
}
