using UnityEngine;
using UnityEngine.UI;

public class CleanFormData : MonoBehaviour
{
    [SerializeField] private InputField[] inputFields;
    [SerializeField] private Text[] texts;

    private void OnDisable()
    {
        foreach (var inputField in inputFields)
        {
            inputField.text = null;
        }

        foreach (var text in texts)
        {
            text.text = null;
        }
    }
}
