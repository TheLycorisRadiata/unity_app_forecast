using UnityEngine;
using TMPro;

public class MenuPointer : MonoBehaviour
{
    public static bool IsPointerOnMenu { get; private set; }
    [SerializeField] private TMP_InputField _geocodingTextInput;

    private void Awake()
    {
        IsPointerOnMenu = false;
    }

    public void OnPointerEnter()
    {
        IsPointerOnMenu = true;
        _geocodingTextInput.interactable = true;
    }

    public void OnPointerExit()
    {
        IsPointerOnMenu = false;
        _geocodingTextInput.interactable = false;
    }
}
