using UnityEngine;
using UnityEngine.UI;

public class QuestReadPaper : MonoBehaviour
{
    bool questComplete = false;
    private Animator animator;
    private Collider colliderObject;
    [Header("Trigger Player")]
    [SerializeField] private Collider collider;
    [Header("Audio take Quest")]
    [SerializeField] private AudioSource audioSource;
    [Header("Quest")]
    [SerializeField] private Image image;

    // Update is called once per frame
    private void Start()
    {
        colliderObject = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider colliderEnter)
    {
        if (colliderEnter.gameObject.name == collider.name)
        {
            if (!questComplete ) audioSource.Play();
            animator = image.gameObject.GetComponent<Animator>();
            animator.SetBool("QuestState", true);
            gameObject.SetActive(false);
            questComplete = true;
            colliderObject.gameObject.SetActive(false);
        }
    }

}
