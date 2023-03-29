using UnityEngine;

public class Quest : MonoBehaviour
{
    private bool isLockQuestMenu = false; // Проверка паузы
    [SerializeField] private GameObject questMenu;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            QuestMenu();
        }
    }

    private void QuestMenu()
    {
        isLockQuestMenu = !isLockQuestMenu;
        questMenu.SetActive(isLockQuestMenu);


    }

}
