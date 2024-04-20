using UnityEngine;
using UnityEngine.EventSystems;

public class AuthButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ButtonType buttonType = ButtonType.None;
    private enum ButtonType
    {
        None = 0,
        SignIn = 0,
        SignUp = 2

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (buttonType == ButtonType.SignIn)
        {
            AuthManager.OnUserSignIn?.Invoke();
        }
        else if (buttonType == ButtonType.SignUp)
        {
            AuthManager.OnUserSignUp?.Invoke();
        }
    }
}
