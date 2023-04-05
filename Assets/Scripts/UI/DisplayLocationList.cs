using UnityEngine;
using TMPro;

public class DisplayLocationList : MonoBehaviour
{
    [SerializeField] private GameObject list;
    [SerializeField] private TMP_InputField userInput;
    [SerializeField] private GameObject iconDisplayed, iconHidden;
    private bool isDisplayed;

    void Start()
    {
        HideList();
    }

    private void ActivateIcons()
    {
        iconDisplayed.SetActive(isDisplayed);
        iconHidden.SetActive(!isDisplayed);
    }

    public void ToggleDisplay()
    {
        isDisplayed = !isDisplayed;
        ActivateIcons();
        list.SetActive(isDisplayed);
    }

    public void DisplayList()
    {
        isDisplayed = true;
        ActivateIcons();
        list.SetActive(isDisplayed);
    }

    public void HideList()
    {
        isDisplayed = false;
        ActivateIcons();
        list.SetActive(isDisplayed);
    }
}
