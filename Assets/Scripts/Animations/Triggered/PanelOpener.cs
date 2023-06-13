using UnityEngine;

public class PanelOpener : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void OpenPanel()
    {
        bool isOpen;

        if (_panel != null && animator != null)
        {
            isOpen = animator.GetBool("open");
            animator.SetBool("open", !isOpen);
        }
    }
}
