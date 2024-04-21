using UnityEngine;
using UnityEngine.EventSystems;
using System.Security.Cryptography;

public class AuthButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ButtonType buttonType = ButtonType.None;

    private enum ButtonType
    {
        None = 0,
        SignIn = 1, // различные значения для SignIn и SignUp
        SignUp = 2
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.SignIn:
                AuthManager.OnUserSignIn?.Invoke();
                break;
            case ButtonType.SignUp:
                AuthManager.OnUserSignUp?.Invoke();
                break;
            default:
                Debug.LogWarning("Unknown button type!");
                break;
        }
    }
}
