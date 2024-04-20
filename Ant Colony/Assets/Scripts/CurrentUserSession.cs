using UnityEngine;
using UnityEngine.UI;
using System;

public class CurrentUserSession : MonoBehaviour
{
    [SerializeField] private Text userLogintext;

    public static Action<UserData> OnDisplayUser;

    private void Start()
    {
        OnDisplayUser += DisplayUser;
    }

    private void OnEnable()
    {
        OnDisplayUser += DisplayUser;
    }

    private void OnDisable()
    {
        OnDisplayUser -= DisplayUser;
    }

    private void OnDestroy()
    {
        OnDisplayUser -= DisplayUser;
    }

    private void DisplayUser(UserData userData)
    {
        userLogintext.text = userData.Login;
    }
}
