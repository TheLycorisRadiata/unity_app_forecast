using UnityEngine;
using TMPro;

public class DisplayLocationList : MonoBehaviour
{
    [SerializeField] private GameObject list;
    [SerializeField] private TMP_InputField userInput;
    private bool isDisplayed;

    public void DisplayList()
    {
        if (userInput.text != "")
        {
            isDisplayed = true;
            list.SetActive(isDisplayed);
        }
    }

    public void ToggleDisplay()
    {
        isDisplayed = !isDisplayed;
        list.SetActive(isDisplayed);
    }
}
