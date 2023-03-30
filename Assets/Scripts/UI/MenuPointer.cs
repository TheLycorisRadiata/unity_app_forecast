using UnityEngine;
using TMPro;

public class MenuPointer : MonoBehaviour
{
    public static bool isPointerOnMenu = false;
    [SerializeField] private TMP_InputField textInput;

    public void OnPointerEnter()
    {
        isPointerOnMenu = true;
        textInput.interactable = true;
    }

    public void OnPointerExit()
    {
        isPointerOnMenu = false;
        textInput.interactable = false;
    }
}
