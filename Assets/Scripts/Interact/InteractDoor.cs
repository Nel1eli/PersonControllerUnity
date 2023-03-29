using UnityEngine;

public class InteractDoor : MonoBehaviour
{
    private bool isOpened;

    [SerializeField] private Animator animator;

    public void Open()
    {
        animator.SetBool("isOpened", isOpened);
        isOpened = !isOpened;
    } 
}
