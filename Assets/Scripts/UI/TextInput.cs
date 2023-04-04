using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TextInput : MonoBehaviour
{
    private TMP_InputField component;
    [SerializeField] private UnityEvent validation;

    void Awake()
    {
        component = GetComponent<TMP_InputField>();

        /* Make sure the input field's Line Type property is set to "Multi Line Newline" */
        component.lineType = TMP_InputField.LineType.MultiLineNewline;
        component.lineLimit = 1;
        /* Then pass the DetectNewLine() method */
        component.onValueChanged.AddListener(DetectNewLine);

        /*
            Note that the OnEndEdit event is not used because it's not only triggered when the user types a newline (typically the "Enter" key), 
            but also when the input field is out of focus, so for example when the user clicks outside of the field. This causes the event methods  
            to be called again, for nothing. Therefore I've implemented my own solution.

            Basically, the input is a "multi line" with the newline not being seen as validation, but as a mere newline. Then, whenever there's a 
            change in input, I check the characters to see if a newline has been entered. Of course, the check starts from the end of the string, 
            because we're interested in newly added characters. If a newline has been detected, the event methods are called.

            The only downside to this method is that, when the user types in a newline, they would see the input on different lines instead of 
            just one (since it's "multi line") and style-wise it's not desired. Which is why when the newline is detected I immediately update 
            the input to remove it. It's seamless and the user doesn't notice a thing.
        */
    }

    private void DetectNewLine(string value)
    {
        int i;
        for (i = value.Length - 1; i >= 0; --i)
        {
            if (value[i] == '\n')
            {
                /* Remove the newline for aesthetic purposes */
                component.text = value.Remove(i, 1);
                /* Newline means "Validate", so call the methods */
                validation.Invoke();
                return;
            }
        }
    }
}
