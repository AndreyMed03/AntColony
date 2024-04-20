using UnityEngine.EventSystems;
using UnityEngine;

public class ButtonBack : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject activate;
    [SerializeField] private GameObject disactivate;
    public void OnPointerClick(PointerEventData eventData)
    {
        activate.SetActive(true);
        disactivate.SetActive(false);
    }
}
