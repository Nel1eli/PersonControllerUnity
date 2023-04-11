using UnityEngine;

public class InteractDoor : MonoBehaviour
{
    private bool isOpened = true;

    [SerializeField] private AudioSource open_Door;
    [SerializeField] private AudioSource close_Door;
    [SerializeField] private Animator animator;

    public void InteractWithDoor()
    {
        if (isOpened) open_Door.Play();
        else close_Door.Play();

        animator.SetBool("isOpened", isOpened);

        isOpened = !isOpened;
    } 
}
