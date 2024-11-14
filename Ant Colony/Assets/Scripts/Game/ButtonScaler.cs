using UnityEngine.EventSystems;
using UnityEngine;

public class ButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float exitScale = 1.0f;
    [SerializeField] private float enterScale = 1.06f;

    private void OnDisable()
    {
        transform.localScale = new Vector3(exitScale, exitScale, exitScale);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(enterScale, enterScale, enterScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector3(exitScale, exitScale, exitScale);
    }
}

